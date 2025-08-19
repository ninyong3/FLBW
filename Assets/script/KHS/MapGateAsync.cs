using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapGateAsync : MonoBehaviour
{
    public static MapGateAsync Instance { get; private set; }
    public string mapSceneName = "Map";

    [Tooltip("현재 선택된 히로인 버블ID(읽기 참조용)")]
    public string currentHeroineBubbleId = BubbleIds.Ru; // 초기값

    public bool verbose = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (verbose)
                Debug.Log($"[GATE] Duplicate. keep existing='{Instance.currentHeroineBubbleId}', drop this='{currentHeroineBubbleId}'");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (verbose) Debug.Log($"[GATE] Awake current={currentHeroineBubbleId}");
    }

    public void OpenMapAsync() => StartCoroutine(Load());

    IEnumerator Load()
    {
        // Day 계산
        int day = 1;
        var dm = FindFirstObjectByType<DayManager>();
        if (dm != null) day = Mathf.Max(1, dm.currentDay);
        else if (GameManager.instance != null) day = Mathf.Max(1, GameManager.instance.dayCount);

        // 전역 선택 인덱스 읽기
        int heroineIndex = (GameManager.instance != null) ? GameManager.instance.selectedHeroine : 0;
        if (verbose) Debug.Log($"[GATE] selectedHeroine idx={heroineIndex}");

        // 인덱스 → 버블ID (소문자 표준화)
        currentHeroineBubbleId = BubbleIds.Normalize(MapSelectedHeroineToBubbleId(heroineIndex));
        if (dm != null) dm.currentHeroineBubbleId = currentHeroineBubbleId;

        if (verbose) Debug.Log($"[GATE] BeginDay: day={day}, current={currentHeroineBubbleId}");

        // 오늘 배치 생성(여기서만 호출)
        var reg = SimpleBubbleRegistry.Instance;
        if (reg != null)
        {
            reg.BeginDay(day, currentHeroineBubbleId, "MapGateAsync.Load()");
            if (verbose)
            {
                var snap = reg.GetTodaySnapshot();
                var list = new System.Text.StringBuilder();
                foreach (var kv in snap) list.Append($"[{kv.Key},{kv.Value}] ");
                Debug.Log($"[MBP] snapshot = {list}");
            }
        }
        else
        {
            Debug.LogWarning("[GATE] SimpleBubbleRegistry.Instance = null");
        }

        // 맵 로드
        if (Application.CanStreamedLevelBeLoaded(mapSceneName))
        {
            Debug.Log($"[GATE] LoadScene '{mapSceneName}' with current={currentHeroineBubbleId}");
            var op = SceneManager.LoadSceneAsync(mapSceneName);
            while (!op.isDone) yield return null;
        }
        else
        {
            Debug.LogWarning($"[GATE] Scene not found: '{mapSceneName}'");
        }
    }

    static string MapSelectedHeroineToBubbleId(int heroineIndex)
    {
        switch (heroineIndex)
        {
            case 1: return BubbleIds.Jin;
            case 2: return BubbleIds.Freyja;
            case 3: return BubbleIds.Ru;
            default: return BubbleIds.Ru;
        }
    }
}
