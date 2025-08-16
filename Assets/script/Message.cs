using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    public bool clickCheck=false;
    public void OnClickSendNext()
    {
        MessengerManager manager = FindFirstObjectByType<MessengerManager>();
        if (manager != null && clickCheck == false)
        {
            clickCheck = true;
            manager.ChatToNext();
        }
    }
}
