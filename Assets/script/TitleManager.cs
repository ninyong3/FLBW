using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TitleManager : MonoBehaviour
{
    [SerializeField] Image jinyeinStar;
    [SerializeField] Image freyjaStar;
    [SerializeField] Image ruStar;
    void Start()
    {
        if(GameManager.instance.PersistentData.endingClearCheck[0] == false)
            jinyeinStar.color = Color.white;
        if (GameManager.instance.PersistentData.endingClearCheck[1] == false)
            freyjaStar.color = Color.white;
        if (GameManager.instance.PersistentData.endingClearCheck[2] == false)
            ruStar.color = Color.white;

    }
    void Update()
    {
        
    }
    public void StartGame() // 시작하기 함수
    {
        GameManager.instance.dayCount = 0; // day를 1로 초기화
        GameManager.instance.relationship_level = 0;
        GameManager.instance.dialogCount = 1;
        GameManager.instance.playerName = "";
        GameManager.instance.previousScene = SceneManager.GetActiveScene().name; // 현재 활성화 된 씬 이름 저장
        GameManager.instance.messageCount = 1;
        for(int i=0;i<6;i++)
        {
            GameManager.instance.messageCountCheckList[i] = false;
        }
        GameManager.instance.leftMessageCount = 0;
        GameManager.instance.userChoice = null;
        SceneManager.LoadScene("ep0"); // ep0 씬 불러오기
    }
    public void OpenLoad()
    {
        GameManager.instance.previousScene = SceneManager.GetActiveScene().name;
        GameManager.instance.saveLoadCheck = false;
        SceneManager.LoadScene("saveload"); // 세이브 로드 씬 불러오기
    }
    public void OpenExtra()
    {
        GameManager.instance.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("extra"); // 추가 요소 씬 불러오기
    }
    public void OpenConfig()
    {
        GameManager.instance.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("config"); // 설정 씬 불러오기
    }
    public void ExitGame()
    {
#if UNITY_EDITOR // 에디터에서 작동하는 경우
        UnityEditor.EditorApplication.isPlaying = false; // 게임뷰 종료
#else // 프로그램으로 작동하는 경우
        Application.Quit(); // 게임 종료
#endif
    }
}
