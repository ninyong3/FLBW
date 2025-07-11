using UnityEngine;
using System.Collections;
using TMPro;

public class DialogManager : MonoBehaviour
{
    bool gotoNext=false; // ���� ���� �Ѿ�� �ϴ��� �Ǵ��ϴ� ����
    int dialogCnt=1; // ��� ��ȣ
    [SerializeField] TextMeshProUGUI name; // �̸� text
    [SerializeField] TextMeshProUGUI dialog; // ��� text
    void Start()
    {
        ShowDialog();
    }
    void Update()
    {
        if(gotoNext)
        {
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) // ����Ű Ȥ�� ��Ŭ�� ��
            {
                dialogCnt++; // ���� ��ȣ�� ����
                gotoNext= false;
                StartCoroutine(PrintText());
            }
        }
        
    }
    IEnumerator PrintText()
    {
        dialog.text = DBManager.instance.dialogueDic[dialogCnt].line; // ��ųʸ����� ��� ��ȣ�� ��� ��������
        name.text = DBManager.instance.dialogueDic[dialogCnt].name; // ��ųʸ����� ��� ��ȣ�� �̸� ��������
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
