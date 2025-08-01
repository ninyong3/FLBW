using UnityEngine;
using TMPro;
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
    void Start()
    {
       
    }
    void Update()
    {
        
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
}
