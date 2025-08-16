using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapGateAsync : MonoBehaviour
{
    public static MapGateAsync Instance { get; private set; }
    public string mapSceneName = "Map";
    public string currentHeroineBubbleId = "bubble_Jin";
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            
            Instance.currentHeroineBubbleId = currentHeroineBubbleId;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[MapGateAsync] Awake current={currentHeroineBubbleId}");
    }

    public void OpenMapAsync()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        // 히로인 ID를 DayManager에 반영
        var dm = FindFirstObjectByType<DayManager>();
        if (dm != null)
            dm.currentHeroineBubbleId = currentHeroineBubbleId;

        //  Day 계산
        int day = 1;
        if (dm != null) day = Mathf.Max(1, dm.currentDay);
        else if (GameManager.instance != null) day = Mathf.Max(1, GameManager.instance.dayCount);

       
        string heroineId = (dm != null) ? dm.currentHeroineBubbleId : currentHeroineBubbleId;
       

        //  비동기 로드
        var op = SceneManager.LoadSceneAsync(mapSceneName);
        while (!op.isDone) yield return null;
    }
}
