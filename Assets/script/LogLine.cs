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
        BackLogManager manager = FindFirstObjectByType < BackLogManager >();
        if(manager != null)
        {

        }

    }
}
