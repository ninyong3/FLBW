using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI saveLoadTextKor;
    [SerializeField] TextMeshProUGUI saveLoadTextEng;
    [SerializeField] GameObject quickSaveSlot;
    [SerializeField] TextMeshProUGUI pageText;
    [SerializeField] int pageIndex;
    [SerializeField] GameObject previousPageButton;
    [SerializeField] GameObject nextPageButton;
    [SerializeField] GameObject quickSaveDefender;
    [SerializeField] List<GameObject> slotList;
    public List<SaveData> saveData;
    public List<TextMeshProUGUI> slotTextList;
    [SerializeField] GameObject Rewritewarningimage;
    GameObject selectedSlot;
    int slotIndex;
    void Start()
    {
        if(GameManager.instance.saveLoadCheck)
        {
            saveLoadTextKor.text = "저장하기";
            saveLoadTextEng.text = "save";
            quickSaveSlot.SetActive(false);
        }
        else
        {
            saveLoadTextKor.text = "불러오기";
            saveLoadTextEng.text = "load";
            quickSaveDefender.SetActive(false);
        }
        for (int i = 6; i < 18; i++)
            slotList[i].SetActive(false);
        pageText.text = "Page 1";
        pageIndex = 1;
        previousPageButton.SetActive(false);
        Rewritewarningimage.SetActive(false);
        LoadData();
    }
    void Update()
    {
        if (pageIndex == 1)
        {
            previousPageButton.SetActive(false);
            nextPageButton.SetActive(true);
            for (int i = 0; i < 6; i++)
                slotList[i].SetActive(true);
            for (int i = 6; i < 18; i++)
                slotList[i].SetActive(false);
        }
        if (pageIndex == 2)
        {
            previousPageButton.SetActive(true);
            nextPageButton.SetActive(true);
            for (int i = 6; i < 12; i++)
                slotList[i].SetActive(true);
            for (int i = 0; i < 6; i++)
                slotList[i].SetActive(false);
            for (int i = 12; i < 18; i++)
                slotList[i].SetActive(false);
        }
        if (pageIndex == 3)
        {
            nextPageButton.SetActive(false);
            previousPageButton.SetActive(true);
            for (int i = 12; i < 18; i++)
                slotList[i].SetActive(true);
            for (int i = 0; i < 12; i++)
                slotList[i].SetActive(false);
        }
        quickSaveDefender.SetActive(false);
        if(GameManager.instance.saveLoadCheck && pageIndex == 1)
        {
            quickSaveDefender.SetActive(true);
            quickSaveSlot.SetActive(false);
        }
    }
    public void SaveLoadReturnScene()
    {
        SceneManager.LoadScene(GameManager.instance.previousScene);
    }
    public void GoToNextPage()
    {
        pageIndex++;
        pageText.text="Page "+pageIndex.ToString();
    }
    public void GoToPreviousPage()
    {
        pageIndex--;
        pageText.text = "Page " + pageIndex.ToString();
    }
    public void SlotSelect()
    { 
        selectedSlot = EventSystem.current.currentSelectedGameObject;
        if (selectedSlot.name == "Quicksaveslot")
            slotIndex = 0;
        else
        {
            string slotIndexString = selectedSlot.name.Remove(0, 8);
            slotIndex = int.Parse(slotIndexString)-1;
        }
        if (GameManager.instance.saveLoadCheck)
        {
            if (saveData[slotIndex].saveCheck == false)
            {
                SaveData();
            }
            else
            {
                Rewritewarningimage.SetActive(true);
            }
        }
        else if(saveData[slotIndex].saveCheck == true)
        {
            GameManager.instance.dayCount = saveData[slotIndex].day;
            GameManager.instance.relationship_level=saveData[slotIndex].relationship_level;
            GameManager.instance.previousScene = "";
            GameManager.instance.printSetting= saveData[slotIndex].printSetting;
            GameManager.instance.bgmSoundvolume = saveData[slotIndex].bgmSoundvolume;
            GameManager.instance.effectSoundvolume = saveData[slotIndex].SoundEffectvolume;
            GameManager.instance.textPrintSpeed= saveData[slotIndex].textPrintSpeed;
            GameManager.instance.selectedHeroine=saveData[slotIndex].selectedHeroine;
            SceneManager.LoadScene(saveData[slotIndex].presentSceneName);
        }
    }
    void LoadData()
    {
        string jsonFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "saveData");
        for (int i = 0; i < 18; i++)
        {
            string jsonFileName = slotList[i].name + ".json";
            string jsonPath = Path.Combine(jsonFolderPath, jsonFileName);
            if (File.Exists(jsonPath))
            {
                string jsonString = File.ReadAllText(jsonPath);
                saveData[i] = JsonUtility.FromJson<SaveData>(jsonString);
                byte[] savedImageBytes = File.ReadAllBytes(saveData[i].slotImagePath);
                Texture2D saveSlotImageTexture = new Texture2D(2, 2);
                saveSlotImageTexture.LoadImage(savedImageBytes);
                Rect savedImageRect = new Rect(0, 0, saveSlotImageTexture.width, saveSlotImageTexture.height);
                Sprite savedImageSprite = Sprite.Create(saveSlotImageTexture, savedImageRect, new Vector2(0.5f, 0.5f));
                slotList[i].GetComponent<Image>().sprite = savedImageSprite;
                slotList[i].GetComponent<Image>().color = Color.white;
                slotTextList[i].text = saveData[i].saveTime;
            }
        }
    }
    void SaveData()
    {
        byte[] saveImageBytes = File.ReadAllBytes(GameManager.instance.saveImagePath);
        Texture2D saveImageTexture = new Texture2D(2, 2);
        saveImageTexture.LoadImage(saveImageBytes);
        Rect saveImageRect = new Rect(0, 0, saveImageTexture.width, saveImageTexture.height);
        Sprite saveImageSprite = Sprite.Create(saveImageTexture, saveImageRect, new Vector2(0.5f, 0.5f));
        selectedSlot.GetComponent<Image>().sprite = saveImageSprite;
        selectedSlot.GetComponent<Image>().color = Color.white;
        byte[] slotImageBytes = saveImageTexture.EncodeToPNG();
        string saveSlotImageFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "saveSlot");
        if (!Directory.Exists(saveSlotImageFolderPath))
        {
            Directory.CreateDirectory(saveSlotImageFolderPath);
        }
        string saveSlotImagetFileName = selectedSlot.name + ".png";
        string saveSlotImagePath = Path.Combine(saveSlotImageFolderPath, saveSlotImagetFileName);
        GameManager.instance.slotImagePath = saveSlotImagePath;
        File.WriteAllBytes(saveSlotImagePath, slotImageBytes);
        slotTextList[slotIndex].text = $"{System.DateTime.Now:MM월 dd일, HH:mm}";
        GameManager.instance.DataSaving(selectedSlot.name + ".json", $"{System.DateTime.Now:MM월 dd일, HH:mm}");
    }
    public void RewriteClickYes()
    {
        SaveData();
        Rewritewarningimage.SetActive(false);
    }
    public void RewriteClickNo()
    {
        Rewritewarningimage.SetActive(false);
    }
}
