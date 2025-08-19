using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class AllowedPair
{
    public string bubbleId;
    public string sceneKey;
}

[DefaultExecutionOrder(-1000)]
public class SimpleBubbleRegistry : MonoBehaviour
{
    public static SimpleBubbleRegistry Instance { get; private set; }

    [Header("허용 매핑 (히로인 버블ID -> 가능한 장소 키)")]
    public List<AllowedPair> allowedPairs = new List<AllowedPair>();

    // ★ 중복 발생 시 파괴 대신 프록시로 동작하게 할지
    [Header("Duplicate Handling")]
    public bool actAsProxyWhenDuplicate = true;
    bool proxyRuntime;

    readonly Dictionary<string, List<string>> allowedMap =
        new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

    readonly Dictionary<string, string> todayMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    readonly Dictionary<string, int> visitsToday =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    int currentDay = int.MinValue;

    public static string LastCallerTag  { get; private set; }
    public static int    LastDay        { get; private set; }
    public static string LastCurrent    { get; private set; }
    public static string LastStackTrace { get; private set; }

    // 부트스트랩: 프리팹 있으면 생성, 없으면 씬 인스턴스 기다림
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureInstance()
    {
        if (Instance != null) return;

        var prefab = Resources.Load<SimpleBubbleRegistry>("Registry/SimpleBubbleRegistry");
        if (prefab != null)
        {
            var go = Instantiate(prefab);
            Instance = go.GetComponent<SimpleBubbleRegistry>();
            DontDestroyOnLoad(go);
            Instance.RebuildAllowedMap();
            Debug.Log("[REG] Bootstrap from prefab (persist)");
        }
        else
        {
            Debug.Log("[REG] No prefab found. Waiting for scene instance.");
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (actAsProxyWhenDuplicate)
            {
                // ★ 파괴하지 않고 프록시로 전환 (에러 안 찍음)
                proxyRuntime = true;                  // 이 컴포넌트는 호출을 Instance로 포워딩
                // DontDestroyOnLoad 호출하지 않음(씬과 함께 사라지게)
                Debug.Log("[REG] Duplicate acting as proxy (keep " + Instance.name + ")");
                return;
            }

            Debug.LogWarning("[REG] Duplicate destroyed: SimpleBubbleRegistry (keep " + Instance.name + ")");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        RebuildAllowedMap();

        Debug.Log($"[REG] Awake id={GetInstanceID()} scene='{gameObject.scene.name}' (persist)");
    }

    void OnValidate() => RebuildAllowedMap();

    // ======== Public API (필요 시 프록시 포워딩) ========

    public void ResetVisitsForNewDay()
    {
        if (proxyRuntime && Instance != null && Instance != this) { Instance.ResetVisitsForNewDay(); return; }
        visitsToday.Clear();
        Debug.Log("[REG] ResetVisitsForNewDay()");
    }

    public void BeginDay(int day, string currentHeroineBubbleId)
        => BeginDay(day, currentHeroineBubbleId, "SimpleBubbleRegistry.BeginDay()");

    public void BeginDay(int day, string currentHeroineBubbleId, string callerTag)
    {
        if (proxyRuntime && Instance != null && Instance != this) { Instance.BeginDay(day, currentHeroineBubbleId, callerTag); return; }

        bool sameDay = (currentDay == day);

        // 같은 Day라도 todayMap이 비어 있으면 재배정 허용(로드/타이틀 복귀 보정)
        if (sameDay && todayMap.Count > 0)
        {
            Debug.Log($"[REG] BeginDay ignored (same day and already assigned). caller='{callerTag}', day={day}");
            return;
        }

        currentDay = day;

        LastCallerTag = callerTag;
        LastDay = day;
        LastCurrent = currentHeroineBubbleId;
        LastStackTrace = UnityEngine.StackTraceUtility.ExtractStackTrace();

        todayMap.Clear();

        var exclude = NormalizeKey(currentHeroineBubbleId);
        var heroines = allowedMap.Keys
            .Where(h => !string.Equals(h, exclude, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var usedScenes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var heroine in heroines)
        {
            if (!allowedMap.TryGetValue(heroine, out var candidates) || candidates.Count == 0)
                continue;

            var chosen = PickSceneRoundRobin(day, candidates, usedScenes, heroine);
            if (string.IsNullOrEmpty(chosen)) continue;

            usedScenes.Add(chosen);
            todayMap[NormalizeKey(chosen)] = NormalizeKey(heroine); // sceneKey -> bubbleId
        }

        visitsToday.Clear();
        Debug.Log($"[REG] BeginDay day={day}, pairs={string.Join(", ", todayMap.Select(kv => $"{kv.Key}->{kv.Value}"))}");
    }

    public bool CanEnterToday(string sceneKey)
    {
        if (proxyRuntime && Instance != null && Instance != this) return Instance.CanEnterToday(sceneKey);
        var k = NormalizeKey(sceneKey);
        int c = visitsToday.TryGetValue(k, out var v) ? v : 0;
        return c < 1;
    }

    public void MarkEntered(string sceneKey)
    {
        if (proxyRuntime && Instance != null && Instance != this) { Instance.MarkEntered(sceneKey); return; }
        var k = NormalizeKey(sceneKey);
        visitsToday[k] = visitsToday.TryGetValue(k, out var v) ? v + 1 : 1;
        Debug.Log($"[REG] MarkEntered('{k}') -> count={visitsToday[k]} total={GetTotalVisitsToday()}");
    }

    public bool HasEnteredAnyMapToday()
    {
        if (proxyRuntime && Instance != null && Instance != this) return Instance.HasEnteredAnyMapToday();
        return GetTotalVisitsToday() > 0;
    }

    public bool TryGetBubbleIdByScene(string sceneKey, out string bubbleId)
    {
        if (proxyRuntime && Instance != null && Instance != this) return Instance.TryGetBubbleIdByScene(sceneKey, out bubbleId);
        return todayMap.TryGetValue(NormalizeKey(sceneKey), out bubbleId);
    }

    public Dictionary<string, string> GetTodaySnapshot()
    {
        if (proxyRuntime && Instance != null && Instance != this) return Instance.GetTodaySnapshot();
        return new Dictionary<string, string>(todayMap);
    }

    public void ReplaceAll(IDictionary<string, string> snapshot)
    {
        if (proxyRuntime && Instance != null && Instance != this) { Instance.ReplaceAll(snapshot); return; }
        todayMap.Clear();
        if (snapshot == null) return;
        foreach (var kv in snapshot)
            todayMap[NormalizeKey(kv.Key)] = NormalizeKey(kv.Value);
    }

    public void ClearToday()
    {
        if (proxyRuntime && Instance != null && Instance != this) { Instance.ClearToday(); return; }
        todayMap.Clear();
    }

    public int GetVisitCount(string sceneKey)
    {
        if (proxyRuntime && Instance != null && Instance != this) return Instance.GetVisitCount(sceneKey);
        var k = NormalizeKey(sceneKey);
        return visitsToday.TryGetValue(k, out var v) ? v : 0;
    }

    public int GetTotalVisitsToday()
    {
        if (proxyRuntime && Instance != null && Instance != this) return Instance.GetTotalVisitsToday();
        int sum = 0; foreach (var kv in visitsToday) sum += kv.Value; return sum;
    }

    public static void DebugDump()
    {
        var r = Instance;
        if (r == null) { Debug.Log("[REG] Dump: Instance=null"); return; }
        var list = r.visitsToday.Select(kv => $"{kv.Key}:{kv.Value}");
        Debug.Log("[REG] VisitsToday = " + string.Join(" ", list));
    }

    // ======== Internal ========

    static string NormalizeKey(string s) => (s ?? "").Trim().ToLowerInvariant();

    string PickSceneRoundRobin(int day, List<string> candidates, HashSet<string> usedScenes, string heroineKey)
    {
        if (candidates == null || candidates.Count == 0) return null;

        int phase = Mathf.Abs((heroineKey ?? "").GetHashCode()) % candidates.Count;
        int idx = ((day - 1) + phase) % candidates.Count;

        for (int step = 0; step < candidates.Count; step++)
        {
            int i = (idx + step) % candidates.Count;
            string s = candidates[i];
            if (!usedScenes.Contains(s)) return s;
        }
        return candidates[idx];
    }

    void RebuildAllowedMap()
    {
        allowedMap.Clear();
        foreach (var p in allowedPairs)
        {
            var h = NormalizeKey(p.bubbleId);
            var s = NormalizeKey(p.sceneKey);
            if (string.IsNullOrEmpty(h) || string.IsNullOrEmpty(s)) continue;

            if (!allowedMap.TryGetValue(h, out var list))
            {
                list = new List<string>();
                allowedMap[h] = list;
            }
            if (!list.Contains(s, StringComparer.OrdinalIgnoreCase))
                list.Add(s);
        }
    }

    // (선택) 타이틀 복귀용 — 원하면 타이틀로 가기 직전에 한 줄 호출
    public void ResetForTitle()
    {
        if (proxyRuntime && Instance != null && Instance != this) { Instance.ResetForTitle(); return; }
        todayMap.Clear();
        visitsToday.Clear();
        currentDay = int.MinValue;
        Debug.Log("[REG] ResetForTitle: cleared todayMap/visits and reset currentDay");
    }
}
