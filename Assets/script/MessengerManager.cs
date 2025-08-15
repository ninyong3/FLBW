using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class MessengerManager : MonoBehaviour
{
    [SerializeField] GameObject messenger;
    Vector2 messengerPosition;
    [SerializeField] RectTransform chatContentRect; // 메신저의 콘텐츠
    [SerializeField] TextMeshProUGUI characterName; // 현재 선택한 캐릭터의 이름
    int selectedHeroineIndex=1;
    [SerializeField] List<Sprite> characterProfileImageList;
    int messagecnt;
    bool clickFlag = false;
    void Start()
    {
        messagecnt=GameManager.instance.messageCount;
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
        clickFlag = true;
    }
    public void SelectJinyeinMessage() // 진예인 메세지 확인 시 작동하는 함수
    {
        characterName.text = "진예인";
        selectedHeroineIndex = 1;
        StartCoroutine(SpawnMessage());

    }
    public void SelectFreyjaMessage() // 프레이야 메세지 확인 시 작동하는 함수
    {
        characterName.text = "프레이야 레가토";
        selectedHeroineIndex = 2;
        StartCoroutine(SpawnMessage());
    }
    public void SelectRuMessage() // 루 메세지 확인 시 작동하는 함수
    {
        characterName.text = "루";
        selectedHeroineIndex = 3;
        StartCoroutine(SpawnMessage());
    }
    IEnumerator SpawnMessage()
    {
        MessageParser messageParser = FindFirstObjectByType<MessageParser>();
        MessageData tempMessage;
        if (selectedHeroineIndex == 1)
        {
            while (messageParser.jinyeinMessageDic.TryGetValue(messagecnt, out tempMessage))
            {
                Debug.Log(messagecnt);
                if (tempMessage.name != "Player")
                {
                    GameObject heroineChatPrefab = Resources.Load<GameObject>("Leftmessagecontrol"); // 프리펩 받아옴
                    GameObject heroineChatGo = GameObject.Instantiate(heroineChatPrefab, chatContentRect); // 자식으로 만들기&클론 생성
                    Transform chatText = heroineChatGo.transform.Find("Chatimage").Find("Chattext"); // 프리펩의 텍스트 찾기
                    chatText.GetComponent<TextMeshProUGUI>().text = tempMessage.messageText; // 텍스트 내용 변환
                    Transform characterProfileImage = heroineChatGo.transform.Find("CharacterProfile"); // 프리펩의 캐릭터 프로필 이미지 찾기
                    characterProfileImage.gameObject.GetComponent<Image>().sprite = characterProfileImageList[selectedHeroineIndex - 1];
                    if (tempMessage.index != "s")
                    {
                        Color color = characterProfileImage.gameObject.GetComponent<Image>().color;
                        color.a = 0;
                        characterProfileImage.gameObject.GetComponent<Image>().color = color; //프로필이미지 숨기기
                    }
                    else
                    {
                        Color color = characterProfileImage.gameObject.GetComponent<Image>().color;
                        color.a = 255;
                        characterProfileImage.gameObject.GetComponent<Image>().color = color; //프로필이미지 보이기
                    }
                    }
                else
                {
                    GameObject PlayerChatPrefab = Resources.Load<GameObject>("Rightmessagecontrol");
                    GameObject PlayerChatGo = GameObject.Instantiate(PlayerChatPrefab, chatContentRect);
                    Transform chatText = PlayerChatGo.transform.Find("Chatimage").Find("Chattext"); // 프리펩의 텍스트 찾기
                    chatText.GetComponent<TextMeshProUGUI>().text = tempMessage.messageText; // 텍스트 내용 변환
                    while(!clickFlag)
                    {
                    }
                }
                yield return new WaitForSeconds(1f);
                messagecnt++;
            }
        }
        yield return null;  
    }
}
