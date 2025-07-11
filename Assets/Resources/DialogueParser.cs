using UnityEngine;
using System.Collections.Generic;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string dialogFileName) // 파싱 함수
    {
        List<Dialogue> dialoguelist=new List<Dialogue>(); // 대사를 저장하는 리스트
        TextAsset dialogueData=Resources.Load<TextAsset>(dialogFileName); // dialogFileName에 들어있는 파일명에서 대사 로드
        string[] data = dialogueData.text.Split(new char[] { '\n' }); // 줄 단위로 data 리스트에 끊어서 저장
        for(int i=1;i<data.Length;i++)
        {
            string[] row = data[i].Split(new char[] {',' }); // 쉼표 단위로 끊어서 저장
            Dialogue dialogue= new Dialogue();
            dialogue.name = row[1]; 
            dialogue.line=row[2];
            dialoguelist.Add(dialogue);
        }
        return dialoguelist.ToArray(); // 배열로 변환해 반환
    }

}
