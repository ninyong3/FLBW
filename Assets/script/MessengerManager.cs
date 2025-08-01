using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MessengerManager : MonoBehaviour
{
    [SerializeField] GameObject messenger;
    Vector2 messengerPosition;
    [SerializeField] RectTransform chatContentRect;
    [SerializeField] TextMeshProUGUI characterName;
    void Start()
    {
        
    }

    void Update()
    {
       
    }
    public void MessengerShow()
    {
        StartCoroutine(ShowMessenger());
    }
    public void MessengerHide()
    {
        StartCoroutine(HideMessenger());
    }
    IEnumerator ShowMessenger()
    {
        messengerPosition = messenger.GetComponent<RectTransform>().anchoredPosition;
        while (messengerPosition.x <= -590)
        {
            messengerPosition.x += 10;
            messenger.GetComponent<RectTransform>().anchoredPosition = messengerPosition;
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator HideMessenger()
    {
        messengerPosition = messenger.GetComponent<RectTransform>().anchoredPosition;
        while (messengerPosition.x >= -1338.5)
        {
            messengerPosition.x -= 10;
            messenger.GetComponent<RectTransform>().anchoredPosition = messengerPosition;
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void ChatToNext()
    {
        GameObject heroineChatPrefab = Resources.Load<GameObject>("Leftmessagecontrol");
        GameObject heroineChatGo = GameObject.Instantiate(heroineChatPrefab, chatContentRect);
        GameObject PlayerChatPrefab = Resources.Load<GameObject>("Rightmessagecontrol");
        GameObject PlayerChatGo = GameObject.Instantiate(PlayerChatPrefab, chatContentRect);
        Transform chatText = PlayerChatGo.transform.Find("Chatimage").Find("Chattext");
        chatText.GetComponent<TextMeshProUGUI>().text = "테스트";
        Transform characterProfileImage = heroineChatGo.transform.Find("CharacterProfile");
      //  characterProfileImage.gameObject.SetActive(false);
    }
    public void SelectJinyeinMessage()
    {
        characterName.text = "진예인";
    }
    public void SelectFreyjaMessage()
    {
        characterName.text = "프레이야 레가토";
    }
    public void SelectRuMessage()
    {
        characterName.text = "루";
    }
}
