using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ExtraManager : MonoBehaviour
{
    PersistentData persistentData;
    [SerializeField] List<Sprite> ExtraSprites;
    [SerializeField] List<GameObject> ExtraObjects;
    [SerializeField] List<GameObject> ExtraScenes;
    int episodepage = 1;
    int cgpage = 1;
    [SerializeField] TextMeshProUGUI pagetext;
    [SerializeField] GameObject nextbutton;
    [SerializeField] GameObject prevbutton;
    [SerializeField] GameObject nextbutton2;
    [SerializeField] GameObject prevbutton2;
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
        if (persistentData.endingClearCheck[0])
        {
            for (int i = 0; i < 10; i++)
            {
                cnt++;
                ExtraObjects[cnt].GetComponent<Image>().sprite = ExtraSprites[cnt];
            }
        }
        else
        {
            for (int i = 0; i < 10; i++)
                cnt++;
        }
        if (persistentData.endingClearCheck[2])
        {
            for (int i = 0; i < 10; i++)
            {
                cnt++;
                ExtraObjects[cnt].GetComponent<Image>().sprite = ExtraSprites[cnt];
            }
        }
        else
        {
            for (int i = 0; i < 10; i++)
                cnt++;
        }
        if (persistentData.endingClearCheck[1])
        {
            for(int i=0;i<5;i++)
            {
                cnt++;
                ExtraObjects[cnt].GetComponent<Image>().sprite= ExtraSprites[cnt];
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
                cnt++;
        }
        cnt++;
        if (persistentData.episodeClearCheck[0, 5])
            ExtraObjects[cnt].GetComponent<Image>().sprite = ExtraSprites[cnt];
        cnt++;
        if (persistentData.episodeClearCheck[1, 5])
            ExtraObjects[cnt].GetComponent<Image>().sprite = ExtraSprites[cnt];
        cnt++;
        if (persistentData.episodeClearCheck[2, 5])
            ExtraObjects[cnt].GetComponent<Image>().sprite = ExtraSprites[cnt];
        cnt++;
        if (persistentData.endingClearCheck[0])
            ExtraObjects[cnt].GetComponent<Image>().sprite = ExtraSprites[cnt];
        cnt++;
        if (persistentData.endingClearCheck[1])
            ExtraObjects[cnt].GetComponent<Image>().sprite = ExtraSprites[cnt];
        cnt++;
        if (persistentData.endingClearCheck[2])
            ExtraObjects[cnt].GetComponent<Image>().sprite = ExtraSprites[cnt];
        for (int i = 0; i < 4; i++)
            ExtraScenes[i].SetActive(false);
    }
    void Update()
    {
        pagetext.text = "Page " + episodepage;
        if (episodepage == 1)
        {
            prevbutton.SetActive(false);
            nextbutton.SetActive(true);
            for(int i=0;i<4;i++)
                ExtraObjects[i].SetActive(true);
            for (int i = 4; i < 16; i++)
                ExtraObjects[i].SetActive(false);
        }
        else if (episodepage == 2)
        {
            prevbutton.SetActive(true);
            nextbutton.SetActive(true);
            for (int i = 4; i < 8; i++)
                ExtraObjects[i].SetActive(true);
            for (int i = 8; i < 16; i++)
                ExtraObjects[i].SetActive(false);
            for(int i=0;i<4;i++)
                ExtraObjects[i].SetActive(false);
        }
        else if (episodepage == 3)
        {
            prevbutton.SetActive(true);
            nextbutton.SetActive(true);
            for (int i = 8; i < 12; i++)
                ExtraObjects[i].SetActive(true);
            for (int i = 12; i < 16; i++)
                ExtraObjects[i].SetActive(false);
            for (int i = 0; i < 8; i++)
                ExtraObjects[i].SetActive(false);
        }
        else if (episodepage == 4)
        {
            prevbutton.SetActive(true);
            nextbutton.SetActive(false);
            for (int i = 12; i < 16; i++)
                ExtraObjects[i].SetActive(true);
            for (int i = 0; i < 12; i++)
                ExtraObjects[i].SetActive(false);
        }
        if (cgpage == 1)
        {
            prevbutton2.SetActive(false);
            nextbutton2.SetActive(true);
            for (int i = 21; i < 31; i++)
                ExtraObjects[i].SetActive(true);
            for (int i = 31; i < 52; i++)
                ExtraObjects[i].SetActive(false);
        }
        else if (cgpage == 2)
        {
            prevbutton2.SetActive(true);
            nextbutton2.SetActive(true);
            for (int i = 31; i < 41; i++)
                ExtraObjects[i].SetActive(true);
            for (int i = 21; i < 31; i++)
                ExtraObjects[i].SetActive(false);
            for (int i = 41; i < 52; i++)
                ExtraObjects[i].SetActive(false);
        }
        else if (cgpage == 3)
        {
            prevbutton2.SetActive(true);
            nextbutton2.SetActive(true);
            for (int i = 41; i < 46; i++)
                ExtraObjects[i].SetActive(true);
            for (int i = 21; i < 41; i++)
                ExtraObjects[i].SetActive(false);
            for (int i = 46; i < 52; i++)
                ExtraObjects[i].SetActive(false);
        }
        else if (cgpage == 4)
        {
            prevbutton2.SetActive(true);
            nextbutton2.SetActive(false);
            for (int i = 21; i < 46; i++)
                ExtraObjects[i].SetActive(false);
            for (int i = 46; i < 52; i++)
                ExtraObjects[i].SetActive(true);
        }
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
    public void ExtraClose()
    {
        SceneManager.LoadScene("title");
    } 
    public void NextPage()
    {
        episodepage++;
    }
    public void PreviousPage()
    {
        episodepage--;
    }
    public void NextPage2()
    {
        cgpage++;
    }
    public void PreviousPage2()
    {
        cgpage--;
    }

}
