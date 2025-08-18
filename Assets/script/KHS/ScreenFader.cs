using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-9)]
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader I { get; private set; }

    [Range(0.01f, 5f)] public float defaultFadeOut = 0.3f;
    [Range(0.01f, 5f)] public float defaultFadeIn  = 0.3f;
    public Color color = Color.black;

    Image overlay;
    bool isFading;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // Canvas , Overlay Image 생성
        var canvasGO = new GameObject("ScreenFaderCanvas");
        canvasGO.transform.SetParent(transform, false);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767; // 최상단
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var imgGO = new GameObject("Overlay");
        imgGO.transform.SetParent(canvasGO.transform, false);
        overlay = imgGO.AddComponent<Image>();
        overlay.color = new Color(color.r, color.g, color.b, 0f);
        overlay.raycastTarget = true; // 페이드 중 입력 차단

        var rt = overlay.rectTransform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    public IEnumerator FadeOut(float d) { if (isFading) yield break; isFading = true; yield return Fade(overlay.color.a, 1f, d); }
    public IEnumerator FadeIn (float d) { yield return Fade(overlay.color.a, 0f, d); isFading = false; }

    IEnumerator Fade(float from, float to, float d)
    {
        float t = 0f; var c = color;
        while (t < d) { t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / d);
            overlay.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        overlay.color = new Color(c.r, c.g, c.b, to);
    }

    // 씬 로드 포함 원샷
    public IEnumerator FadeOutInLoad(string sceneName, System.Action beforeLoad = null, float outDur = -1f, float inDur = -1f)
    {
        yield return FadeOut(outDur > 0 ? outDur : defaultFadeOut);
        beforeLoad?.Invoke();
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        yield return null; // 한 프레임 안정화
        yield return FadeIn(inDur > 0 ? inDur : defaultFadeIn);
    }
}
