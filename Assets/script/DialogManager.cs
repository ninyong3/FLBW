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
            if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButton(0))
            {
                dialogCnt++;
            }
        }
        
    }
    IEnumerator PrintText()
    {
        dialog.text = DBManager.instance.dialogueDic[dialogCnt].line;
        name.text = DBManager.instance.dialogueDic[dialogCnt].name;
        yield break;
    }
    public void ShowDialog()
    {
        dialog.text = "";
        name.text = "";
        StartCoroutine(PrintText());
    }
}
