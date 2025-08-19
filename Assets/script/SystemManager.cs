using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.UI;
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
    [SerializeField] GameObject Quickloadwarningimage;
    [SerializeField] TextMeshProUGUI day;
    [SerializeField] GameObject Autoimage;
    [SerializeField] GameObject Log;
    [SerializeField] GameObject Allsystem;
    Coroutine saveCoroutine;
    void Update()
    {
        if (previousClickObject != Closeimage) // 이전에 클릭 된 것이 클로즈 버튼이 아닌지 확인
            check = 0; // 초기화
        previousClickObject = currentClickObject; // 이전에 클릭된 것 갱신
        currentClickObject = EventSystem.current.currentSelectedGameObject; //현재 클릭된 것 갱신
        if(SceneManager.GetActiveScene().name == "main" || SceneManager.GetActiveScene().name == "map")
            day.text = "Day " + GameManager.instance.dayCount.ToString(); // 일자 수 갱신
        if (GameManager.instance.printSetting == 0)
        {
            Autoimage.transform.Rotate(new Vector3(0, 0, -400f * Time.deltaTime));
        }
        else
            Autoimage.transform.rotation =Quaternion.Euler(0, 0, 0);
    }
    void Start()
    {
        Titlewarningimage.SetActive(false); // 타이틀 경고창 숨김
        Skipwarningimage.SetActive(false); // 스킵 경고창 숨김
        Quickloadwarningimage.SetActive(false);
        if (SceneManager.GetActiveScene().name == "main") 
             day.text = "Day 1";
    }
    public void CloseSystem() // 클로즈 구현을 위한 함수
    {
        dialog.SetActive(false); // 대화창 숨기기
        Allsystem.SetActive(false);
        StartCoroutine(ReshowDialog());
    }
    IEnumerator ReshowDialog()  // 대화창 보이기를 위한 함수
    { 
        while(!Input.GetMouseButtonDown(0)) // 좌클릭을 할 때까지 반복
        {
            yield return null;
        }
        Allsystem.SetActive(true);
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
        if(SceneManager.GetActiveScene().name != "ep0")
            Skipwarningimage.SetActive(true); // 스킵 경고창 보이기
    }
    public void SkipDayClickYes() // 스킵 경고창에서 네를 눌렀을 시 작동하는 함수
    {
        GameManager.instance.dayCount++; // 다음 일자로 넘기기
        GameManager.instance.dialogCount = 1;
        if (SceneManager.GetActiveScene().name != "main")
            SceneManager.LoadScene("main");
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
        saveCoroutine = null;
        saveCoroutine=StartCoroutine(CaptureSaveImage());
        StartCoroutine(WaitForSave());
    }
    IEnumerator CaptureSaveImage()
    {
        yield return new WaitForEndOfFrame();
        Texture2D saveScreenshot = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] saveImagetobytes = saveScreenshot.EncodeToPNG();
        Destroy(saveScreenshot);
        string saveScreenshotFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "saveScreenshot");
        if (!Directory.Exists(saveScreenshotFolderPath))
        {
            Directory.CreateDirectory(saveScreenshotFolderPath);
        }
        string saveScreenshotFileName = $"SaveScreenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        string saveScreenshotPath = Path.Combine(saveScreenshotFolderPath, saveScreenshotFileName);
        GameManager.instance.saveImagePath = saveScreenshotPath;
        File.WriteAllBytes(saveScreenshotPath, saveImagetobytes);
        saveCoroutine = null;
    }
    IEnumerator WaitForSave()
    {
        while (saveCoroutine != null)
        {
            yield return null;
        }
        GameManager.instance.previousScene= SceneManager.GetActiveScene().name;
        GameManager.instance.saveLoadCheck = true;
        GameManager.instance.quickCheck = false;
        SceneManager.LoadScene("saveload");
    }
    public void LoadSystem()
    {
        GameManager.instance.previousScene=SceneManager.GetActiveScene().name;
        GameManager.instance.saveLoadCheck = false;
        SceneManager.LoadScene("saveload");
    }
    public void QuickSaveSystem()
    {
        saveCoroutine = null;
        saveCoroutine = StartCoroutine(CaptureSaveImage());
        StartCoroutine(WaitForQuickSave());
    }
    IEnumerator WaitForQuickSave()
    {
        while (saveCoroutine != null)
        {
            yield return null;
        }
        byte[] saveImageBytes = File.ReadAllBytes(GameManager.instance.saveImagePath);
        Texture2D saveImageTexture = new Texture2D(2, 2);
        saveImageTexture.LoadImage(saveImageBytes);
        byte[] slotImageBytes = saveImageTexture.EncodeToPNG();
        string saveSlotImageFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "saveSlot");
        if (!Directory.Exists(saveSlotImageFolderPath))
        {
            Directory.CreateDirectory(saveSlotImageFolderPath);
        }
        string saveSlotImagetFileName = "QuickSaveSlot.png";
        string saveSlotImagePath = Path.Combine(saveSlotImageFolderPath, saveSlotImagetFileName);
        GameManager.instance.slotImagePath = saveSlotImagePath;
        File.WriteAllBytes(saveSlotImagePath, slotImageBytes);
        GameManager.instance.quickCheck = true;
        GameManager.instance.DataSaving("QuickSaveSlot.json", $"{System.DateTime.Now:MM월 dd일, HH:mm}");
    }
    public void QuickLoadSystem()
    {
        Quickloadwarningimage.SetActive(true);
    }
    public void QuickLoadClickYes()
    {
        string jsonFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "saveData");
        string jsonFileName = "QuickSaveSlot.json";
        string jsonPath = Path.Combine(jsonFolderPath, jsonFileName);
        SaveData quickSaveData = new SaveData();
        if (File.Exists(jsonPath))
        {
            string jsonString = File.ReadAllText(jsonPath);
            quickSaveData = JsonUtility.FromJson<SaveData>(jsonString);
        }
        GameManager.instance.dayCount = quickSaveData.day;
        GameManager.instance.relationship_level = quickSaveData.relationship_level;
        GameManager.instance.previousScene = "";
        GameManager.instance.printSetting = quickSaveData.printSetting;
        GameManager.instance.bgmSoundvolume = quickSaveData.bgmSoundvolume;
        GameManager.instance.effectSoundvolume = quickSaveData.SoundEffectvolume;
        GameManager.instance.textPrintSpeed = quickSaveData.textPrintSpeed;
        GameManager.instance.selectedHeroine = quickSaveData.selectedHeroine;
        GameManager.instance.dialogCount = quickSaveData.dialogCount;
        GameManager.instance.playerName = quickSaveData.playerName;
        GameManager.instance.backgroundIndex = quickSaveData.backgroundIndex;
        GameManager.instance.messageCount = quickSaveData.messageCount;
        GameManager.instance.leftMessageCount = quickSaveData.leftMessageCount;
        GameManager.instance.userChoice = quickSaveData.userChoice;
        SceneManager.LoadScene(quickSaveData.presentSceneName);
    }
    public void QuickLoadClickNo()
    {
        Quickloadwarningimage.SetActive(false);
    }
    public void AutoSystem()
    {
        if (GameManager.instance.printSetting == 0)
            GameManager.instance.printSetting = 1;
        else
            GameManager.instance.printSetting = 0;
    }
    public void LogOpenSystem()
    {
        GameManager.instance.printSetting = 1;
        Log.GetComponent<RectTransform>().anchoredPosition = new Vector2(3.1199e-08f, -4.4107e-06f);
    }
    public void LogCloseSystem()
    {
        Log.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920f, -4.4107e-06f);
    }
    public void test()
    {
        SceneManager.LoadScene("ep2_ru");
    }
}
