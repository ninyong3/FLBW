using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DBManager_DialogOnly : MonoBehaviour
{
    [Header("히로인별 CSV (Resources/scenario, 확장자 제외)")]
    public string csvForJin;    // ex) hospital_jin_message
    public string csvForFreyja; // ex) hospital_freyja_message
    public string csvForRu;     // ex) hospital_ru_message

    [Tooltip("Awake에서 Registry를 조회해 즉시 로드할지 여부(버블 없으면 미로드)")]
    public bool loadOnAwake = false;  // 기본은 꺼두는 걸 권장

    public bool IsReady { get; private set; }
    public readonly Dictionary<int, Dialogue> Data = new Dictionary<int, Dialogue>();

    DialogueParser_DialogOnly parser;

    void Awake()
    {
        // parser는 여기서만 보장하지 말고, Load()에서 게으른 초기화도 함
        parser = GetComponent<DialogueParser_DialogOnly>() ?? gameObject.AddComponent<DialogueParser_DialogOnly>();

        if (loadOnAwake) TryLoadFromRegistry();
    }

    public void TryLoadFromRegistry()
    {
        string scene = SceneManager.GetActiveScene().name;
        var reg = SimpleBubbleRegistry.Instance;

        if (reg != null && reg.TryGetBubbleIdByScene(scene, out var bubble))
        {
            string file = MapBubbleToFile(bubble);
            if (!string.IsNullOrEmpty(file)) Load(file);
            else Debug.LogWarning($"[DB_DialogOnly] bubble='{bubble}' 매핑 실패 → 미로드");
        }
        else
        {
            Debug.LogWarning("[DB_DialogOnly] 버블 없음 → 미로드");
        }
    }

    public void Load(string fileNameNoExt)
    {
        if (string.IsNullOrEmpty(fileNameNoExt))
        {
            Debug.LogError("[DB_DialogOnly] fileNameNoExt is null or empty");
            return;
        }

        // 게으른 초기화(부트스트랩이 너무 빨리 호출돼도 안전)
        if (parser == null)
            parser = GetComponent<DialogueParser_DialogOnly>() ?? gameObject.AddComponent<DialogueParser_DialogOnly>();

        IsReady = false;
        Data.Clear();

        var arr = parser.Parse(fileNameNoExt);
        for (int i = 0; i < arr.Length; i++) Data[i + 1] = arr[i];

        IsReady = true;
        Debug.Log($"[DB_DialogOnly:{name}] Loaded {arr.Length} lines from scenario/{fileNameNoExt}.csv");
    }

    // bubble 값 → 파일명. bubble이 파일명 자체면 그대로 사용.
    string MapBubbleToFile(string bubble)
    {
        if (string.IsNullOrWhiteSpace(bubble)) return null;

        string b = bubble.Trim().ToLowerInvariant();

        // 파일명 자체가 들어오는 경우 (ex) "hospital_ru_message")
        if (Resources.Load<TextAsset>($"scenario/{b}") != null)
            return b;

        // 느슨한 매칭: ru / freyja / jin
        if (b.Contains("ru"))      return csvForRu;
        if (b.Contains("freyja"))  return csvForFreyja;
        if (b.Contains("jin"))     return csvForJin;

        // 엄격 포맷도 지원
        switch (b)
        {
            case "bubble_ru":     return csvForRu;
            case "bubble_freyja": return csvForFreyja;
            case "bubble_jin":    return csvForJin;
        }

        return null;
    }
}
