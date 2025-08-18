using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExtraManager : MonoBehaviour
{
    PersistentData persistentData;
    [SerializeField] List<Sprite> ExtraSprites;
    [SerializeField] List<GameObject> ExtraObjects;
    [SerializeField] List<GameObject> ExtraScenes;
    void Start()
    {
        persistentData = new PersistentData();
        string jsonFolderPath = Path.Combine(Application.persistentDataPath, "persistentSaveData");
        string jsonPath = Path.Combine(jsonFolderPath, "persistentData.json");
        if(Directory.Exists(jsonFolderPath) && File.Exists(jsonPath))
        {
            string jsonString=File.ReadAllText(jsonPath);
            persistentData=JsonConvert.DeserializeObject<PersistentData>(jsonString);
        }
        if (persistentData.episodeClearCheck[0, 0] || persistentData.episodeClearCheck[1, 0] || persistentData.episodeClearCheck[2, 0])
            ExtraObjects[0].GetComponent<Image>().sprite = ExtraSprites[0];
        int cnt=0;
        for (int i = 0;i<3;i++)
        { 
            for(int j=1;j<6;j++)
            {
                cnt++;
                if (persistentData.episodeClearCheck[i , j])
                {
                    ExtraObjects[cnt].GetComponent<Image>().sprite = ExtraSprites[cnt];
                }
            }
        }
        for(int i=0;i<5;i++)
        {
            cnt++;
            if(persistentData.endingClearCheck[i])
                ExtraObjects[cnt].GetComponent <Image>().sprite = ExtraSprites[cnt];
        }
      //  if (persistentData.endingClearCheck[0])
         
    }
    void Update()
    {
        
    }
    public void EpisodeShow()
    {
        ExtraScenes[0].SetActive(true);
        ExtraScenes[1].SetActive(false);
        ExtraScenes[2].SetActive(false);
        ExtraScenes[3].SetActive(false);
    }
    public void EndingShow()
    {
        ExtraScenes[0].SetActive(false);
        ExtraScenes[1].SetActive(true);
        ExtraScenes[2].SetActive(false);
        ExtraScenes[3].SetActive(false);
    }
    public void CGShow()
    {
        ExtraScenes[0].SetActive(false);
        ExtraScenes[1].SetActive(false);
        ExtraScenes[2].SetActive(true);
        ExtraScenes[3].SetActive(false);
    }
    public void InformationShow()
    {
        ExtraScenes[0].SetActive(false);
        ExtraScenes[1].SetActive(false);
        ExtraScenes[2].SetActive(false);
        ExtraScenes[3].SetActive(true);
    }
    public void GoToNormalEnd()
    {
        if (persistentData.endingClearCheck[3])
            SceneManager.LoadScene("normalending");
    }
    public void GoToBadEnd()
    {
        if (persistentData.endingClearCheck[4])
            SceneManager.LoadScene("badending");
    }
    public void GoToJinYeinHappyEnd()
    {
        if (persistentData.endingClearCheck[0])
            SceneManager.LoadScene("happyending_jinyein");
    }
    public void GoToFreyjaHappyEnd()
    {
        if (persistentData.endingClearCheck[1])
            SceneManager.LoadScene("happyending_freyja");
    }
    public void GoToRuHappyEnd()
    {
        if (persistentData.endingClearCheck[2])
            SceneManager.LoadScene("happyending_ru");
    }

}
