using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour
   
{
 [Tooltip("외곽선 역할을 하는 자식 GameObject")]
    public GameObject outlineObj;

    // 반드시 BaseEventData 파라미터를 선언해야 EventTrigger 목록에 뜹니다.
    public void OnPointerEnter(BaseEventData data)
    {
        outlineObj.SetActive(true);
    }

    public void OnPointerExit(BaseEventData data)
    {
        outlineObj.SetActive(false);
    }
}
