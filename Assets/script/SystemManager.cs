using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class SystemManager : MonoBehaviour
{
    [SerializeField] GameObject dialog;
    Coroutine waitClick;
    int check=0;
    GameObject currentClickObject;
    GameObject previousClickObject;
    [SerializeField] GameObject Closeimage;
    [SerializeField] GameObject Titlewarningimage;
    [SerializeField] GameObject Skipwarningimage;
    void Update()
    {
        if (previousClickObject != Closeimage)
            check = 0;
        previousClickObject = currentClickObject;
        currentClickObject = EventSystem.current.currentSelectedGameObject;
    }
    void Start()
    {
        Titlewarningimage.SetActive(false);
        Skipwarningimage.SetActive(false);
    }
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
    public void ToTitleSystem()
    {
        Titlewarningimage.SetActive(true);
    }
    public void ToTitleClickYes()
    {
      SceneManager.LoadScene("title");
    }
    public void ToTitleClickNo()
    {
        Titlewarningimage.SetActive(false);
    }
    public void SkipDaySystem()
    {
        Skipwarningimage.SetActive(true);
    }
    public void SkipDayClickYes()
    {
        GameManager.instance.dayCount++;
        Skipwarningimage.SetActive(false);
    }
    public void SkipDayClickNo()
    {
        Skipwarningimage.SetActive(false);
    }
}
