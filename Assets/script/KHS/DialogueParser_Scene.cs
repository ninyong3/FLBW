using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DialogueParser_Scene : MonoBehaviour
{
    // CSV 헤더(8열): Character,Switch,Switch Text,Script,SCG,Background,BGM,Sound Effect
    public Dialogue[] Parse(string dialogFileName)
    {
        var list = new List<Dialogue>();
        TextAsset ta = Resources.Load<TextAsset>("scenario/" + dialogFileName);
        if (ta == null)
        {
            Debug.LogError($"[Parser_Scene] TextAsset not found: scenario/{dialogFileName}");
            return list.ToArray();
        }

        string text = ta.text;
        if (!string.IsNullOrEmpty(text) && text[0] == '\uFEFF') text = text.Substring(1); // BOM 제거
        text = text.Replace("\r\n", "\n"); // 개행 통일

        var lines = text.Split('\n');
        var csv = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"); // 따옴표 안 콤마 무시

        for (int i = 0; i < lines.Length; i++)
        {
            var raw = lines[i];
            if (string.IsNullOrWhiteSpace(raw)) continue;
            if (i == 0 || raw.StartsWith("Character,")) continue; // 헤더 스킵

            var row = csv.Split(raw);

            // 8열 패딩(부족시 채우기 → IndexOutOfRange 방지)
            if (row.Length < 8)
            {
                var tmp = new string[8];
                for (int k = 0; k < 8; k++) tmp[k] = (k < row.Length) ? row[k] : string.Empty;
                row = tmp;
                Debug.LogWarning($"[Parser_Scene] padded columns at line {i+1}: {raw}");
            }

            var d = new Dialogue();

            d.name        = row[0];
            d.choiceIndex = row[1];

            row[2] = row[2].Trim();
            if (row[2].StartsWith("\"") && row[2].EndsWith("\"")) row[2] = row[2].Substring(1, row[2].Length - 2);
            d.choiceline  = row[2].Replace("\\n", "\n");

            row[3] = row[3].Trim();
            if (row[3].StartsWith("\"") && row[3].EndsWith("\"")) row[3] = row[3].Substring(1, row[3].Length - 2);
            d.line        = row[3].Replace("\\n", "\n");

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
