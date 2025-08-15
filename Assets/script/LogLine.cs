using UnityEngine;

public class LogLine : MonoBehaviour
{
    int myIndex;
    public void Count(int index)
    {
        myIndex = index;
    }
    public void ClickMe()
    {
        DialogManager manager = FindFirstObjectByType<DialogManager>();
        if (manager != null)
        { 
            manager.BackLog(myIndex);
        }
    }
}
