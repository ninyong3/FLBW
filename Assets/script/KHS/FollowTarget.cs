using UnityEngine;

[ExecuteInEditMode] 
public class FollowTarget : MonoBehaviour
{
    public RectTransform targetObject;

    void OnValidate()
    {
        if (targetObject != null)
        {
            RectTransform myRect = GetComponent<RectTransform>();

            // 위치와 크기
            myRect.anchoredPosition = targetObject.anchoredPosition;
            myRect.sizeDelta = targetObject.sizeDelta;
            // 정렬 관련 요소
            myRect.anchorMin = targetObject.anchorMin;
            myRect.anchorMax = targetObject.anchorMax;
            myRect.pivot = targetObject.pivot;
            myRect.localScale = targetObject.localScale;
            enabled = false; // 한 번만 실행
        }
    }
}