using UnityEngine;
using System.Collections.Generic;
public class DBManager : MonoBehaviour
{
    public static DBManager instance;
    [SerializeField] string dialogFileName; // 파싱할 파일명을 작성하기 위한 변수
    public Dictionary<int, Dialogue> dialogueDic=new Dictionary<int, Dialogue>();
    public static bool isFinish = false;
    private void Start()
    {
       if(instance == null)
       {
            instance= this;
            DialogueParser parser=GetComponent<DialogueParser>();
            Dialogue[] dialogues = parser.Parse(dialogFileName); // 파싱해서 dialogues에 저장
            for (int i = 0; i < dialogues.Length; i++)
                dialogueDic.Add(i + 1, dialogues[i]);  //딕셔너리에 대사 번호를 key, 이름과 대사를 value로 해서 저장  
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
