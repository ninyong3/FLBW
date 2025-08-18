// HeroineImageSwitcher.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroineImageSwitcher : MonoBehaviour
{
    [Header("씬에 배치된 히로인 오브젝트")]
    public GameObject Image_Jin;
    public GameObject Image_Freyja;
    public GameObject Image_Ru;

    [Header("이름으로 자동 연결 (Image_Jin / Image_Freyja / Image_Ru)")]
    public bool autoWireByName = true;

    void Awake()
    {
        if (autoWireByName)
        {
            if (Image_Jin == null) Image_Jin = GameObject.Find("Image_Jin");
            if (Image_Freyja == null) Image_Freyja = GameObject.Find("Image_Freyja");
            if (Image_Ru == null) Image_Ru = GameObject.Find("Image_Ru");
        }
    }

    void OnEnable()
    {
        var id = MapGateAsync.Instance ? MapGateAsync.Instance.currentHeroineBubbleId : "(null)";
        Debug.Log($"[HIS] OnEnable read heroine={id}");

        Apply();
    }

    public void Apply()
    {
        // 기본적으로 전부 끄고 시작
        SetOnlyActive(null);

        var sceneKey = SceneManager.GetActiveScene().name;
        var reg = SimpleBubbleRegistry.Instance;

        if (reg == null)
        {
            Debug.LogWarning("[HeroineImageSwitcher] SimpleBubbleRegistry 없음");
            return;
        }

        if (!reg.TryGetBubbleIdByScene(sceneKey, out var bubbleId))
        {
            Debug.LogWarning($"[HeroineImageSwitcher] 키 '{sceneKey}' 매핑 없음");
            return;
        }

        var id = (bubbleId ?? "").Trim().ToLowerInvariant();
        if (id == "bubble_jin") SetOnlyActive(Image_Jin);
        else if (id == "bubble_freyja") SetOnlyActive(Image_Freyja);
        else if (id == "bubble_ru") SetOnlyActive(Image_Ru);
        else
        {
            Debug.LogWarning($"[HeroineImageSwitcher] 알 수 없는 bubbleId '{bubbleId}'");
        }
    }

    void SetOnlyActive(GameObject target)
    {
        if (Image_Jin != null) Image_Jin.SetActive(target == Image_Jin);
        if (Image_Freyja != null) Image_Freyja.SetActive(target == Image_Freyja);
        if (Image_Ru != null) Image_Ru.SetActive(target == Image_Ru);
    }
    GameObject FindInSceneEvenIfInactive(string targetName)
{
    
    var roots = gameObject.scene.GetRootGameObjects();
    foreach (var root in roots)
    {
        var trs = root.GetComponentsInChildren<Transform>(true);
        foreach (var tr in trs)
        {
            if (tr.name == targetName) return tr.gameObject;
        }
    }
    return null;
}


}
