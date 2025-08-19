using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 맵 화면에서 버블을 배치하고, 장소 버튼의 진입 제어(1일 1회 제한, 레스토랑 라우팅, outline 색상)를 수행.
/// 흐름:
/// 1) OnEnable → ApplyToday()로 오늘자 배정 배치
/// 2) WireButtonsOnce()로 버튼 onClick을 현재 핸들러로 교체(퍼시스턴트 포함 초기화)
/// 3) RefreshAllButtonsOnce()로 인터랙션/outline 갱신
/// 4) LateWireAndRefresh(), KeepRefreshForFrames()로 외부 스크립트의 덮어쓰기 방지
/// </summary>
[Serializable]
public struct ButtonRef
{
    public string sceneKey;      // 이 버튼이 의미하는 씬 키
    public RectTransform rect;   // 버튼 RectTransform
}

[DefaultExecutionOrder(200)]
public class MapBubblePlacer : MonoBehaviour
{
    [Header("버블 RectTransform")]
    public RectTransform bubble_Ru;
    public RectTransform bubble_Freyja;
    public RectTransform bubble_Jin;

    [Header("버튼(장소) RectTransform 목록")]
    public List<ButtonRef> buttons = new List<ButtonRef>();

    [Header("배치 오프셋")]
    public Vector2 baseOffset;
    public Vector2 offsetRu;
    public Vector2 offsetFreyja;
    public Vector2 offsetJin;

    [Tooltip("버튼 목록에 없을 경우 Hierarchy 이름으로 탐색")]
    public bool fallbackFindByName = true;

    [Header("키 설정")]
    [Tooltip("레스토랑 버튼/씬 키")]
    public string restaurantKey = "restaurant";

    [Header("예외 키")]
    [Tooltip("메인 버튼/씬 키(방문 카운트 및 차단 로직에서 제외)")]
    public string mainKey = "main";

    [Header("Outline 색상")]
    [Tooltip("차단 시 outline에 적용할 색상")]
    public Color outlineDisabled = new Color(0.6f, 0.6f, 0.6f, 0.8f);

    [Header("씬 라우팅 오버라이드")]
    [Tooltip("레스토랑 버튼 클릭 시 실제 로드할 씬 이름")]
    public string restaurantLoadsScene = "minigame_main";

    // 버튼 원래 스타일(Transition/ColorBlock) 캐시
    private readonly Dictionary<Button, Selectable.Transition> _origTransition =
        new Dictionary<Button, Selectable.Transition>();
    private readonly Dictionary<Button, ColorBlock> _origColors =
        new Dictionary<Button, ColorBlock>();

    // outline 원래 색상 캐시(버튼별)
    readonly Dictionary<GameObject, Color> _originalOutlineColor = new Dictionary<GameObject, Color>();

    void Awake()
    {
        if (string.IsNullOrEmpty(restaurantKey)) restaurantKey = "restaurant";
        if (string.IsNullOrEmpty(restaurantLoadsScene)) restaurantLoadsScene = "minigame_main";
        if (string.IsNullOrEmpty(mainKey)) mainKey = "main";
    }

    void OnEnable()
    {
        ApplyToday();
        RefreshAllButtonsOnce();
        StartCoroutine(LateWireAndRefresh());
        StartCoroutine(KeepRefreshForFrames(4)); // 외부 덮어쓰기 방지
        SimpleBubbleRegistry.DebugDump();        // 상태 확인에 유용
    }

    // ---------- 배치 ----------

    void ResetBubbles()
    {
        if (bubble_Ru)     bubble_Ru.gameObject.SetActive(false);
        if (bubble_Freyja) bubble_Freyja.gameObject.SetActive(false);
        if (bubble_Jin)    bubble_Jin.gameObject.SetActive(false);
    }

    public void ApplyToday()
    {
        ResetBubbles();

        var reg = SimpleBubbleRegistry.Instance;
        if (reg == null) { Debug.LogWarning("[MBP] SimpleBubbleRegistry not found"); return; }

        var today = reg.GetTodaySnapshot();
        if (today == null || today.Count == 0)
        {
            Debug.LogWarning("[MBP] Today map is empty (check BeginDay)");
            return;
        }

        foreach (var kv in today)
        {
            var key      = NormalizeKey(kv.Key);
            var bubbleId = NormalizeKey(kv.Value);

            var button = FindButtonRect(key);
            if (button == null) { Debug.LogWarning($"[MBP] Button '{key}' not found"); continue; }

            var bubble = ResolveBubbleRect(bubbleId);
            if (bubble == null) { Debug.LogWarning($"[MBP] Bubble '{bubbleId}' not assigned"); continue; }

            PlaceBubbleAtButton(bubble, button, bubbleId);
        }

        WireButtonsOnce();
        RefreshAllButtonsOnce();
    }

    // ---------- 버튼 와이어링/갱신 ----------

    void CacheBtnStyle(Button btn)
    {
        if (btn == null) return;
        if (!_origTransition.ContainsKey(btn)) _origTransition[btn] = btn.transition;
        if (!_origColors.ContainsKey(btn))     _origColors[btn]     = btn.colors;
    }

    void WireButtonsOnce()
    {
        foreach (var b in buttons)
        {
            if (b.rect == null) continue;
            var btn = b.rect.GetComponent<Button>();
            if (btn == null) continue;

            WireButton(btn, b.sceneKey);
        }
    }

    void WireButton(Button btn, string sceneKey)
    {
        if (btn == null) return;

        // 퍼시스턴트 onClick까지 초기화
        if (btn.onClick != null && btn.onClick.GetPersistentEventCount() > 0)
            btn.onClick = new Button.ButtonClickedEvent();
        else
            btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() => OnClickSceneButton(sceneKey));

        // 원래 스타일 캐시
        CacheBtnStyle(btn);

        RefreshButtonState(btn, sceneKey);
//        Debug.Log($"[MBP] Wire '{btn.name}' key='{sceneKey}' pers={btn.onClick.GetPersistentEventCount()}");
    }

    System.Collections.IEnumerator LateWireAndRefresh()
    {
        yield return null;
        WireButtonsOnce();
        RefreshAllButtonsOnce();
    }

    System.Collections.IEnumerator KeepRefreshForFrames(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
            WireButtonsOnce();
            RefreshAllButtonsOnce();
        }
    }

    void RefreshAllButtonsOnce()
    {
        foreach (var b in buttons)
        {
            if (b.rect == null) continue;
            var btn = b.rect.GetComponent<Button>();
            if (btn != null) RefreshButtonState(btn, b.sceneKey);
        }
    }

    /// <summary>
    /// 버튼 상태 갱신(레스토랑/버블 씬의 입장 제한 반영 + outline 색상).
    /// 상태/이유 로그를 함께 남긴다.
    /// </summary>
    public void RefreshButtonState(Button btn, string sceneKey)
    {
        if (btn == null || !btn) return;

        // Wire 이전 호출 대비
        CacheBtnStyle(btn);

        var reg = SimpleBubbleRegistry.Instance;
        if (reg == null) return;

        var k0 = NormalizeKey(sceneKey);

        // --- main 예외: 항상 활성/원래 스타일/원래 outline 유지 ---
        if (IsMain(k0))
        {
            btn.interactable = true;
            if (_origTransition.TryGetValue(btn, out var t)) btn.transition = t;
            if (_origColors.TryGetValue(btn, out var c))     btn.colors = c;

            var outlineGO = FindOutlineGOForButton(btn, sceneKey);
            if (outlineGO != null && _originalOutlineColor.TryGetValue(outlineGO, out var orig2))
                TrySetOutlineColor(outlineGO, orig2);

//            Debug.Log($"[MBP] Refresh '{sceneKey}' => ENABLED (main-exception)");
            return; // 더 이상 차단 로직 적용하지 않음
        }

        bool disabled = false;
        string reason = "enabled";

        // 레스토랑: 오늘 이미 다른 맵 1회 이상 방문했으면 차단
        if (!string.IsNullOrEmpty(restaurantKey) &&
            string.Equals(k0, NormalizeKey(restaurantKey), StringComparison.OrdinalIgnoreCase) &&
            reg.GetTotalVisitsToday() > 0)
        {
            disabled = true;
            reason = "restaurant-after-first-visit";
        }

        // 버블 씬: 오늘 1회 방문 후 재입장 차단
        if (!disabled && reg.TryGetBubbleIdByScene(sceneKey, out var _))
        {
            if (!reg.CanEnterToday(sceneKey))
            {
                disabled = true;
                reason = $"bubble-reenter (visits={reg.GetVisitCount(sceneKey)})";
            }
        }

        // 버튼 색은 유지하고 클릭만 막기 위해 Transition을 None으로 전환/복구
        if (disabled)
        {
            btn.interactable = false;
            btn.transition = Selectable.Transition.None;
        }
        else
        {
            btn.interactable = true;
            if (_origTransition.TryGetValue(btn, out var t2)) btn.transition = t2;
            if (_origColors.TryGetValue(btn, out var c2))     btn.colors = c2;
        }

        // outline 색상 처리
        var outGO = FindOutlineGOForButton(btn, sceneKey);
        if (outGO != null)
        {
            if (!_originalOutlineColor.ContainsKey(outGO) && TryGetOutlineColor(outGO, out var orig))
                _originalOutlineColor[outGO] = orig;

            if (disabled)
                TrySetOutlineColor(outGO, outlineDisabled);
            else if (_originalOutlineColor.TryGetValue(outGO, out var origOk))
                TrySetOutlineColor(outGO, origOk);
        }

        // Debug.Log($"[MBP] Refresh '{sceneKey}' => {(disabled ? "DISABLED" : "ENABLED")} ({reason}), " +
        //           $"totalVisits={reg.GetTotalVisitsToday()}, thisVisits={reg.GetVisitCount(sceneKey)}");
    }

    // ---------- 클릭 처리 ----------

    public void OnClickSceneButton(string sceneKey)
    {
        StartCoroutine(OnClickSceneButtonRoutine(sceneKey));
    }

    System.Collections.IEnumerator OnClickSceneButtonRoutine(string sceneKey)
    {
        var reg = SimpleBubbleRegistry.Instance;
        string k = NormalizeKey(sceneKey);

        // main 예외: 방문 카운트/차단 미적용, 바로 로드
        if (IsMain(k))
        {
            var es0 = EventSystem.current; if (es0) es0.SetSelectedGameObject(null);
            yield return null;
            SceneManager.LoadScene(sceneKey);
            yield break;
        }

        if (reg == null) yield break;

        // 레스토랑 차단
        if (!string.IsNullOrEmpty(restaurantKey) &&
            string.Equals(k, NormalizeKey(restaurantKey), StringComparison.OrdinalIgnoreCase) &&
            reg.GetTotalVisitsToday() > 0)
            yield break;

        // 버블 씬 재입장 차단
        if (reg.TryGetBubbleIdByScene(sceneKey, out var _) && !reg.CanEnterToday(sceneKey))
            yield break;

        // 로드 대상 결정(restaurant → minigame_main)
        string loadScene = sceneKey;
        if (!string.IsNullOrEmpty(restaurantKey) &&
            string.Equals(k, NormalizeKey(restaurantKey), StringComparison.OrdinalIgnoreCase))
            loadScene = string.IsNullOrEmpty(restaurantLoadsScene) ? sceneKey : restaurantLoadsScene;

        // UI 클릭 이벤트 종료 보장
        var es = EventSystem.current; if (es) es.SetSelectedGameObject(null);
        yield return null;

        // 방문 기록 및 로드
        reg.MarkEntered(sceneKey);
        SceneManager.LoadScene(loadScene);
    }

    // ---------- 보조 ----------

    bool IsMain(string keyLower)
    {
        var k = (keyLower ?? "").Trim().ToLowerInvariant();
        var m = (mainKey   ?? "").Trim().ToLowerInvariant();
        return !string.IsNullOrEmpty(m) && k == m;
    }

    void PlaceBubbleAtButton(RectTransform bubble, RectTransform button, string bubbleIdLower)
    {
        var btnCanvas = button.GetComponentInParent<Canvas>();
        var cam = (btnCanvas != null && btnCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                  ? btnCanvas.worldCamera
                  : null;

        Vector3 buttonWorldCenter = button.TransformPoint(button.rect.center);
        Vector2 screenPos         = RectTransformUtility.WorldToScreenPoint(cam, buttonWorldCenter);

        var bubbleParent = bubble.transform.parent as RectTransform;
        if (bubbleParent == null) { Debug.LogWarning("[MBP] Missing bubble parent RectTransform"); return; }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bubbleParent, screenPos, cam, out var local))
        {
            bubble.anchoredPosition = local + baseOffset + ExtraOffset(bubbleIdLower);
            bubble.gameObject.SetActive(true);
        }
    }

    RectTransform ResolveBubbleRect(string bubbleIdLower)
    {
        if (bubbleIdLower == "bubble_ru")     return bubble_Ru;
        if (bubbleIdLower == "bubble_freyja") return bubble_Freyja;
        if (bubbleIdLower == "bubble_jin")    return bubble_Jin;
        return null;
    }

    Vector2 ExtraOffset(string bubbleIdLower)
    {
        if (bubbleIdLower == "bubble_ru")     return offsetRu;
        if (bubbleIdLower == "bubble_freyja") return offsetFreyja;
        if (bubbleIdLower == "bubble_jin")    return offsetJin;
        return Vector2.zero;
    }

    RectTransform FindButtonRect(string sceneKeyLower)
    {
        foreach (var b in buttons)
            if (NormalizeKey(b.sceneKey) == sceneKeyLower && b.rect != null)
                return b.rect;

        if (fallbackFindByName)
        {
            var go = GameObject.Find(sceneKeyLower);
            if (go != null) return go.GetComponent<RectTransform>();
        }
        return null;
    }

    GameObject FindChildByNameRecursive(Transform root, string nameLower)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var t = root.GetChild(i);
            if (t.name.Trim().ToLowerInvariant() == nameLower) return t.gameObject;
            var found = FindChildByNameRecursive(t, nameLower);
            if (found != null) return found;
        }
        return null;
    }

    GameObject FindOutlineGOForButton(Button btn, string sceneKey)
    {
        if (btn == null) return null;

        string want1 = (btn.name + "_outline").Trim().ToLowerInvariant();
        string want2 = ((sceneKey ?? "").Trim().ToLowerInvariant() + "_outline");

        GameObject TryFind(string want)
        {
            var go = FindChildByNameRecursive(btn.transform.root, want);
            if (go != null) return go;
            var p = btn.transform.parent;
            while (go == null && p != null)
            {
                go = FindChildByNameRecursive(p, want);
                p  = p.parent;
            }
            return go;
        }

        return TryFind(want1) ?? TryFind(want2);
    }

    bool TryGetOutlineColor(GameObject outlineGO, out Color color)
    {
        color = default;
        if (outlineGO == null) return false;

        var outline = outlineGO.GetComponent<Outline>();
        if (outline != null) { color = outline.effectColor; return true; }

        var img = outlineGO.GetComponent<Image>();
        if (img != null) { color = img.color; return true; }

        return false;
    }

    bool TrySetOutlineColor(GameObject outlineGO, Color color)
    {
        if (outlineGO == null) return false;

        var outline = outlineGO.GetComponent<Outline>();
        if (outline != null) { outline.effectColor = color; return true; }

        var img = outlineGO.GetComponent<Image>();
        if (img != null) { img.color = color; return true; }

        return false;
    }

    static string NormalizeKey(string s) => (s ?? "").Trim().ToLowerInvariant();
}
