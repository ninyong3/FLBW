using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapGateAsync : MonoBehaviour
{
    public static MapGateAsync Instance { get; private set; }
    public string mapSceneName = "Map";
    public string currentHeroineBubbleId = "bubble_Ru";



    void Awake()
    {
        if (Instance != null && Instance != this)
        {

            // Instance.currentHeroineBubbleId = currentHeroineBubbleId;
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



         // GameManager의 selectedHeroine 값 받아오기
        int heroineIndex = (GameManager.instance != null) ? GameManager.instance.selectedHeroine : 0;
        Debug.Log($"[MapGateAsync] 선택된 히로인 인덱스: {heroineIndex}");

        // heroineIndex에 따라 currentHeroineBubbleId 바꾸기 (예시)
        switch (heroineIndex)
        {
            case 1: currentHeroineBubbleId = "bubble_Jin"; break;
            case 2: currentHeroineBubbleId = "bubble_Freyja"; break;
            case 3: currentHeroineBubbleId = "bubble_Ru"; break;
            default: currentHeroineBubbleId = "bubble_Ru"; break;
        }


        //  비동기 로드
        var op = SceneManager.LoadSceneAsync(mapSceneName);
        while (!op.isDone) yield return null;
    }
    

    
}
