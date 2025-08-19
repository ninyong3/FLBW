using UnityEngine;
using UnityEngine.SceneManagement;
// SceneChanger.cs (핵심 패치)
public class SceneChanger : MonoBehaviour
{
    [SerializeField] string mapSceneName = "map";

    public void ChangeScene(GameObject clickedObj)
    {
        string sceneName = clickedObj != null ? clickedObj.name : "";

        // ✅ map은 직접 LoadScene 금지 → MapGate 경유
        if (string.Equals(sceneName, mapSceneName, System.StringComparison.OrdinalIgnoreCase))
        {
            if (MapGateAsync.Instance != null)
            {
                Debug.Log("[SC] redirect to MapGateAsync.OpenMapAsync()");
                MapGateAsync.Instance.OpenMapAsync();   // 내부에서 selectedHeroine→bubble 동기화 + BeginDay + LoadScene
                return;
            }
            else
            {
                Debug.LogWarning("[SC] MapGateAsync.Instance == null. Fallback to direct load (will have no BeginDay)");
            }
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning("Scene not Founded: " + sceneName);
            return;
        }

        int selIdx = GameManager.instance != null ? GameManager.instance.selectedHeroine : -1;
        string current = MapGateAsync.Instance != null ? MapGateAsync.Instance.currentHeroineBubbleId : "(null)";
        Debug.Log($"[SC] ChangeScene('{sceneName}') read-only: day=?, selectedHeroine={selIdx}, current={current}");
        SceneManager.LoadScene(sceneName);
    }
}
