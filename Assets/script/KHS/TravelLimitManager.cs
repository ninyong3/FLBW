using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[DefaultExecutionOrder(-10)]
public class TravelLimitManager : MonoBehaviour
{
    public static TravelLimitManager I { get; private set; }

    [Header("Scene names")]
    [SerializeField] string mapSceneName = "map";
    [SerializeField] string homeSceneName = "main";

    [Header("Exclusions")]
    [SerializeField] string[] minigameSceneNames = new string[] { "minigame_main" };

    [Header("Limit")]
    [SerializeField, Min(0)] int maxNonMinigameMoves = 2;

    [Header("Debug")]
    [SerializeField] bool verboseLogs = true;

    HashSet<string> miniSet;
    int count;
    bool redirecting;

    // ★ prev 파라미터 대신 우리가 직접 추적
    string lastSceneName = "";

    // ★ 앱 시작 전에 반드시 1개 존재하도록 보장 (씬에 안 둬도 됨)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (I == null)
        {
            var go = new GameObject(nameof(TravelLimitManager));
            go.AddComponent<TravelLimitManager>();
        }
    }

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        miniSet = new HashSet<string>(minigameSceneNames);

        // 현재 액티브 씬을 시작점으로 기록
        lastSceneName = SceneManager.GetActiveScene().name;

        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        if (verboseLogs)
        {
            Debug.Log($"[TLM] Awake | map='{mapSceneName}', home='{homeSceneName}', start='{lastSceneName}', max={maxNonMinigameMoves}");
            DumpScenesInBuild();
        }
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        if (I == this) I = null;
    }

    bool IsMinigame(string scene) => !string.IsNullOrEmpty(scene) && miniSet.Contains(scene);

    public void ResetCount()
    {
        if (verboseLogs) Debug.Log($"[TLM] ResetCount (before={count})");
        count = 0;
    }

    void OnActiveSceneChanged(Scene prev, Scene next)
    {
        // prev.name 대신 우리가 저장한 lastSceneName 사용
        string prevName = lastSceneName;
        string nextName = next.name;

        if (verboseLogs)
            Debug.Log($"[TLM] activeSceneChanged: '{prevName}' -> '{nextName}' (count={count}/{maxNonMinigameMoves})");

        // 맵으로 돌아올 때만 카운트 증가
        if (nextName == mapSceneName)
        {
            bool prevIsMap  = (prevName == mapSceneName);
            bool prevIsHome = (prevName == homeSceneName);
            bool prevIsMini = IsMinigame(prevName);

            bool shouldCount =
                !string.IsNullOrEmpty(prevName) &&
                !prevIsMap && !prevIsHome && !prevIsMini;

            if (verboseLogs)
                Debug.Log($"[TLM] return→map? prevIsMap={prevIsMap}, prevIsHome={prevIsHome}, prevIsMini={prevIsMini}, shouldCount={shouldCount}");

            if (shouldCount)
            {
                int before = count;
                count++;
                Debug.Log($"[TLM] ++count {before}→{count} (from '{prevName}' to '{nextName}')");
            }
            else if (verboseLogs)
            {
                Debug.Log($"[TLM] no count (prev='{prevName}' exempted)");
            }

            // 제한 도달: 맵 진입 직후 홈으로 리다이렉트 + count=0 + day++
            if (count >= maxNonMinigameMoves && !redirecting)
            {
                redirecting = true;

                int before = count;
                count = 0;

                if (GameManager.instance != null)
                {
                    GameManager.instance.dayCount++;
                    Debug.Log($"[TLM] limit reached: count {before}→0, dayCount++ → {GameManager.instance.dayCount}");
                }
                else
                {
                    Debug.LogWarning("[TLM] GameManager.instance null, dayCount++ skipped");
                }

                if (ScreenFader.I != null)
                {
                    I.StartCoroutine(ScreenFader.I.FadeOutInLoad(homeSceneName, () =>
                    {
                        redirecting = false;
                        if (verboseLogs) Debug.Log("[TLM] redirected to home (with fade)");
                    }));
                }
                else
                {
                    SceneManager.LoadScene(homeSceneName, LoadSceneMode.Single);
                    redirecting = false;
                    if (verboseLogs) Debug.Log("[TLM] redirected to home (instant)");
                }
            }
            else if (verboseLogs)
            {
                Debug.Log($"[TLM] after-return count={count}/{maxNonMinigameMoves}, redirecting={redirecting}");
            }
        }

        // ★ 마지막에 항상 업데이트 (다음 변경의 prev로 사용)
        lastSceneName = nextName;
    }

    void DumpScenesInBuild()
    {
        int n = SceneManager.sceneCountInBuildSettings;
        var names = new List<string>(n);
        for (int i = 0; i < n; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            names.Add(Path.GetFileNameWithoutExtension(path));
        }
        Debug.Log("[TLM] ScenesInBuild: " + string.Join(", ", names));
    }
}
