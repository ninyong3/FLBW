using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AllowedPair
{
    public string bubbleId;  // "bubble_Ru" / "bubble_Freyja" / "bubble_Jin"
    public string sceneKey;  // "beach" / "hospital" / ...
}

public class SimpleBubbleRegistry : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static SimpleBubbleRegistry Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        RebuildAllowedMap();
    }

    void OnValidate()
    {
        // 인스펙터에서 AllowedPairs 수정 시 즉시 반영
        RebuildAllowedMap();
    }

    // ── 인스펙터 설정 ────────────────────────────────────────────────────────
    [Header("허용 매핑 (히로인 버블ID -> 가능한 장소 키)")]
    public List<AllowedPair> allowedPairs = new List<AllowedPair>();

    // 내부 테이블
    readonly Dictionary<string, List<string>> allowedMap =
        new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

    // 오늘자 배정 결과: sceneKey -> bubbleId
    readonly Dictionary<string, string> todayMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    static string NormalizeKey(string s) => (s ?? "").Trim().ToLowerInvariant();

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

    // (디버깅용) 마지막 BeginDay 호출 기록
    public static string LastCallerTag  { get; private set; }
    public static int    LastDay        { get; private set; }
    public static string LastCurrent    { get; private set; }
    public static string LastStackTrace { get; private set; }

    // 기존 시그니처 유지
    public void BeginDay(int day, string currentHeroineBubbleId)
    {
        Debug.Log($"[SBR] BeginDay(day={day}, current={currentHeroineBubbleId}, caller={new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType?.Name}.{new System.Diagnostics.StackFrame(1).GetMethod().Name}())");

        BeginDay(day, currentHeroineBubbleId, "SimpleBubbleRegistry.BeginDay()");
    }

    // 기존 시그니처는 그대로 두고, 오버로드만 추가(호출자 태그 기록)
    public void BeginDay(int day, string currentHeroineBubbleId, string callerTag)
    {
        // 호출 추적 저장
        LastCallerTag  = callerTag;
        LastDay        = day;
        LastCurrent    = currentHeroineBubbleId;
        LastStackTrace = UnityEngine.StackTraceUtility.ExtractStackTrace();

        Debug.Log($"[SBR] BeginDay(day={day}, current={currentHeroineBubbleId}, caller={callerTag})");

        todayMap.Clear();

        // 1) 플레이 중인 히로인 제외
        var exclude = NormalizeKey(currentHeroineBubbleId);
        var heroines = allowedMap.Keys
            .Where(h => !h.Equals(exclude, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (heroines.Count == 0)
        {
            Debug.LogWarning("[SBR] 배정 대상 히로인이 없습니다.");
            return;
        }

        // 2) 재현 가능한 랜덤: 같은 day면 같은 결과
        var rng        = new System.Random(day);
        var usedScenes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 3) 각 히로인에게 장소 1개씩 배정 (가능하면 서로 다른 장소)
        foreach (var heroine in heroines)
        {
            if (!allowedMap.TryGetValue(heroine, out var candidates) || candidates.Count == 0)
            {
                Debug.LogWarning($"[SBR] '{heroine}' 후보 장소 없음");
                continue;
            }

            var fresh  = candidates.Where(s => !usedScenes.Contains(s)).ToList();
            var chosen = (fresh.Count > 0)
                ? fresh[rng.Next(fresh.Count)]
                : candidates[rng.Next(candidates.Count)]; // 후보 다 썼으면 중복 허용

            usedScenes.Add(chosen);
            todayMap[NormalizeKey(chosen)] = NormalizeKey(heroine); // sceneKey -> bubbleId
        }

        var pairs = string.Join(", ", todayMap.Select(kv => $"{kv.Key}->{kv.Value}"));
        Debug.Log($"[SBR] Day {day} 배정 완료: {pairs}");
    }

    // ── 조회/유틸 ─────────────────────────────────────────────────────────────
    public bool TryGetBubbleIdByScene(string sceneKey, out string bubbleId)
        => todayMap.TryGetValue(NormalizeKey(sceneKey), out bubbleId);

    public Dictionary<string, string> GetTodaySnapshot()
        => new Dictionary<string, string>(todayMap);

    public void ReplaceAll(IDictionary<string, string> snapshot)
    {
        todayMap.Clear();
        if (snapshot == null) return;
        foreach (var kv in snapshot)
            todayMap[NormalizeKey(kv.Key)] = NormalizeKey(kv.Value);
    }

    public void ClearToday() => todayMap.Clear();
}
