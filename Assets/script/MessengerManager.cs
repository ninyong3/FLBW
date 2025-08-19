using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MessengerManager : MonoBehaviour
{
    [SerializeField] GameObject messenger;
    Vector2 messengerPosition;
    [SerializeField] RectTransform chatContentRect; // 메신저의 콘텐츠
    [SerializeField] TextMeshProUGUI characterName; // 현재 선택한 캐릭터의 이름
    int selectedHeroineIndex=0;
    [SerializeField] List<Sprite> characterProfileImageList;
    [SerializeField] TextMeshProUGUI leftMessageText;
    int messagecnt;
    bool clickFlag = false;
    void Start()
    {
        messagecnt=GameManager.instance.messageCount;
    }

    void Update()
    {
        leftMessageText.text = GameManager.instance.leftMessageCount.ToString();
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
        MessageParser messageParser = FindFirstObjectByType<MessageParser>();
        MessageData tempMessage;
        if(selectedHeroineIndex == 1)
        {
            if(messageParser.jinyeinMessageDic.TryGetValue(messagecnt, out tempMessage))
            {
                if(tempMessage.index != "-")
                {
                    GameManager.instance.leftMessageCount--;
                    GameManager.instance.messageCount++;
                    SceneManager.LoadScene(tempMessage.index);
                }
            }    
        }
        else if (selectedHeroineIndex == 2)
        {
            if (messageParser.freyjaMessageDic.TryGetValue(messagecnt, out tempMessage))
            {
                if (tempMessage.index != "-")
                {
                    GameManager.instance.leftMessageCount--;
                    GameManager.instance.messageCount++;
                    SceneManager.LoadScene(tempMessage.index);
                }
            }
        }
        else if (selectedHeroineIndex == 3)
        {
            if (messageParser.ruMessageDic.TryGetValue(messagecnt, out tempMessage))
            {
                if (tempMessage.index != "-")
                {
                    GameManager.instance.leftMessageCount--;
                    GameManager.instance.messageCount++;
                    SceneManager.LoadScene(tempMessage.index);
                }
            }
        }
        clickFlag = true;
    }
    public void SelectJinyeinMessage() // 진예인 메세지 확인 시 작동하는 함수
    {
        if (selectedHeroineIndex != 1)
        {
            foreach (Transform child in chatContentRect)
                Destroy(child.gameObject);
            StopAllCoroutines();
            characterName.text = "진예인";
            selectedHeroineIndex = 1;
            MessageParser messageParser = FindFirstObjectByType<MessageParser>();
            MessageData tempMessage;
            for (int i = 1; i < messagecnt; i++)
            {
                GameManager.instance.messageCount = messagecnt;
                tempMessage = messageParser.jinyeinMessageDic[i];
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
                    if (tempMessage.index == "normalending" && GameManager.instance.relationship_level < 3 || tempMessage.index == "happyending_jinyein" && GameManager.instance.relationship_level > 2 || tempMessage.index != "happyending_jinyein" && tempMessage.index != "normalending")
                    {
                        GameObject PlayerChatPrefab = Resources.Load<GameObject>("Rightmessagecontrol");
                        GameObject PlayerChatGo = GameObject.Instantiate(PlayerChatPrefab, chatContentRect);
                        PlayerChatGo.GetComponentInChildren<Message>().clickCheck = true;
                        Transform chatText = PlayerChatGo.transform.Find("Chatimage").Find("Chattext"); // 프리펩의 텍스트 찾기
                        chatText.GetComponent<TextMeshProUGUI>().text = tempMessage.messageText; // 텍스트 내용 변경
                        clickFlag = false;
                    }
                }
            }
            StartCoroutine(SpawnMessage());
        }
    }
    public void SelectFreyjaMessage() // 프레이야 메세지 확인 시 작동하는 함수
    {
        if (selectedHeroineIndex != 2)
        {
            foreach (Transform child in chatContentRect)
                Destroy(child.gameObject);
            StopAllCoroutines();
            characterName.text = "프레이야 레가토";
            selectedHeroineIndex = 2;
            MessageParser messageParser = FindFirstObjectByType<MessageParser>();
            MessageData tempMessage;
            for (int i = 1; i < messagecnt; i++)
            {
                GameManager.instance.messageCount = messagecnt;
                tempMessage = messageParser.freyjaMessageDic[i];
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
                    if (tempMessage.index == "normalending" && GameManager.instance.relationship_level < 3 || tempMessage.index == "happyending_freyja" && GameManager.instance.relationship_level > 2 || tempMessage.index != "happyending_freyja" && tempMessage.index != "normalending")
                    {
                        GameObject PlayerChatPrefab = Resources.Load<GameObject>("Rightmessagecontrol");
                        GameObject PlayerChatGo = GameObject.Instantiate(PlayerChatPrefab, chatContentRect);
                        PlayerChatGo.GetComponentInChildren<Message>().clickCheck = true;
                        Transform chatText = PlayerChatGo.transform.Find("Chatimage").Find("Chattext"); // 프리펩의 텍스트 찾기
                        chatText.GetComponent<TextMeshProUGUI>().text = tempMessage.messageText; // 텍스트 내용 변경
                        clickFlag = false;
                    }
                }
            }
            StartCoroutine(SpawnMessage());
        }
    }
    public void SelectRuMessage() // 루 메세지 확인 시 작동하는 함수
    {
        if (selectedHeroineIndex != 3)
        {
            foreach (Transform child in chatContentRect)
                Destroy(child.gameObject);
            StopAllCoroutines();
            characterName.text = "루";
            selectedHeroineIndex = 3;
            MessageParser messageParser = FindFirstObjectByType<MessageParser>();
            MessageData tempMessage;
            for (int i = 1; i < messagecnt; i++)
            {
                GameManager.instance.messageCount = messagecnt;
                tempMessage = messageParser.ruMessageDic[i];
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
                    if (tempMessage.index == "normalending" && GameManager.instance.relationship_level < 3 || tempMessage.index == "happyending_ru" && GameManager.instance.relationship_level > 2 || tempMessage.index != "happyending_ru" && tempMessage.index != "normalending")
                    {
                        GameObject PlayerChatPrefab = Resources.Load<GameObject>("Rightmessagecontrol");
                        GameObject PlayerChatGo = GameObject.Instantiate(PlayerChatPrefab, chatContentRect);
                        PlayerChatGo.GetComponentInChildren<Message>().clickCheck = true;
                        Transform chatText = PlayerChatGo.transform.Find("Chatimage").Find("Chattext"); // 프리펩의 텍스트 찾기
                        chatText.GetComponent<TextMeshProUGUI>().text = tempMessage.messageText; // 텍스트 내용 변경
                        clickFlag = false;
                    }
                }
            }
            StartCoroutine(SpawnMessage());
        }
    }
    IEnumerator SpawnMessage()
    {
        if (GameManager.instance.leftMessageCount != 0)
        {
            MessageParser messageParser = FindFirstObjectByType<MessageParser>();
            MessageData tempMessage;
            if (selectedHeroineIndex == 1)
            {
                while (messageParser.jinyeinMessageDic.TryGetValue(messagecnt, out tempMessage))
                {
                    GameManager.instance.messageCount = messagecnt;
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
                        if (tempMessage.index == "normalending" && GameManager.instance.relationship_level < 3 || tempMessage.index == "happyending_jinyein" && GameManager.instance.relationship_level > 2 || tempMessage.index != "happyending_jinyein" && tempMessage.index != "normalending")
                        {
                            GameObject PlayerChatPrefab = Resources.Load<GameObject>("Rightmessagecontrol");
                            GameObject PlayerChatGo = GameObject.Instantiate(PlayerChatPrefab, chatContentRect);
                            Transform chatText = PlayerChatGo.transform.Find("Chatimage").Find("Chattext"); // 프리펩의 텍스트 찾기
                            chatText.GetComponent<TextMeshProUGUI>().text = tempMessage.messageText; // 텍스트 내용 변환
                            yield return new WaitUntil(() => clickFlag);
                            clickFlag = false;
                        }
                    }
                    yield return new WaitForSeconds(1f);
                    messagecnt++;
                }
            }
            else if (selectedHeroineIndex == 2)
            {
                while (messageParser.freyjaMessageDic.TryGetValue(messagecnt, out tempMessage))
                {
                    GameManager.instance.messageCount = messagecnt;
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
                        if (tempMessage.index == "normalending" && GameManager.instance.relationship_level < 3 || tempMessage.index == "happyending_freyja" && GameManager.instance.relationship_level > 2 || tempMessage.index != "happyending_freyja" && tempMessage.index != "normalending")
                        {
                            GameObject PlayerChatPrefab = Resources.Load<GameObject>("Rightmessagecontrol");
                            GameObject PlayerChatGo = GameObject.Instantiate(PlayerChatPrefab, chatContentRect);
                            Transform chatText = PlayerChatGo.transform.Find("Chatimage").Find("Chattext"); // 프리펩의 텍스트 찾기
                            chatText.GetComponent<TextMeshProUGUI>().text = tempMessage.messageText; // 텍스트 내용 변환
                            yield return new WaitUntil(() => clickFlag);
                            clickFlag = false;
                        }
                    }
                    yield return new WaitForSeconds(1f);
                    messagecnt++;
                }
            }
            else if (selectedHeroineIndex == 3)
            {
                while (messageParser.ruMessageDic.TryGetValue(messagecnt, out tempMessage))
                {
                    GameManager.instance.messageCount = messagecnt;
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
                        if (tempMessage.index == "normalending" && GameManager.instance.relationship_level < 3 || tempMessage.index == "happyending_ru" && GameManager.instance.relationship_level > 2 || tempMessage.index != "happyending_ru" && tempMessage.index != "normalending")
                        {
                            GameObject PlayerChatPrefab = Resources.Load<GameObject>("Rightmessagecontrol");
                            GameObject PlayerChatGo = GameObject.Instantiate(PlayerChatPrefab, chatContentRect);
                            Transform chatText = PlayerChatGo.transform.Find("Chatimage").Find("Chattext"); // 프리펩의 텍스트 찾기
                            chatText.GetComponent<TextMeshProUGUI>().text = tempMessage.messageText; // 텍스트 내용 변환
                            yield return new WaitUntil(() => clickFlag);
                            clickFlag = false;
                        }
                    }
                    yield return new WaitForSeconds(1f);
                    messagecnt++;
                }
            }
            yield return null;
        }
    }
}
