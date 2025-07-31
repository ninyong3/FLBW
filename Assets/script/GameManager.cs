using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public int dayCount = 1;
    public static GameManager instance;
    public int relationship_level=0;
    public string previousScene;
    public int printSetting=1;
    public double bgmSoundvolume = 50f;
    public double effectSoundvolume = 50f;
    public double textPrintSpeed = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {
        if(GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        if(instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance= this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
