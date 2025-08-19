using UnityEngine;
using UnityEngine.EventSystems;

public class UiSfx : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("SFX 인덱스(없으면 -1)")]
    public int hoverSfxIndex = -1;
    public int clickSfxIndex = -1;

    [Header("호버 연타 방지(초)")]
    public float hoverCooldown = 0.05f;
    float _lastHoverTime = -999f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSfxIndex < 0) return;
        if (Time.unscaledTime - _lastHoverTime < hoverCooldown) return;
        _lastHoverTime = Time.unscaledTime;
        SfxRegistry_Int.I?.PlayByIndex(hoverSfxIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSfxIndex < 0) return;
        SfxRegistry_Int.I?.PlayByIndex(clickSfxIndex);
    }

    // 필요시 외부 호출
    public void PlayCustom(int index) => SfxRegistry_Int.I?.PlayByIndex(index);
}
