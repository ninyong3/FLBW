using UnityEngine;
using System.Collections.Generic;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string dialogFileName) // �Ľ� �Լ�
    {
        List<Dialogue> dialoguelist=new List<Dialogue>(); // ��縦 �����ϴ� ����Ʈ
        TextAsset dialogueData=Resources.Load<TextAsset>(dialogFileName); // dialogFileName�� ����ִ� ���ϸ��� ��� �ε�
        string[] data = dialogueData.text.Split(new char[] { '\n' }); // �� ������ data ����Ʈ�� ��� ����
        for(int i=1;i<data.Length;i++)
        {
            string[] row = data[i].Split(new char[] {',' }); // ��ǥ ������ ��� ����
            Dialogue dialogue= new Dialogue();
            dialogue.name = row[1]; 
            dialogue.line=row[2];
            dialoguelist.Add(dialogue);
        }
        return dialoguelist.ToArray(); // �迭�� ��ȯ�� ��ȯ
    }

}
