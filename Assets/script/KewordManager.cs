using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class KewordManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI jinyeinSelect;
    [SerializeField] TextMeshProUGUI freyjaSelect;
    [SerializeField] TextMeshProUGUI ruSelect;
    [SerializeField] List<TextMeshProUGUI> keword=new List<TextMeshProUGUI>();
    [SerializeField] Image characterImage;
    [SerializeField] List<Sprite> characterImageList = new List<Sprite>();
    void Start()
    {
        jinyeinSelect.text = "???";
        freyjaSelect.text = "???";
        Color color=characterImage.color;
        color.a = 0;
        characterImage.color = color;
    }


    void Update()
    {
        
    }
    public void YeinKewordSelect()
    {
        Color color = characterImage.color;
        color.a = 1;
        characterImage.color = color;
        keword[0].text = "이름: 진예인";
        keword[1].text = "성별: 여";
        keword[2].text = "종족: 인간";
        keword[3].text = "직업: 대학생(의학실습생)";
        keword[4].text = "상징색: 노랑";
        keword[5].text = "천재성: 의학에 대한 재능";
        keword[6].text = "비밀: 독실한 신자다.";
        characterImage.sprite = characterImageList[0];
    }
    public void FreyjaKewordSelect()
    {
        Color color = characterImage.color;
        color.a = 1;
        characterImage.color = color;
        keword[0].text = "이름: 프레이야 레가토";
        keword[1].text = "성별: 여";
        keword[2].text = "종족: 엘프";
        keword[3].text = "직업: 바이올리니스트";
        keword[4].text = "상징색: 파랑";
        keword[5].text = "천재성: 바이올린 연주에 대한 재능";
        keword[6].text = "비밀: 사실은 겁이 많다.";
        characterImage.sprite = characterImageList[1];
    }
    public void RuKewordSelect()
    {
        Color color= characterImage.color;
        color.a = 1;
        characterImage.color = color;
        keword[0].text = "이름: 루";
        keword[1].text = "성별: 여";
        keword[2].text = "종족: 수인(상어)";
        keword[3].text = "직업: 편의점 알바생";
        keword[4].text = "상징색: 분홍";
        keword[5].text = "천재성: 타인의 천재성을 알아볼 수\n있는 재능";
        keword[6].text = "비밀: 의외로 자존감이 낮은 타입";
        characterImage.sprite = characterImageList[2];
    }
    public void KewordReturnScene()
    {
        SceneManager.LoadScene(GameManager.instance.previousScene);
    }
}
