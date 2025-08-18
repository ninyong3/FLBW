using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour
{
    [Tooltip("외곽선 역할을 하는 자식 GameObject")]
    public GameObject outlineObj;

    void Awake()
    {
        // 에디터에서 비워둔 경우, 관례적으로 "Outline" 이름의 자식을 자동 할당(있으면) 
        if (outlineObj == null)
        {
            var t = transform.Find("Outline");
            if (t != null) outlineObj = t.gameObject;
        }
    }

    void OnDisable()
    {
        // 비활성화/씬 전환 시 안전하게 꺼줌
        if (outlineObj != null) outlineObj.SetActive(false);
    }

    // 반드시 BaseEventData 파라미터를 선언해야 EventTrigger에서 보임
    public void OnPointerEnter(BaseEventData data)
    {
        if (outlineObj != null) outlineObj.SetActive(true);
        
    }

    public void OnPointerExit(BaseEventData data)
    {
        if (outlineObj != null) outlineObj.SetActive(false);
    }
}
