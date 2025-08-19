using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전용 대화 부트스트랩.
/// - 히로인별 CSV 시퀀스를 사용(인덱스 순환), 비어있으면 자동 규칙/DB기본으로 폴백
/// - 시퀀스에 폴더가 없으면 자동으로 &lt;현재씬&gt;/ 접두어를 붙여 검색
/// - 파일은 Resources/scenario/ 기준 경로(확장자 제외)
/// </summary>
[DefaultExecutionOrder(-4000)]
public class SceneDialogueFromSimple : MonoBehaviour
{
    [Header("히로인별 CSV 시퀀스(확장자 제외, Resources/scenario 기준)\n- 폴더 미포함이면 자동으로 <현재씬>/ 이 붙습니다.")]
    public List<string> jinCsvSequence    = new List<string>();
    public List<string> freyjaCsvSequence = new List<string>();
    public List<string> ruCsvSequence     = new List<string>();

    [Tooltip("같은 (scene,heroine)로 들어올 때 방문 회수(0→1→2…)로 순환")]
    public bool persistSequenceThisSession = true;

    // (scene|heroine) -> 방문 회수
    static readonly Dictionary<string, int> _occ = new Dictionary<string, int>(System.StringComparer.OrdinalIgnoreCase);

    void Start() => Bootstrap();

    /// <summary>Day가 바뀔 때 호출하면 시퀀스 인덱스를 0으로 되돌립니다.</summary>
    public static void ResetSequenceForDay() => _occ.Clear();

    void Bootstrap()
    {
        // DBManager_DialogOnly 확보(없으면 동적으로 생성)
        DBManager_DialogOnly db =
#if UNITY_2023_1_OR_NEWER
            Object.FindFirstObjectByType<DBManager_DialogOnly>(FindObjectsInactive.Include);
#else
            Object.FindObjectOfType<DBManager_DialogOnly>(true);
#endif
        if (db == null) db = new GameObject("DBManager_DialogOnly").AddComponent<DBManager_DialogOnly>();

        string scene = SceneManager.GetActiveScene().name;

        var reg = SimpleBubbleRegistry.Instance;
        if (reg == null || !reg.TryGetBubbleIdByScene(scene, out var bubbleId))
        {
            // 배정된 버블이 없으면 DB의 기본값(인스펙터에 입력한 Jin/Freyja/Ru) 사용됨
            Debug.LogWarning("[SDFS] 오늘 이 씬에 배정된 버블 없음 → DB 기본값 유지");
            return;
        }

        // bubbleID가 곧 파일명(경로포함)일 수도 있으니 먼저 직접 시도
        string direct = bubbleId?.Trim();
        if (TryResolveExisting(scene, direct, out var directPath))
        {
            db.Load(directPath);
            Debug.Log($"[SDFS] 파일명 직접 사용: '{directPath}.csv'");
            return;
        }

        // bubble → heroine key 정규화
        string heroine = ToHeroineKey(bubbleId);
        if (string.IsNullOrEmpty(heroine))
        {
            Debug.LogWarning($"[SDFS] 알 수 없는 bubble '{bubbleId}'");
            return;
        }

        // (scene|heroine) 방문 회수
        int occ = 0;
        if (persistSequenceThisSession)
        {
            string key = $"{scene.ToLowerInvariant()}|{heroine}";
            _occ.TryGetValue(key, out occ);
            _occ[key] = occ + 1;
        }

        // 1) 시퀀스 사용
        string fromSeq = PickFromSequence(heroine, occ);
        if (TryResolveExisting(scene, fromSeq, out var seqPath))
        {
            db.Load(seqPath);
            Debug.Log($"[SDFS] scene='{scene}', heroine='{heroine}', occ={occ} → '{seqPath}.csv'");
            return;
        }

        // 2) 자동 규칙: <scene>/<scene>_<Heroine>_<occ>
        string autoName = $"{scene}_{HeroineNameForFile(heroine)}_{occ}";
        if (TryResolveExisting(scene, autoName, out var autoPath))
        {
            db.Load(autoPath);
            Debug.Log($"[SDFS] 자동 규칙 사용: '{autoPath}.csv'");
            return;
        }

        // 3) DB 기본값으로 폴백
        string fallback = heroine == "jin" ? db.csvForJin
                          : heroine == "freyja" ? db.csvForFreyja
                          : db.csvForRu;

        if (TryResolveExisting(scene, fallback, out var fbPath))
        {
            db.Load(fbPath);
            Debug.Log($"[SDFS] DB 기본값 사용: '{fbPath}.csv'");
            return;
        }

        Debug.LogError($"[SDFS] CSV를 찾을 수 없습니다. bubble='{bubbleId}', " +
                       $"seq='{fromSeq}', auto='{autoName}', fallback='{fallback}'");
    }

    // --- 도우미 ---

    static string ToHeroineKey(string bubbleId)
    {
        string s = (bubbleId ?? "").ToLowerInvariant();
        if (s.Contains("jin"))    return "jin";
        if (s.Contains("freyja")) return "freyja";
        if (s.Contains("ru"))     return "ru";
        return null;
    }

    static string HeroineNameForFile(string heroine)
    {
        // 파일명이 대문자 규칙이라면 아래 유지, 전부 소문자 규칙이면 return heroine;
        if (heroine == "jin") return "Jin";
        if (heroine == "freyja") return "Freyja";
        return "Ru";
    }

    string PickFromSequence(string heroine, int occ)
    {
        List<string> seq = heroine == "jin"    ? jinCsvSequence
                         : heroine == "freyja" ? freyjaCsvSequence
                         :                       ruCsvSequence;
        if (seq == null || seq.Count == 0) return null;
        int idx = Mathf.Abs(occ) % seq.Count;
        return seq[idx]?.Trim();
    }

    /// <summary>
    /// fileNameNoExt가 (1) 경로포함이거나 (2) 경로없음인 두 경우 모두에서
    /// 아래 순서로 실제 존재하는 TextAsset을 찾음.
    ///   A. scenario/<file>
    ///   B. scenario/<scene>/<file>  (경로없을 때만)
    /// </summary>
    bool TryResolveExisting(string scene, string fileNameNoExt, out string resolved)
    {
        resolved = null;
        if (string.IsNullOrWhiteSpace(fileNameNoExt)) return false;

        string tryA = $"scenario/{fileNameNoExt}";
        if (Resources.Load<TextAsset>(tryA) != null) { resolved = fileNameNoExt; return true; }

        if (!fileNameNoExt.Contains("/"))
        {
            string tryB = $"scenario/{scene}/{fileNameNoExt}";
            if (Resources.Load<TextAsset>(tryB) != null) { resolved = $"{scene}/{fileNameNoExt}"; return true; }
        }
        return false;
    }
}
