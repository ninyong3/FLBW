using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬 간 유지되는 버블 배정/방문 기록 레지스트리.
/// - 허용 매핑(AllowedPairs) → Day별 라운드로빈 배정
/// - 오늘 방문 기록(씬별 1회 제한), 레스토랑 입장 차단 근거 제공
/// - 같은 Day로 BeginDay가 중복 호출되면 무시(방문 기록 보존)
/// </summary>
[Serializable]
public class AllowedPair
{
    public string bubbleId;  // "bubble_Ru" / "bubble_Freyja" / "bubble_Jin"
    public string sceneKey;  // "beach" / "hospital" / ...
}

[DefaultExecutionOrder(-1000)]
public class SimpleBubbleRegistry : MonoBehaviour
{
    public static SimpleBubbleRegistry Instance { get; private set; }

    [Header("허용 매핑 (히로인 버블ID -> 가능한 장소 키)")]
    public List<AllowedPair> allowedPairs = new List<AllowedPair>();

    // allowedPairs 정규화: bubbleId → [sceneKey...]
    readonly Dictionary<string, List<string>> allowedMap =
        new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

    // 오늘자 배정: sceneKey → bubbleId
    readonly Dictionary<string, string> todayMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    // 오늘 방문 기록: sceneKey → count
    readonly Dictionary<string, int> _visitsToday =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    // Day 중복 초기화 방지
    int _currentDay = int.MinValue;

    // 최근 BeginDay 호출 추적(디버깅용)
    public static string LastCallerTag { get; private set; }
    public static int    LastDay       { get; private set; }
    public static string LastCurrent   { get; private set; }
    public static string LastStackTrace{ get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"[REG] Duplicate destroyed: {name} (keep {Instance.name})");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        RebuildAllowedMap();
        Debug.Log($"[REG] Awake id={GetInstanceID()} scene='{gameObject.scene.name}' (persist)");
    }

    void OnValidate() => RebuildAllowedMap();

    // ---------- Public API ----------

    /// <summary>Day 시작 시 방문기록/시퀀스 초기화.</summary>
    public void ResetVisitsForNewDay()
    {
        _visitsToday.Clear();
        SceneDialogueFromSimple.ResetSequenceForDay();
        Debug.Log("[REG] ResetVisitsForNewDay()");
    }

    /// <summary>기존 시그니처 유지용.</summary>
    public void BeginDay(int day, string currentHeroineBubbleId)
        => BeginDay(day, currentHeroineBubbleId, "SimpleBubbleRegistry.BeginDay()");

    /// <summary>
    /// Day 시작: 현재 히로인 제외하고 라운드로빈 배정.
    /// 같은 Day로 중복 호출되면 방문 기록을 보존하고 초기화/재배정을 무시한다.
    /// </summary>
    public void BeginDay(int day, string currentHeroineBubbleId, string callerTag)
    {
        // 같은 Day로 또 들어오면 무시(방문 기록 보존)
        if (_currentDay == day)
        {
            Debug.Log($"[REG] BeginDay ignored (same day). caller='{callerTag}', day={day}");
            return;
        }
        _currentDay   = day;
        LastCallerTag = callerTag;
        LastDay       = day;
        LastCurrent   = currentHeroineBubbleId;
        LastStackTrace= UnityEngine.StackTraceUtility.ExtractStackTrace();

        todayMap.Clear();

        var exclude  = NormalizeKey(currentHeroineBubbleId);
        var heroines = allowedMap.Keys
            .Where(h => !string.Equals(h, exclude, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)   // ← 이건 그대로 OK (LINQ Contains/Distinct는 Comparer 사용)
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

        _visitsToday.Clear(); // Day가 변경된 경우에만 초기화
        Debug.Log($"[REG] BeginDay day={day}, pairs={string.Join(", ", todayMap.Select(kv=>$"{kv.Key}->{kv.Value}"))}");
    }

    /// <summary>오늘 이 씬에 입장 가능한지(기본 1회 제한).</summary>
    public bool CanEnterToday(string sceneKey)
    {
        var k = NormalizeKey(sceneKey);
        int c = _visitsToday.TryGetValue(k, out var v) ? v : 0;
        return c < 1;
    }

    /// <summary>입장 성공 시 호출하여 방문 횟수 기록.</summary>
    public void MarkEntered(string sceneKey)
    {
        var k = NormalizeKey(sceneKey);
        _visitsToday[k] = _visitsToday.TryGetValue(k, out var v) ? v + 1 : 1;
        Debug.Log($"[REG] MarkEntered('{k}') -> count={_visitsToday[k]} total={GetTotalVisitsToday()}");
    }

    /// <summary>오늘 아무 맵이라도 1회 이상 입장했는지.</summary>
    public bool HasEnteredAnyMapToday() => GetTotalVisitsToday() > 0;

    /// <summary>오늘자 배정 조회: sceneKey → bubbleId.</summary>
    public bool TryGetBubbleIdByScene(string sceneKey, out string bubbleId)
        => todayMap.TryGetValue(NormalizeKey(sceneKey), out bubbleId);

    /// <summary>오늘자 배정 스냅샷.</summary>
    public Dictionary<string, string> GetTodaySnapshot()
        => new Dictionary<string, string>(todayMap);

    /// <summary>스냅샷으로 오늘자 배정 대체.</summary>
    public void ReplaceAll(IDictionary<string, string> snapshot)
    {
        todayMap.Clear();
        if (snapshot == null) return;
        foreach (var kv in snapshot)
            todayMap[NormalizeKey(kv.Key)] = NormalizeKey(kv.Value);
    }

    /// <summary>오늘자 배정 초기화.</summary>
    public void ClearToday() => todayMap.Clear();

    /// <summary>특정 씬의 오늘 방문 횟수(디버깅용).</summary>
    public int GetVisitCount(string sceneKey)
    {
        var k = NormalizeKey(sceneKey);
        return _visitsToday.TryGetValue(k, out var v) ? v : 0;
    }

    /// <summary>오늘의 총 방문 횟수(디버깅용).</summary>
    public int GetTotalVisitsToday()
    {
        int sum = 0; foreach (var kv in _visitsToday) sum += kv.Value; return sum;
    }

    /// <summary>현재 방문 기록 덤프(디버깅용).</summary>
    public static void DebugDump()
    {
        var r = Instance;
        if (r == null) { Debug.Log("[REG] Dump: Instance=null"); return; }
        var sb = new System.Text.StringBuilder();
        foreach (var kv in r._visitsToday) sb.Append($"{kv.Key}:{kv.Value} ");
        Debug.Log($"[REG] VisitsToday = {sb}");
    }

    // ---------- Internal ----------

    static string NormalizeKey(string s) => (s ?? "").Trim().ToLowerInvariant();

    // 라운드로빈 선택(중복 장소 피함)
    string PickSceneRoundRobin(int day, List<string> candidates, HashSet<string> usedScenes, string heroineKey)
    {
        if (candidates == null || candidates.Count == 0) return null;

        int phase = Mathf.Abs((heroineKey ?? "").GetHashCode()) % candidates.Count;
        int idx   = ((day - 1) + phase) % candidates.Count;

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
}
