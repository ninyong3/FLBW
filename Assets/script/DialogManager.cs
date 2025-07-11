using UnityEngine;
using System.Collections;
using TMPro;

public class DialogManager : MonoBehaviour
{
    bool gotoNext=false;
    int dialogCnt=1;
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI dialog;
    void Start()
    {
        ShowDialog();
    }
    void Update()
    {
        if(gotoNext)
        {
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                dialogCnt++;
                gotoNext= false;
                StartCoroutine(PrintText());
            }
        }
        
    }
    IEnumerator PrintText()
    {
        dialog.text = DBManager.instance.dialogueDic[dialogCnt].line;
        name.text = DBManager.instance.dialogueDic[dialogCnt].name;
        gotoNext= true;
        yield break;
    }
    public void ShowDialog()
    {
        dialog.text = "";
        name.text = "";
        StartCoroutine(PrintText());
    }
}
