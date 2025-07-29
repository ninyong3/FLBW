using UnityEngine;
using System.Collections;
public class SystemManager : MonoBehaviour
{
    [SerializeField] GameObject dialog;
    Coroutine waitClick;
    int check=0;
    public void CloseSystem()
    {
        if (check == 0)
        {
            dialog.SetActive(false);
            check = 1;
            StartCoroutine(ReshowDialog());
        }
        else
            check= 0;
    }
    IEnumerator ReshowDialog() 
    { 
        while(!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        dialog.SetActive(true);
    }
}
