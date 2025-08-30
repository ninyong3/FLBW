using NUnit.Framework;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class BubbleIds
{
    public const string Jin    = "bubble_jin";
    public const string Freyja = "bubble_freyja";
    public const string Ru     = "bubble_ru";

    public static string Normalize(string id) => (id ?? "").Trim().ToLowerInvariant();

    public static string FromIndex(int idx)
    {
        switch (idx)
        {
            case 1: return Jin;     // 진예인
            case 2: return Freyja;  // 프레이야
            case 3: return Ru;      // 루
            default: return Jin;
        }
    }
}

public class GameManager : MonoBehaviour
{
    public int dayCount = 0; // 현재 day 수
    public static GameManager instance; 
    public int relationship_level=0; // 호감도
    public string previousScene; // 이전 씬
    public int printSetting=1; // 출력 설정 0->자동, 1->직접
    public double bgmSoundvolume = 50f; // 배경음악 음량
    public double effectSoundvolume = 50f; // 효과음 음량
    public double textPrintSpeed = 50f; // 텍스트 출력 속도

    // ★ 전역 선택 히로인 인덱스 (쓰기: 선택/MapGate만, 나머지는 읽기 전용)
    public int selectedHeroine; // 1=진예인, 2=프레이야, 3=루

    public bool saveLoadCheck;
    public string saveImagePath;
    public string saveDataPath;
    public string saveJsonPath;
    public bool quickCheck;
    public string slotImagePath;
    public int dialogCount=1;
    public string playerName;
    public int backgroundIndex;
    public int messageCount=1;
    public int leftMessageCount = 0;
    public List<bool> messageCountCheckList=new List<bool>();
    public string userChoice;
    public bool IsLoading;
    public PersistentData PersistentData;

    // === 디버그: selectedHeroine 변화 감지용
    private int _prevSelectedHeroine = 0;

    void Start()
    {
        string saveJsonFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "persistentSaveData");
        string persistentSaveJsonPath = Path.Combine(saveJsonFolderPath, "persistentData.json");
        if (!Directory.Exists(saveJsonFolderPath))
        {
            Directory.CreateDirectory(saveJsonFolderPath);
        }
        if (!File.Exists(persistentSaveJsonPath))
        {
            PersistentData persistentData = new PersistentData();
            string saveToJsonData=JsonConvert.SerializeObject(persistentData, Formatting.Indented);
            File.WriteAllText(persistentSaveJsonPath, saveToJsonData);
        }
        else
        {
            string jsonString=File.ReadAllText(persistentSaveJsonPath);
            PersistentData=JsonConvert.DeserializeObject<PersistentData>(jsonString);
        }

        Debug.Log($"[GM] Start: selectedHeroine={selectedHeroine}, dayCount={dayCount}");
        _prevSelectedHeroine = selectedHeroine;
    }

    void Update()
    {
        // 디버그: 전역 인덱스 외부 변경 추적
        if (_prevSelectedHeroine != selectedHeroine)
        {
            Debug.Log($"[GM] selectedHeroine changed {_prevSelectedHeroine} -> {selectedHeroine}");
            _prevSelectedHeroine = selectedHeroine;
        }

        if (dayCount != 0)
        {
            if (dayCount % 5 == 0 && messageCountCheckList[dayCount / 5 - 1] == false)
            {
                leftMessageCount++;
                messageCountCheckList[dayCount / 5 - 1] = true;
            }
        }
        if (relationship_level < -1)
        {
            dialogCount = 1;
            relationship_level = 0;
            SceneManager.LoadScene("badending");
        }
    }

    void Awake()
    {
        if(GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        if(instance != this && instance != null) // 타이틀로 돌아왔을 시 Gamemanager 오브젝트 중복 생성 방지
        {
            Debug.Log("[GM] Duplicate GameManager detected. Destroy this.");
            Destroy(gameObject);
        }
        else
        {
            instance= this;
            DontDestroyOnLoad(gameObject); // 씬 간 공유
            Debug.Log("[GM] Awake & DontDestroyOnLoad");
        }
    }

    public void DataSaving(string saveJsonFileName, string saveTime)
    {
        // ... (원본 저장 로직 동일)
        // 저장 직전 디버그
        Debug.Log($"[GM] DataSaving: day={dayCount}, selHeroine={selectedHeroine}, scene={(quickCheck?SceneManager.GetActiveScene().name:previousScene)}");
        SaveData saveData = new SaveData();
        saveData.day = dayCount;
        saveData.relationship_level = relationship_level;
        if (quickCheck == false)
            saveData.presentSceneName = previousScene;
        else
            saveData.presentSceneName = SceneManager.GetActiveScene().name;
        saveData.slotImagePath = slotImagePath;
        saveData.printSetting = printSetting;
        saveData.bgmSoundvolume = bgmSoundvolume;
        saveData.SoundEffectvolume = effectSoundvolume;
        saveData.textPrintSpeed = textPrintSpeed;
        saveData.selectedHeroine=selectedHeroine;
        saveData.saveTime = saveTime;
        saveData.saveCheck = true;
        saveData.dialogCount = dialogCount;
        saveData.playerName = playerName;
        saveData.backgroundIndex = backgroundIndex;
        saveData.messageCount=messageCount;
        saveData.leftMessageCount=leftMessageCount;
        saveData.userChoice=userChoice;
        string saveToJsonData = JsonUtility.ToJson(saveData, true);
        string saveJsonFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "saveData");
        if (!Directory.Exists(saveJsonFolderPath))
        {
            Directory.CreateDirectory(saveJsonFolderPath);
        }
        saveJsonPath = Path.Combine(saveJsonFolderPath, saveJsonFileName);
        File.WriteAllText(saveJsonPath, saveToJsonData);
    }

    public void ClearCheck(int checkIndex, int index1, int index2)
    {
        if (checkIndex == 1)
            PersistentData.episodeClearCheck[index1, index2] = true;
        else
            PersistentData.endingClearCheck[index1] = true;
        string saveToJsonData = JsonConvert.SerializeObject(PersistentData, Formatting.Indented);
        string saveJsonFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "persistentSaveData");
        if (!Directory.Exists(saveJsonFolderPath))
        {
            Directory.CreateDirectory(saveJsonFolderPath);
        }
        string persistentSaveJsonPath = Path.Combine(saveJsonFolderPath, "persistentData.json");
        File.WriteAllText(persistentSaveJsonPath, saveToJsonData);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsLoading = false;
        Debug.Log($"[GM] OnSceneLoaded: '{scene.name}', selectedHeroine={selectedHeroine}");
    }
}
