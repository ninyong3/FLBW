using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 


public class MySceneManager_NoTween : MonoBehaviour
{
    public static MySceneManager_NoTween I { get; private set; }
    private const string PrefabPath = "MySceneManager_NoTween"; 

    [Header("Fade (CanvasGroup on full-screen black image)")]
    [SerializeField] private CanvasGroup fade;               
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.75f;

    [Header("Loading UI (optional)")]
    [SerializeField] private GameObject loadingRoot;   
    [SerializeField] private Text percentText;                  

    private bool busy;

   
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Ensure()
    {
        if (I == null)
        {
            var prefab = Resources.Load<MySceneManager_NoTween>(PrefabPath);
            if (prefab != null) Instantiate(prefab);
        }
    }

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        if (fade)
        {
            fade.alpha = 0f;
            fade.blocksRaycasts = false;
            fade.interactable = false;
        }
        if (loadingRoot) loadingRoot.SetActive(false);
        if (percentText) percentText.text = "";
    }

    /// 외부 호출 씬 전환
    public void ChangeScene(string sceneName)
    {
        if (busy || string.IsNullOrEmpty(sceneName)) return;
        StartCoroutine(Run(sceneName));
    }

    public static void Go(string sceneName) => I?.ChangeScene(sceneName);

    private System.Collections.IEnumerator Run(string sceneName)
    {
        busy = true;

        // 1) 암전
        yield return FadeTo(1f, fadeDuration);

        // 2) 로딩 + 비동기 로드
        if (loadingRoot) loadingRoot.SetActive(true);
        SetPercent(0);

        var async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        float display = 0f; 
        while (!async.isDone)
        {
            
            float target = Mathf.Clamp01(async.progress / 0.9f); 
            display = Mathf.MoveTowards(display, target, Time.unscaledDeltaTime * 1.5f);
            SetPercent(display * 100f);

            if (display >= 0.999f && async.progress >= 0.9f)
            {
                SetPercent(100f);
                async.allowSceneActivation = true;
            }
            yield return null;
        }

        if (loadingRoot) loadingRoot.SetActive(false);

        yield return FadeTo(0f, fadeDuration);

        busy = false;
    }

    private System.Collections.IEnumerator FadeTo(float to, float duration)
    {
        if (!fade) yield break;

        fade.blocksRaycasts = true;   
        fade.interactable = true;

        float from = fade.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; 
            float u = Mathf.Clamp01(t / duration);
            fade.alpha = Mathf.Lerp(from, to, u);
            yield return null;
        }
        fade.alpha = to;

        if (to <= 0f)
        {
            fade.blocksRaycasts = false;
            fade.interactable = false;
        }
    }

    private void SetPercent(float v)
    {
        if (!percentText) return;
        percentText.text = v.ToString("0") + "%";
    }
}
