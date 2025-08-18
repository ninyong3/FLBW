using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ButtonRef
{
    public string sceneKey;
    public RectTransform rect;
}

public class MapBubblePlacer : MonoBehaviour
{
    [Header("버블 RectTransform")]
    public RectTransform bubble_Ru;
    public RectTransform bubble_Freyja;
    public RectTransform bubble_Jin; // 필요 없으면 비워도 됨

    [Header("버튼(장소) RectTransform 목록")]
    public List<ButtonRef> buttons = new List<ButtonRef>();

    [Header("오프셋")]
    public Vector2 baseOffset;
    public Vector2 offsetRu;
    public Vector2 offsetFreyja;
    public Vector2 offsetJin;

    [Tooltip("버튼 목록에 없다면 Hierarchy 이름으로 찾아 사용")]
    public bool fallbackFindByName = true;

    void OnEnable() => ApplyToday();

    void ResetBubbles()
    {
        if (bubble_Ru)     bubble_Ru.gameObject.SetActive(false);
        if (bubble_Freyja) bubble_Freyja.gameObject.SetActive(false);
        if (bubble_Jin)    bubble_Jin.gameObject.SetActive(false);
    }

    // Day가 바뀐 직후 등 외부에서 수동 호출 가능
    public void ApplyToday()
    {
        ResetBubbles();

        var reg = SimpleBubbleRegistry.Instance;
        if (reg == null) { Debug.LogWarning("[MBP] SimpleBubbleRegistry 없음"); return; }

        var today = reg.GetTodaySnapshot();

        // 디버깅 문자열
        var sb = new System.Text.StringBuilder();
        foreach (var kv in today)
        {
            if (sb.Length > 0) sb.Append(", ");
            sb.Append(kv.Key).Append("->").Append(kv.Value);
        }
        Debug.Log("[MBP] pairs = " + sb.ToString());

        if (today == null || today.Count == 0)
        {
            Debug.LogWarning("[MBP] 오늘자 매핑 비어있음(BeginDay 호출 확인)");
            return;
        }

        foreach (var kv in today)
        {
            var key      = NormalizeKey(kv.Key);
            var bubbleId = NormalizeKey(kv.Value);

            var button = FindButtonRect(key);
            if (button == null) { Debug.LogWarning($"[MBP] 버튼 '{key}' 없음"); continue; }

            var bubble = ResolveBubbleRect(bubbleId);
            if (bubble == null) { Debug.LogWarning($"[MBP] 버블 '{bubbleId}' 미할당"); continue; }

            PlaceBubbleAtButton(bubble, button, bubbleId);
        }
    }

    // 버튼 중심 → 화면좌표 → 버블 부모 로컬좌표로 변환해서 앵커 위치 지정
    void PlaceBubbleAtButton(RectTransform bubble, RectTransform button, string bubbleIdLower)
    {
        var btnCanvas = button.GetComponentInParent<Canvas>();
        var cam = (btnCanvas != null && btnCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                  ? btnCanvas.worldCamera
                  : null;

        Vector3 buttonWorldCenter = button.TransformPoint(button.rect.center);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, buttonWorldCenter);

        var bubbleParent = bubble.transform.parent as RectTransform;
        if (bubbleParent == null) { Debug.LogWarning("[MapBubblePlacer] 버블 부모 RectTransform 없음"); return; }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bubbleParent, screenPos, cam, out var local))
        {
            bubble.anchoredPosition = local + baseOffset + ExtraOffset(bubbleIdLower);
            // ★ 중요: 위치 지정 후 켜주기
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
        {
            if (NormalizeKey(b.sceneKey) == sceneKeyLower && b.rect != null)
                return b.rect;
        }

        if (fallbackFindByName)
        {
            var go = GameObject.Find(sceneKeyLower);
            if (go != null) return go.GetComponent<RectTransform>();
        }
        return null;
    }

    static string NormalizeKey(string s) => (s ?? "").Trim().ToLowerInvariant();
}
