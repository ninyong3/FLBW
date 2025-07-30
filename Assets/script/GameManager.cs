using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI day;
    public int dayCount = 1;
    public static GameManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        day.text = "Day 1";
    }

    // Update is called once per frame
    void Update()
    {
        day.text="Day "+dayCount.ToString();
    }
    void Awake()
    {
        if(GameManager.instance == null)
        {
            GameManager.instance = this;
        }
    }
}
