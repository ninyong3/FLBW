using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MessengerManager : MonoBehaviour
{
    [SerializeField] GameObject messenger;
    Vector2 messengerPosition;
    [SerializeField] RectTransform chatContentRect; // 메신저의 콘텐츠
    [SerializeField] TextMeshProUGUI characterName; // 현재 선택한 캐릭터의 이름
    void Start()
    {
        
    }

    void Update()
    {
       
    }
    public void MessengerShow() // 메신저 보이기를 위한 함수
    {
        StartCoroutine(ShowMessenger());
    }
    public void MessengerHide() // 메신저 숨기기를 위한 함수
    {
        StartCoroutine(HideMessenger());
    }
    IEnumerator ShowMessenger() // 코루틴 작동 시 좌측에서 화면안으로 이동
    {
        messengerPosition = messenger.GetComponent<RectTransform>().anchoredPosition;
        while (messengerPosition.x <= -590)
        {
            messengerPosition.x += 10;
            messenger.GetComponent<RectTransform>().anchoredPosition = messengerPosition;
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator HideMessenger() // 코루틴 작동 시 화면에서 화면 밖으로 좌측으로 이동
    {
        messengerPosition = messenger.GetComponent<RectTransform>().anchoredPosition;
        while (messengerPosition.x >= -1338.5)
        {
            messengerPosition.x -= 10;
            messenger.GetComponent<RectTransform>().anchoredPosition = messengerPosition;
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void ChatToNext() // 다음 메세지로 넘기기 위한 함수
    {
        GameObject heroineChatPrefab = Resources.Load<GameObject>("Leftmessagecontrol"); // 프리펩 받아옴
        GameObject heroineChatGo = GameObject.Instantiate(heroineChatPrefab, chatContentRect); // 자식으로 만들기&클론 생성
        GameObject PlayerChatPrefab = Resources.Load<GameObject>("Rightmessagecontrol");
        GameObject PlayerChatGo = GameObject.Instantiate(PlayerChatPrefab, chatContentRect);
        Transform chatText = PlayerChatGo.transform.Find("Chatimage").Find("Chattext"); // 프리펩의 텍스트 찾기
        chatText.GetComponent<TextMeshProUGUI>().text = "테스트"; // 텍스트 내용 변환
        Transform characterProfileImage = heroineChatGo.transform.Find("CharacterProfile"); // 프리펩의 캐릭터 프로필 이미지 찾기
      //  characterProfileImage.gameObject.SetActive(false); // 차후 이용 예정(프로필이미지 숨기기)
    }
    public void SelectJinyeinMessage() // 진예인 메세지 확인 시 작동하는 함수
    {
        characterName.text = "진예인";
    }
    public void SelectFreyjaMessage() // 프레이야 메세지 확인 시 작동하는 함수
    {
        characterName.text = "프레이야 레가토";
    }
    public void SelectRuMessage() // 루 메세지 확인 시 작동하는 함수
    {
        characterName.text = "루";
    }
}
