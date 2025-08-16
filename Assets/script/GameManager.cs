using NUnit.Framework;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public int dayCount = 1; // 현재 day 수
    public static GameManager instance; 
    public int relationship_level=0; // 호감도
    public string previousScene; // 이전 씬
    public int printSetting=1; // 출력 설정 0->자동, 1->직접
    public double bgmSoundvolume = 50f; // 배경음악 음량
    public double effectSoundvolume = 50f; // 효과음 음량
    public double textPrintSpeed = 50f; // 텍스트 출력 속도
    public int selectedHeroine; // 선택된 여주인공 인덱스 1->진예인, 2->프레이야 레가토, 3->루
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
    void Start()
    {
       
    }
    void Update()
    {
        if(dayCount%5 == 0 && messageCountCheckList[dayCount/5-1] == false)
        {
            leftMessageCount++;
            messageCountCheckList[dayCount/5-1]=true;
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
            Destroy(gameObject);
        }
        else
        {
            instance= this;
            DontDestroyOnLoad(gameObject); // 씬 간 Gamemanager 오브젝트 공유 가능하게 하기 위한 파괴 금지
        }
    }
    public void DataSaving(string saveJsonFileName, string saveTime)
    {
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
        string saveToJsonData = JsonUtility.ToJson(saveData, true);
        string saveJsonFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "saveData");
        if (!Directory.Exists(saveJsonFolderPath))
        {
            Directory.CreateDirectory(saveJsonFolderPath);
        }
        saveJsonPath = Path.Combine(saveJsonFolderPath, saveJsonFileName);
        File.WriteAllText(saveJsonPath, saveToJsonData);
    }
}
