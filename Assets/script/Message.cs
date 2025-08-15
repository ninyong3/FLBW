using UnityEngine;

public class Message : MonoBehaviour
{
    public void OnClickSendNext()
    {
        MessengerManager manager = FindFirstObjectByType<MessengerManager>();
        if (manager != null)
        {
            manager.ChatToNext();
        }
    }
}
