using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleManager : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void StartGame() // 시작하기 함수
    {
        GameManager.instance.dayCount = 1; // day를 1로 초기화
        GameManager.instance.previousScene = SceneManager.GetActiveScene().name; // 현재 활성화 된 씬 이름 저장
        SceneManager.LoadScene("main"); // 메인 씬 불러오기(차후 ep0로 연결 예정)
    }
    public void OpenLoad()
    {
        GameManager.instance.previousScene = SceneManager.GetActiveScene().name;
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
