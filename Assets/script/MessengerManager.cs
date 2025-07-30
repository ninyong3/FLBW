using UnityEngine;
using System.Collections;
public class MessengerManager : MonoBehaviour
{
    [SerializeField] GameObject messenger;
    Vector2 messengerPosition;
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
}
