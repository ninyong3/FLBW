using UnityEngine;
using System.Collections.Generic;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string dialogFileName)
    {
        List<Dialogue> dialoguelist=new List<Dialogue>();
        TextAsset dialogueData=Resources.Load<TextAsset>(dialogFileName);
        string[] data = dialogueData.text.Split(new char[] { '\n' });
        for(int i=1;i<data.Length;i++)
        {
            string[] row = data[i].Split(new char[] {',' });
            Dialogue dialogue= new Dialogue();
            dialogue.name = row[1];
            dialogue.line=row[2];
            dialoguelist.Add(dialogue);
        }
        return dialoguelist.ToArray();
    }

}
