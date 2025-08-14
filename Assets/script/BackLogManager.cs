using UnityEngine;

public class BackLogManager : MonoBehaviour
{
    public void SendMyIndex()
    {
        int myIndex=transform.GetSiblingIndex();
        Debug.Log(myIndex);
    }
}
