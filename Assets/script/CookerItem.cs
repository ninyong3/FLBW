using UnityEngine;

public class CookerItem : MonoBehaviour
{
    // 조리기구에 해당하는 아이콘 
    public Sprite cookerIcon;

    // 조리 방식 정의
    public enum CookType
    {
        Grill, 
        Boil,   
        Cut     
    }

    public CookType cookType;
}
