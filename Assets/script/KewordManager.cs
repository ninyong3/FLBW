using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class KewordManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI jinyeinSelect; // 캐릭터 선택 부분에 있는 텍스트들
    [SerializeField] TextMeshProUGUI freyjaSelect;
    [SerializeField] TextMeshProUGUI ruSelect;
    [SerializeField] List<TextMeshProUGUI> keword=new List<TextMeshProUGUI>(); // 키워드 내용 리스트
    [SerializeField] Image characterImage; // 키워드와 같이 출력할 캐릭터 이미지
    [SerializeField] List<Sprite> characterImageList = new List<Sprite>(); // 캐릭터 스프라이트를 넣을 리스트
    void Start()
    {
        jinyeinSelect.text = "???"; // 기본값은 ???, 캐릭터 선택값 받아와서 바꿀 예정
        freyjaSelect.text = "???";
        ruSelect.text = "???";
        if (GameManager.instance.selectedHeroine == 1)
            jinyeinSelect.text = "진예인";
        else if (GameManager.instance.selectedHeroine == 2)
            freyjaSelect.text = "프레이야";
        else if (GameManager.instance.selectedHeroine == 3)
            ruSelect.text = "루";
        Color color = characterImage.color; // 캐릭터 이미지 투명화
        color.a = 0;
        characterImage.color = color;
    }


    void Update()
    {
        
    }
    public void YeinKewordSelect() // 진예인 키워드 함수
    {
        if (GameManager.instance.selectedHeroine == 1)
        {
            Color color = characterImage.color; // 캐릭터 이미지 불투명화
            color.a = 1;
            characterImage.color = color;
            keword[0].text = "이름: 진예인";
            keword[1].text = "성별: 여";
            keword[2].text = "종족: 인간";
            keword[3].text = "직업: 대학생(의학실습생)";
            keword[4].text = "상징색: 노랑";
            keword[5].text = "천재성: 의학에 대한 재능";
            if (GameManager.instance.relationship_level > 1)
                keword[6].text = "비밀: 독실한 신자다.";
            else
                keword[6].text = "???";
            if(GameManager.instance.relationship_level > 2)
                keword[7].text = "비밀 2: 취미를 더 가져볼까 고민 중이다.";
            else
                keword[7].text="???";
            if(GameManager.instance.relationship_level > 3)
                keword[8].text = "과거사: 루, 프레이야와는 고등학교 음악 동아리에서 알게 된 사이. 단순한 호기심으로 들어갔다.";
            else
                keword[8].text="???";
            characterImage.sprite = characterImageList[0]; //0번 캐릭터 이미지->진예인
        }
    }
    public void FreyjaKewordSelect() // 프레이야 키워드 함수
    {
        if (GameManager.instance.selectedHeroine == 2)
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
            if(GameManager.instance.relationship_level >1)
                keword[6].text = "비밀: 사실은 겁이 많다.";
            else
                keword[6].text="???";
            if(GameManager.instance.relationship_level > 2)
                keword[7].text = "비밀 2: 과거에 귀신과 만나서 좋지 않은 일을 당했다. 아직도 트라우마가 남아있다.";
            else
                keword[7].text="???";
            if(GameManager.instance.relationship_level > 3)
                keword[8].text = "과거사: 예인, 루와는 고등학교 음악 동아리에서 알게 된 사이. 음악을 좋아해서 들어갔다.";
            else
                keword[8].text="???";
            characterImage.sprite = characterImageList[1]; //1번 캐릭터 이미지->프레이야
        }
    }
    public void RuKewordSelect() // 루 키워드 함수
    {
        if (GameManager.instance.selectedHeroine == 3)
        {
            Color color = characterImage.color;
            color.a = 1;
            characterImage.color = color;
            keword[0].text = "이름: 루";
            keword[1].text = "성별: 여";
            keword[2].text = "종족: 수인(상어)";
            keword[3].text = "직업: 편의점 알바생";
            keword[4].text = "상징색: 분홍";
            keword[5].text = "천재성: 타인의 천재성을 알아볼 수\n있는 재능";
            if(GameManager.instance.relationship_level > 1)
                keword[6].text = "비밀: 의외로 자존감이 낮은 타입";
            else
                keword[6].text="???";
            if(GameManager.instance.relationship_level > 2)
                keword[7].text = "비밀 2: 바다 상어 수인이기에 민물 상어 수인과는 사이가 좋지 않다.";
            else
                keword[7].text="???";
            if(GameManager.instance.relationship_level > 3)
                keword[8].text = "과거사: 예인, 프레이야와는 고등학교 음악 동아리에서 알게 된 사이. 음악을 하는 친구들이 멋있어 보여서 들어갔다.";
            else
                keword[8].text="???";
            characterImage.sprite = characterImageList[2]; //2번 캐릭터 이미지->루
        }
    }
    public void KewordReturnScene() // 이전씬으로 돌아가기 위한 함수
    {
        SceneManager.LoadScene(GameManager.instance.previousScene);
    }
}
