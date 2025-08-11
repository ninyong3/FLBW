using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ConfigManager : MonoBehaviour
{
    [SerializeField] Slider bgmSoundSlider; 
    [SerializeField] Slider effectSoundSlider;
    [SerializeField] Slider printSpeedSlider;
    void Start()
    {
        bgmSoundSlider.value = 50f;
        effectSoundSlider.value = 50f;
        printSpeedSlider.value = 50f;
    }

    void Update()
    {
        GameManager.instance.bgmSoundvolume = bgmSoundSlider.value; // 배경음악 음량값에 슬라이더 값 전달
        GameManager.instance.effectSoundvolume = effectSoundSlider.value; // 효과음~~
        GameManager.instance.textPrintSpeed= printSpeedSlider.value; // 출력 속도~~
    }
    public void PrintAutoSetting() // 자동출력으로 바꾸는 함수
    {
        GameManager.instance.printSetting = 0;
    }
    public void PrintTouchSetting() // 직접출력으로 바꾸는 함수
    {
        GameManager.instance.printSetting = 1;
    }
    public void ConfigReturnScene() // 이전씬으로 돌아가기 위한 함수
    {
        SceneManager.LoadScene(GameManager.instance.previousScene);
    }
}
