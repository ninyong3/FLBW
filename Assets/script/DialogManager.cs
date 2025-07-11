using UnityEngine;
using System.Collections;
using TMPro;

public class DialogManager : MonoBehaviour
{
    bool gotoNext=false; // 다음 대사로 넘어가야 하는지 판단하는 변수
    int dialogCnt=1; // 대사 번호
    [SerializeField] TextMeshProUGUI name; // 이름 text
    [SerializeField] TextMeshProUGUI dialog; // 대사 text
    void Start()
    {
        ShowDialog();
    }
    void Update()
    {
        if(gotoNext)
        {
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) // 엔터키 혹은 좌클릭 시
            {
                dialogCnt++; // 다음 번호의 대사로
                gotoNext= false;
                StartCoroutine(PrintText());
            }
        }
        
    }
    IEnumerator PrintText()
    {
        dialog.text = DBManager.instance.dialogueDic[dialogCnt].line; // 딕셔너리에서 대사 번호로 대사 가져오기
        name.text = DBManager.instance.dialogueDic[dialogCnt].name; // 딕셔너리에서 대사 번호로 이름 가져오기
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
