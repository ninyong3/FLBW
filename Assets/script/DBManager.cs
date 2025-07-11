using UnityEngine;
using System.Collections.Generic;
public class DBManager : MonoBehaviour
{
    public static DBManager instance;
    [SerializeField] string dialogFileName; // �Ľ��� ���ϸ��� �ۼ��ϱ� ���� ����
    public Dictionary<int, Dialogue> dialogueDic=new Dictionary<int, Dialogue>();
    public static bool isFinish = false;
    private void Start()
    {
       if(instance == null)
       {
            instance= this;
            DialogueParser parser=GetComponent<DialogueParser>();
            Dialogue[] dialogues = parser.Parse(dialogFileName); // �Ľ��ؼ� dialogues�� ����
            for (int i = 0; i < dialogues.Length; i++)
                dialogueDic.Add(i + 1, dialogues[i]);  //��ųʸ��� ��� ��ȣ�� key, �̸��� ��縦 value�� �ؼ� ����  
            isFinish= true;
       }
    }
 /*   public Dialogue[] GetDialogue(int dialogCnt)
    {
        List<Dialogue> dialogueList=new List<Dialogue>();
        dialogueList.Add(dialogueDic[dialogCnt]);
        return dialogueList.ToArray();
    }*/
}
