using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameEscapeManager : MonoBehaviour
{
    public static MiniGameEscapeManager Instance { get; private set; }

    [SerializeField]
    GameObject escpanel;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        escpanel.SetActive(false);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escpanel.SetActive(true);
            Time.timeScale = 0;
        }
    }


    public void resume()
    {
        Time.timeScale = 1.0f;
        escpanel.SetActive(false);
    }

    public void quit()
    {
        //메인화면으로 나가기 현재는 게임종료로 구현
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
