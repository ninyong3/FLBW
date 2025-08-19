using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameEscapeManager : MonoBehaviour
{
    public static MiniGameEscapeManager Instance { get; private set; }

    [SerializeField] GameObject escPanel;          // ESC 메뉴 패널
    [SerializeField] GameObject quitConfirmPanel;  // 종료 확인 패널

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
        escPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!escPanel.activeSelf && !quitConfirmPanel.activeSelf)
            {
                escPanel.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        escPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
    }

    public void Quit()
    {
        // ESC 메뉴 닫고 확인창 열기
        escPanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    public void ConfirmQuit()
    {
        // 진짜 종료
        Time.timeScale = 1.0f;
        GameManager.instance.dayCount++;
        GameManager.instance.relationship_level--;
        // day + 1 , 호감도 -1.
        SceneManager.LoadScene("main");
    }

    public void CancelQuit()
    {
        // 종료 취소 → ESC 메뉴 다시 열기
        quitConfirmPanel.SetActive(false);
        escPanel.SetActive(true);
    }
}
