using UnityEngine;

public class WorldSfx : MonoBehaviour
{
    public int onEnterSfxIndex = -1; // 마우스 오버
    public int onClickSfxIndex = -1; // 클릭

    void OnMouseEnter()
    {
        if (onEnterSfxIndex >= 0)
            SfxRegistry_Int.I?.PlayByIndex(onEnterSfxIndex);
    }

    void OnMouseDown()
    {
        if (onClickSfxIndex >= 0)
            SfxRegistry_Int.I?.PlayByIndex(onClickSfxIndex);
    }
}
