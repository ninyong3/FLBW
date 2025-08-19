using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;

/// 선택 히로인 대상 버튼만 미니게임 라우팅 + 그 자리 배정 제외 + 1회 방문 후 비활성
[DefaultExecutionOrder(10000)] // MapBubblePlacer보다 늦게
public class MinigameRouter : MonoBehaviour
{
    [Header("Buttons (drag & drop)")]
    [SerializeField] private Button hospitalButton;     // Jin
    [SerializeField] private Button concertHallButton;  // Freyja
    [SerializeField] private Button convenienceButton;  // Ru

    [Header("Keys (비우면 버튼 이름으로 자동)")]
    [SerializeField] private string hospitalKey;        // "hospital"
    [SerializeField] private string concertHallKey;     // "concert_hall"
    [SerializeField] private string convenienceKey;     // "convenience" or "convenience_store"

    [Header("Scenes (로드될 씬명)")]
    [SerializeField] private string minigameJinScene    = "minigame_Jin";
    [SerializeField] private string minigameFreyjaScene = "minigame_Freyja";
    [SerializeField] private string minigameRuScene     = "minigame_Ru";

    [Header("Wiring")]
    [Tooltip("MapBubblePlacer 재바인딩이 끝난 뒤 우리가 덮어쓸 대기 프레임 수")]
    [SerializeField] private int rewireAfterFrames = 6;

    [Header("Outline Highlight (optional)")]
    [SerializeField] private bool  enableOutlineHighlight = true;
    [SerializeField] private Color highlightColor = new Color(0.25f, 0.6f, 1f, 1f);
    [SerializeField] private bool  onlyWhenInteractable   = true;

    void OnEnable() => StartCoroutine(SetupAfterMBP());

    IEnumerator SetupAfterMBP()
    {
        for (int i = 0; i < Mathf.Max(0, rewireAfterFrames); i++)
            yield return null;

        // 키 자동 세팅(비어있을 때만)
        if (string.IsNullOrWhiteSpace(hospitalKey)    && hospitalButton)    hospitalKey    = Slug(hospitalButton.name);
        if (string.IsNullOrWhiteSpace(concertHallKey) && concertHallButton) concertHallKey = Slug(concertHallButton.name);
        if (string.IsNullOrWhiteSpace(convenienceKey) && convenienceButton) convenienceKey = Slug(convenienceButton.name);

        // 오늘 배정에서 대상 장소 제거 후 재배치
        SanitizeTodayMapExcludeActiveLocation();

        // 대상 버튼만 onClick 오버라이드
        WireOnlyTargetButton();

        // 1회 방문 후 비활성 반영
        RefreshTargetButtonState();

        // 파란 아웃라인(선택)
        if (enableOutlineHighlight) HighlightTargetOutline();
    }

    // --- todayMap에서 대상 키 제거 후 재배치 ---
    void SanitizeTodayMapExcludeActiveLocation()
    {
        var reg = SimpleBubbleRegistry.Instance; if (reg == null) return;

        int heroine = (GameManager.instance != null) ? GameManager.instance.selectedHeroine : 0;
        string targetKey = GetTargetKeyForHeroine(heroine);
        if (string.IsNullOrEmpty(targetKey)) return;

        var snap = reg.GetTodaySnapshot(); // sceneKey -> bubbleId
        if (snap != null && snap.Remove(Normalize(targetKey)))
        {
            reg.ReplaceAll(snap);              // 레지스트리에 수정 반영
            var mbp = FindObjectOfType<MapBubblePlacer>();
            mbp?.ApplyToday();                 // 맵 버튼들 재적용
        }
    }

    // --- 대상 버튼만 우리 핸들러로 교체 ---
    void WireOnlyTargetButton()
    {
        var (btn, key) = GetTargetButtonAndKey();
        if (!btn || string.IsNullOrEmpty(key)) return;

        // 기존 리스너 초기화(퍼시스턴트 포함)
        if (btn.onClick != null && btn.onClick.GetPersistentEventCount() > 0)
            btn.onClick = new Button.ButtonClickedEvent();
        else
            btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() => StartCoroutine(OnClickTargetRoutine(key)));
    }

    void RefreshTargetButtonState()
    {
        var (btn, _) = GetTargetButtonAndKey(); if (!btn) return;
        var reg = SimpleBubbleRegistry.Instance;
        bool disable = (reg != null && reg.GetTotalVisitsToday() > 0); // 오늘 1회 방문 후 차단(레스토랑 룰)
        btn.interactable = !disable;
    }

    IEnumerator OnClickTargetRoutine(string key)
    {
        var reg = SimpleBubbleRegistry.Instance; if (reg == null) yield break;

        // 오늘 1회 방문 후 차단
        if (reg.GetTotalVisitsToday() > 0) yield break;

        // 일반 버블 재입장 차단도 존중
        if (reg.TryGetBubbleIdByScene(key, out var _) && !reg.CanEnterToday(key))
            yield break;

        // 히로인별 미니게임 씬 결정
        string loadScene = ResolveMinigameSceneForCurrentHeroine();
        if (string.IsNullOrEmpty(loadScene)) yield break;

        var es = EventSystem.current; if (es) es.SetSelectedGameObject(null);
        yield return null;

        reg.MarkEntered(key); // 방문 1회 기록
        SceneManager.LoadScene(loadScene);
    }

    // ---------- Outline 파란색 하이라이트 ----------
    void HighlightTargetOutline()
    {
        var (btn, key) = GetTargetButtonAndKey();
        if (!btn || string.IsNullOrEmpty(key)) return;
        if (onlyWhenInteractable && !btn.interactable) return;

        var outlineGO = FindOutlineGOForButton(btn, key);
        if (outlineGO == null) return;

        var outline = outlineGO.GetComponent<Outline>();
        if (outline != null) { outline.effectColor = highlightColor; return; }

        var img = outlineGO.GetComponent<Image>();
        if (img != null) { img.color = highlightColor; }
    }

    // "<ButtonName>_outline" 또는 "<sceneKey>_outline" 을 찾아서 반환
    GameObject FindOutlineGOForButton(Button btn, string sceneKey)
    {
        if (btn == null) return null;

        string want1 = (btn.name + "_outline").Trim().ToLowerInvariant();
        string want2 = (sceneKey ?? "").Trim().ToLowerInvariant() + "_outline";

        GameObject r = TryFindByNameUpwards(btn.transform, want1);
        if (r != null) return r;

        r = TryFindByNameUpwards(btn.transform, want2);
        return r; // 못 찾으면 null 반환
    }

    // 자기 트랜스폼에서 부모 방향으로 올라가며, 각 단계에서 재귀적으로 자식들 탐색
    GameObject TryFindByNameUpwards(Transform start, string nameLower)
    {
        Transform p = start;
        while (p != null)
        {
            var found = FindChildByNameRecursive(p, nameLower);
            if (found != null) return found;
            p = p.parent;
        }
        return null;
    }

    GameObject FindChildByNameRecursive(Transform root, string nameLower)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var t = root.GetChild(i);
            if (t.name.Trim().ToLowerInvariant() == nameLower) return t.gameObject;
            var sub = FindChildByNameRecursive(t, nameLower);
            if (sub != null) return sub;
        }
        return null;
    }

    // ---------- 헬퍼 ----------
    (Button btn, string key) GetTargetButtonAndKey()
    {
        int heroine = (GameManager.instance != null) ? GameManager.instance.selectedHeroine : 0;
        switch (heroine)
        {
            case 1: return (hospitalButton,    hospitalKey);    // Jin
            case 2: return (concertHallButton, concertHallKey); // Freyja
            case 3: return (convenienceButton, convenienceKey); // Ru
        }
        return (null, null);
    }

    string GetTargetKeyForHeroine(int heroine)
    {
        switch (heroine)
        {
            case 1: return hospitalKey;
            case 2: return concertHallKey;
            case 3: return convenienceKey;
        }
        return null;
    }

    string ResolveMinigameSceneForCurrentHeroine()
    {
        int heroine = (GameManager.instance != null) ? GameManager.instance.selectedHeroine : 0;
        switch (heroine)
        {
            case 1: return minigameJinScene;
            case 2: return minigameFreyjaScene;
            case 3: return minigameRuScene;
        }
        return null;
    }

    static string Normalize(string s) => (s ?? "").Trim().ToLowerInvariant();

    static string Slug(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        s = s.Trim().ToLowerInvariant();
        var arr = s.Select(ch => (char.IsLetterOrDigit(ch) ? ch : '_')).ToArray();
        var t = new string(arr);
        while (t.Contains("__")) t = t.Replace("__", "_");
        return t.Trim('_');
    }
}
