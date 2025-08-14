using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string dialogFileName) // 파싱 함수
    {
        List<Dialogue> dialoguelist=new List<Dialogue>(); // 대사를 저장하는 리스트
        TextAsset dialogueData=Resources.Load<TextAsset>("scenario/"+dialogFileName); // dialogFileName에 들어있는 파일명에서 대사 로드
        string[] data = dialogueData.text.Split(new char[] { '\n' }); // 줄 단위로 data 리스트에 끊어서 저장
        for(int i=1;i<data.Length;i++)
        {
            Regex splitRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            string[] row = splitRegex.Split(data[i]); // 쉼표 단위로 끊어서 저장
            Dialogue dialogue= new Dialogue();
            dialogue.name = row[0]; 
            dialogue.choiceIndex = row[1];
            row[2] = row[2].Trim();
            if (row[2].StartsWith("\"") && row[2].EndsWith("\""))
                row[2] = row[2].Substring(1, row[2].Length - 2);
            dialogue.choiceline = row[2].Replace("…", "...");
            row[3]=row[3].Trim();
            if (row[3].StartsWith("\"") && row[3].EndsWith("\""))
                row[3]=row[3].Substring(1, row[3].Length - 2);
            dialogue.line = row[3].Replace("…", "...");
            string[] characterIndexTemp = row[4].Split("_");
            if (characterIndexTemp.Length == 2)
            {
                dialogue.characterIndex[0] = int.Parse(characterIndexTemp[0]);
                dialogue.characterIndex[1] = int.Parse(characterIndexTemp[1]);
            }
            else
                dialogue.characterIndex[0] = -1;
            if (int.TryParse(row[5], out dialogue.backgroundIndex))
            { 
            }
            else
                dialogue.backgroundIndex = -1;
            if (int.TryParse(row[6], out dialogue.BGMIndex))
            {

            }
            else
                dialogue.BGMIndex = -1;
            if (int.TryParse(row[7], out dialogue.SFXIndex))
            {

            }
            else
                dialogue.SFXIndex = -1;
            dialoguelist.Add(dialogue);
        }
        return dialoguelist.ToArray(); // 배열로 변환해 반환
    }

}
