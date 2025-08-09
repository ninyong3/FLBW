using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;
public class SystemManager : MonoBehaviour
{
    [SerializeField] GameObject dialog;
    Coroutine waitClick;
    int check=0;
    GameObject currentClickObject;
    GameObject previousClickObject;
    [SerializeField] GameObject Closeimage;
    [SerializeField] GameObject Titlewarningimage;
    [SerializeField] GameObject Skipwarningimage;
    [SerializeField] TextMeshProUGUI day;
    [SerializeField] Image Testimage;
    void Update()
    {
        if (previousClickObject != Closeimage) // 이전에 클릭 된 것이 클로즈 버튼이 아닌지 확인
            check = 0; // 초기화
        previousClickObject = currentClickObject; // 이전에 클릭된 것 갱신
        currentClickObject = EventSystem.current.currentSelectedGameObject; //현재 클릭된 것 갱신
        day.text = "Day " + GameManager.instance.dayCount.ToString(); // 일자 수 갱신
    }
    void Start()
    {
        Titlewarningimage.SetActive(false); // 타이틀 경고창 숨김
        Skipwarningimage.SetActive(false); // 스킵 경고창 숨김
        day.text = "Day 1";
    }
    public void CloseSystem() // 클로즈 구현을 위한 함수
    {
        if (check == 0) // 현재 대화창이 보이고 있을 경우
        {
            dialog.SetActive(false); // 대화창 숨김
            check = 1;
            StartCoroutine(ReshowDialog());
        }
        else // 두번 연속으로 클로즈를 클릭했을 시 대화창을 숨기지 않고 보이기
            check= 0;
    }
    IEnumerator ReshowDialog()  // 대화창 보이기를 위한 함수
    { 
        while(!Input.GetMouseButtonDown(0)) // 좌클릭을 할 때까지 반복
        {
            yield return null;
        }
        dialog.SetActive(true); //대화창 보이기
    }
    public void ToTitleSystem() // 타이틀로 이동하기 위한 함수
    {
        Titlewarningimage.SetActive(true); // 타이틀 경고창 보이기
    }
    public void ToTitleClickYes() // 타이틀 경고창에서 네를 눌렀을 때 작동하는 함수
    {
        GameManager.instance.previousScene=SceneManager.GetActiveScene().name; // 현재 씬을 이전 씬으로 등록
        SceneManager.LoadScene("title"); // 타이틀 씬으로 이동
    }
    public void ToTitleClickNo() // 타이틀 경고창에서 아니오를 눌렀을 때 작동하는 함수
    {
        Titlewarningimage.SetActive(false); //경고창 닫기
    }
    public void SkipDaySystem() // 일자를 넘기기 위한 함수
    {
        Skipwarningimage.SetActive(true); // 스킵 경고창 보이기
    }
    public void SkipDayClickYes() // 스킵 경고창에서 네를 눌렀을 시 작동하는 함수
    {
        GameManager.instance.dayCount++; // 다음 일자로 넘기기
        Skipwarningimage.SetActive(false); // 스킵 경고창 닫기
    }
    public void SkipDayClickNo() // 스킵 경고창에서 아니오를 눌렀을 시 작동하는 함수
    {
        Skipwarningimage.SetActive(false); // 스킵 경고창 닫기
    }
    public void ToConfigSystem() // 설정창으로 이동하기 위한 함수
    {
        GameManager.instance.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("config");
    }
    public void OpenKeword() // 키워드로 이동하기 위한 함수
    {
        GameManager.instance.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("keword");
    }
    public void SaveSystem()
    {
       StartCoroutine(CaptureSaveImage());
    }
    IEnumerator CaptureSaveImage()
    {
        yield return new WaitForEndOfFrame();
        Texture2D saveScreenshot = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytes = saveScreenshot.EncodeToPNG();
        Destroy(saveScreenshot);
        string saveScreenshotFolderPath = Path.Combine(Application.persistentDataPath, "saveScreenshot");
        if (!Directory.Exists(saveScreenshotFolderPath))
        {
            Directory.CreateDirectory(saveScreenshotFolderPath);
        }
        string saveScreenshotFileName = $"SaveScreenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        string saveScreenshotPath = Path.Combine(saveScreenshotFolderPath, saveScreenshotFileName);
        File.WriteAllBytes(saveScreenshotPath, bytes);
        byte[] bytes1 = File.ReadAllBytes(saveScreenshotPath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes1);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        Testimage.sprite = sprite;
        Testimage.color = Color.white;
        Debug.Log(saveScreenshotPath);
    }
}
