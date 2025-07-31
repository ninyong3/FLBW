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
        
    }

    void Update()
    {
        GameManager.instance.bgmSoundvolume = bgmSoundSlider.value;
        GameManager.instance.effectSoundvolume = effectSoundSlider.value;
        GameManager.instance.textPrintSpeed= printSpeedSlider.value;
    }
    public void PrintAutoSetting()
    {
        GameManager.instance.printSetting = 0;
    }
    public void PrintTouchSetting()
    {
        GameManager.instance.printSetting = 1;
    }
    public void ConfigReturnScene()
    {
        SceneManager.LoadScene(GameManager.instance.previousScene);
    }
}
