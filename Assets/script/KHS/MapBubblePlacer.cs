using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[Serializable]
public struct ButtonRef { public string sceneKey; public RectTransform rect; }

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
    public string restaurantKey = "restaurant";
    [Header("예외 키")]
    public string mainKey = "main";

    [Header("Outline 색상")]
    public Color outlineDisabled = new Color(0.6f, 0.6f, 0.6f, 0.8f);

    [Header("씬 라우팅 오버라이드")]
    public string restaurantLoadsScene = "minigame_main";

    readonly Dictionary<Button, Selectable.Transition> _origTransition = new();
    readonly Dictionary<Button, ColorBlock> _origColors = new();
    readonly Dictionary<GameObject, Color> _originalOutlineColor = new();

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
        StartCoroutine(KeepRefreshForFrames(4));
        SimpleBubbleRegistry.DebugDump();
    }

    void ResetBubbles()
    {
        if (bubble_Ru)     bubble_Ru.gameObject.SetActive(false);
        if (bubble_Freyja) bubble_Freyja.gameObject.SetActive(false);
        if (bubble_Jin)    bubble_Jin.gameObject.SetActive(false);
    }

    public void ApplyToday()
    {
        var reg = SimpleBubbleRegistry.Instance;
        if (reg == null) { Debug.LogWarning("[MBP] SimpleBubbleRegistry not found"); return; }

        var today = reg.GetTodaySnapshot();
        Debug.Log("[MBP] snapshot = " + (today==null ? "null" : string.Join(", ", today.Select(kv => $"[{kv.Key}, {kv.Value}]"))));

        // ★ 스냅샷이 비었으면 기존 배치 유지 (증발 방지)
        if (today == null || today.Count == 0)
        {
            Debug.LogWarning("[MBP] Today map is empty → keep previous bubbles (no reset)");
            return;
        }

        ResetBubbles();

        foreach (var kv in today)
        {
            var key = NormalizeKey(kv.Key);
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

        if (btn.onClick != null && btn.onClick.GetPersistentEventCount() > 0)
            btn.onClick = new Button.ButtonClickedEvent();
        else
            btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() => OnClickSceneButton(sceneKey));
        CacheBtnStyle(btn);
        RefreshButtonState(btn, sceneKey);
    }

    System.Collections.IEnumerator LateWireAndRefresh()
    { yield return null; WireButtonsOnce(); RefreshAllButtonsOnce(); }

    System.Collections.IEnumerator KeepRefreshForFrames(int frames)
    {
        for (int i = 0; i < frames; i++) { yield return null; WireButtonsOnce(); RefreshAllButtonsOnce(); }
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

    public void RefreshButtonState(Button btn, string sceneKey)
    {
        if (btn == null || !btn) return;
        CacheBtnStyle(btn);

        var reg = SimpleBubbleRegistry.Instance;
        if (reg == null) return;

        var k0 = NormalizeKey(sceneKey);

        if (IsMain(k0))
        {
            btn.interactable = true;
            if (_origTransition.TryGetValue(btn, out var t)) btn.transition = t;
            if (_origColors.TryGetValue(btn, out var c))     btn.colors = c;
            var outlineGO = FindOutlineGOForButton(btn, sceneKey);
            if (outlineGO != null && _originalOutlineColor.TryGetValue(outlineGO, out var orig2))
                TrySetOutlineColor(outlineGO, orig2);
            return;
        }

        bool disabled = false;

        if (!string.IsNullOrEmpty(restaurantKey) &&
            string.Equals(k0, NormalizeKey(restaurantKey), StringComparison.OrdinalIgnoreCase) &&
            reg.GetTotalVisitsToday() > 0)
            disabled = true;

        if (!disabled && reg.TryGetBubbleIdByScene(sceneKey, out var _) && !reg.CanEnterToday(sceneKey))
            disabled = true;

        // ★ 버튼을 비활성화해야 한다면: 먼저 재배치 시도
        if (disabled)
        {
            if (reg.TryReassignFromDisabledScene(sceneKey, "MapBubblePlacer.RefreshButtonState"))
            {
                // 재배치에 성공했으면 화면을 즉시 갱신
                ApplyToday();
            }

            btn.interactable = false;
            btn.transition = Selectable.Transition.None;
        }
        else
        {
            btn.interactable = true;
            if (_origTransition.TryGetValue(btn, out var t2)) btn.transition = t2;
            if (_origColors.TryGetValue(btn, out var c2))     btn.colors = c2;
        }

        var outGO = FindOutlineGOForButton(btn, sceneKey);
        if (outGO != null)
        {
            if (!_originalOutlineColor.ContainsKey(outGO) && TryGetOutlineColor(outGO, out var orig))
                _originalOutlineColor[outGO] = orig;
            if (disabled) TrySetOutlineColor(outGO, outlineDisabled);
            else if (_originalOutlineColor.TryGetValue(outGO, out var origOk)) TrySetOutlineColor(outGO, origOk);
        }
    }

    public void OnClickSceneButton(string sceneKey)
    { StartCoroutine(OnClickSceneButtonRoutine(sceneKey)); }

    System.Collections.IEnumerator OnClickSceneButtonRoutine(string sceneKey)
    {
        var reg = SimpleBubbleRegistry.Instance; string k = NormalizeKey(sceneKey);

        if (IsMain(k))
        { var es0 = EventSystem.current; if (es0) es0.SetSelectedGameObject(null);
          yield return null; SceneManager.LoadScene(sceneKey); yield break; }

        if (reg == null) yield break;

        if (!string.IsNullOrEmpty(restaurantKey) &&
            string.Equals(k, NormalizeKey(restaurantKey), StringComparison.OrdinalIgnoreCase) &&
            reg.GetTotalVisitsToday() > 0) yield break;

        if (reg.TryGetBubbleIdByScene(sceneKey, out var _) && !reg.CanEnterToday(sceneKey)) yield break;

        string loadScene = sceneKey;
        if (!string.IsNullOrEmpty(restaurantKey) &&
            string.Equals(k, NormalizeKey(restaurantKey), StringComparison.OrdinalIgnoreCase))
            loadScene = string.IsNullOrEmpty(restaurantLoadsScene) ? sceneKey : restaurantLoadsScene;

        var es = EventSystem.current; if (es) es.SetSelectedGameObject(null);
        yield return null;

        reg.MarkEntered(sceneKey);
        SceneManager.LoadScene(loadScene);
    }

    bool IsMain(string keyLower)
    { var k = (keyLower ?? "").Trim().ToLowerInvariant();
      var m = (mainKey   ?? "").Trim().ToLowerInvariant();
      return !string.IsNullOrEmpty(m) && k == m; }

    void PlaceBubbleAtButton(RectTransform bubble, RectTransform button, string bubbleIdLower)
    {
        var btnCanvas = button.GetComponentInParent<Canvas>();
        var cam = (btnCanvas != null && btnCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? btnCanvas.worldCamera : null;

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
    { if (bubbleIdLower == "bubble_ru") return bubble_Ru;
      if (bubbleIdLower == "bubble_freyja") return bubble_Freyja;
      if (bubbleIdLower == "bubble_jin") return bubble_Jin;
      return null; }

    Vector2 ExtraOffset(string bubbleIdLower)
    { if (bubbleIdLower == "bubble_ru") return offsetRu;
      if (bubbleIdLower == "bubble_freyja") return offsetFreyja;
      if (bubbleIdLower == "bubble_jin") return offsetJin;
      return Vector2.zero; }

    RectTransform FindButtonRect(string sceneKeyLower)
    {
        foreach (var b in buttons)
            if (NormalizeKey(b.sceneKey) == sceneKeyLower && b.rect != null) return b.rect;

        if (fallbackFindByName)
        { var go = GameObject.Find(sceneKeyLower);
          if (go != null) return go.GetComponent<RectTransform>(); }
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
            { go = FindChildByNameRecursive(p, want); p = p.parent; }
            return go;
        }
        return TryFind(want1) ?? TryFind(want2);
    }

    bool TryGetOutlineColor(GameObject outlineGO, out Color color)
    {
        color = default; if (outlineGO == null) return false;
        var outline = outlineGO.GetComponent<Outline>(); if (outline != null) { color = outline.effectColor; return true; }
        var img = outlineGO.GetComponent<Image>(); if (img != null) { color = img.color; return true; }
        return false;
    }

    bool TrySetOutlineColor(GameObject outlineGO, Color color)
    {
        if (outlineGO == null) return false;
        var outline = outlineGO.GetComponent<Outline>(); if (outline != null) { outline.effectColor = color; return true; }
        var img = outlineGO.GetComponent<Image>(); if (img != null) { img.color = color; return true; }
        return false;
    }

    static string NormalizeKey(string s) => (s ?? "").Trim().ToLowerInvariant();
}
