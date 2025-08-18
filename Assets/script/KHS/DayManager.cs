using UnityEngine;
using UnityEngine.Events;

public class DayManager : MonoBehaviour
{
    public int currentDay = 1;

    // 기본값
    public string currentHeroineBubbleId = "bubble_Jin";

    public UnityEvent<int> onDayChanged;
  
void Awake()
{
    var gm = GameManager.instance;
    // Day 복원 (없으면 기존 값 유지)
    if (gm != null)
        currentDay = Mathf.Max(1, gm.dayCount);

   
    if (MapGateAsync.Instance != null &&
        !string.IsNullOrWhiteSpace(MapGateAsync.Instance.currentHeroineBubbleId))
    {
        currentHeroineBubbleId = MapGateAsync.Instance.currentHeroineBubbleId;
    }

    Debug.Log($"[DayManager] Awake day={currentDay}, heroine={currentHeroineBubbleId}");
}


public void ApplyDay()
{
    Debug.Log($"[DayManager] ApplyDay day={currentDay}, heroine={currentHeroineBubbleId}");

    onDayChanged?.Invoke(currentDay);

    // ★ Day/히로인 보존
    PlayerPrefs.SetInt("lastDay", currentDay);
    PlayerPrefs.SetString("lastHeroineBubbleId", currentHeroineBubbleId);
    PlayerPrefs.Save();
}



}