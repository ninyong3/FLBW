using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(GameObject clickedObj)
    {
        string sceneName = clickedObj.name;
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning("Scene not Founded: " + sceneName);
            return;
        }

        int day = (FindFirstObjectByType<DayManager>()?.currentDay)
                  ?? Mathf.Max(1, GameManager.instance?.dayCount ?? 1);

        //  MapGateAsync 싱글톤에서 읽기
        string heroineId =
            (MapGateAsync.Instance != null && !string.IsNullOrWhiteSpace(MapGateAsync.Instance.currentHeroineBubbleId))
            ? MapGateAsync.Instance.currentHeroineBubbleId
            : (FindFirstObjectByType<DayManager>()?.currentHeroineBubbleId ?? "bubble_Jin");

        // 여기서만 BeginDay 호출 (중복 금지)
        SimpleBubbleRegistry.Instance.BeginDay(day, heroineId);
        Debug.Log($"[SBR] BeginDay(day={day}, current={heroineId}, caller=SceneChanger.ChangeScene())");

        SceneManager.LoadScene(sceneName);
    }
}
