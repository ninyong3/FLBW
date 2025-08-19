using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

/// <summary>
/// CSV(TextAsset) → Dialogue[] 파서.
/// - 우선 scenario/<file>, 실패 시 scenario/<scene>/<file>도 자동 시도
/// - 엑셀 'sep=,' 행, BOM, 개행 통일 처리
/// - 열 수 부족 시 8열로 패딩
/// </summary>
public class DialogueParser_DialogOnly : MonoBehaviour
{
    // 8열: Character,Switch,Switch Text,Script,SCG,Background,BGM,Sound Effect
    public Dialogue[] Parse(string fileNameNoExt)
    {
        var list = new List<Dialogue>();
        if (string.IsNullOrWhiteSpace(fileNameNoExt)) return list.ToArray();

        // A. scenario/<file>
        TextAsset ta = Resources.Load<TextAsset>($"scenario/{fileNameNoExt}");

        // B. 실패했고 경로가 없으면 scenario/<scene>/<file> 시도
        if (ta == null && !fileNameNoExt.Contains("/"))
        {
            string scene = SceneManager.GetActiveScene().name;
            ta = Resources.Load<TextAsset>($"scenario/{scene}/{fileNameNoExt}");
        }

        if (ta == null)
        {
            Debug.LogError($"[Parser_DialogOnly] TextAsset not found: " +
                           $"scenario/{fileNameNoExt}.csv (scene subfolder fallback도 실패)");
            return list.ToArray();
        }

        // 텍스트 정리
        string text = ta.text ?? "";
        if (text.Length > 0 && text[0] == '\uFEFF') text = text.Substring(1); // BOM 제거
        text = text.Replace("\r\n", "\n").Replace("\r", "\n");

        // 엑셀 'sep=,' 헤더 제거
        if (text.StartsWith("sep=", System.StringComparison.OrdinalIgnoreCase))
        {
            int nl = text.IndexOf('\n');
            text = (nl >= 0) ? text.Substring(nl + 1) : string.Empty;
        }

        var lines = text.Split('\n');
        var csv = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"); // 따옴표 안 콤마 무시

        for (int i = 0; i < lines.Length; i++)
        {
            var raw = lines[i];
            if (string.IsNullOrWhiteSpace(raw)) continue;
            if (i == 0 || raw.StartsWith("Character,")) continue; // 헤더 스킵

            var row = csv.Split(raw);

            // 8열 패딩
            if (row.Length < 8)
            {
                var tmp = new string[8];
                for (int k = 0; k < 8; k++) tmp[k] = (k < row.Length) ? row[k] : string.Empty;
                row = tmp;
            }

            var d = new Dialogue();
            d.name        = row[0];
            d.choiceIndex = row[1];

            // Switch Text
            string s2 = row[2].Trim();
            if (s2.StartsWith("\"") && s2.EndsWith("\"")) s2 = s2.Substring(1, s2.Length - 2);
            d.choiceline  = s2.Replace("\\n", "\n");

            // Script
            string s3 = row[3].Trim();
            if (s3.StartsWith("\"") && s3.EndsWith("\"")) s3 = s3.Substring(1, s3.Length - 2);
            d.line       = s3.Replace("\\n", "\n");

            // 나머지 컬럼은 보존만
            var ci = row[4].Split('_'); // 예: "1_2"
            if (ci.Length == 2 &&
                int.TryParse(ci[0], out d.characterIndex[0]) &&
                int.TryParse(ci[1], out d.characterIndex[1])) { }
            else d.characterIndex[0] = -1;

            d.backgroundIndex = int.TryParse(row[5], out var bg)  ? bg  : -1;
            d.BGMIndex        = int.TryParse(row[6], out var bgm) ? bgm : -1;
            d.SFXIndex        = int.TryParse(row[7], out var sfx) ? sfx : -1;

            list.Add(d);
        }

        return list.ToArray();
    }
}
