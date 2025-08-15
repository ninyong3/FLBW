using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
public class MessageParser : MonoBehaviour
{
    [SerializeField] List<string> fileNameList;
    public Dictionary<int, MessageData> jinyeinMessageDic=new Dictionary<int, MessageData>();
    public Dictionary<int, MessageData> freyjaMessageDic = new Dictionary<int, MessageData>();
    public Dictionary<int, MessageData> ruMessageDic = new Dictionary<int, MessageData>();
    void Start()
    {
        MessageData[] messageDatas = Parse(fileNameList[0]);
        for (int i = 0; i < messageDatas.Length; i++)
        {
            jinyeinMessageDic.Add(i + 1, messageDatas[i]);
        }
        /*messageDatas = Parse(fileNameList[1]);
        for (int i = 0; i < messageDatas.Length; i++)
            freyjaMessageDic.Add(i + 1, messageDatas[i]);
        messageDatas = Parse(fileNameList[2]);
        for (int i = 0; i < messageDatas.Length; i++)
            ruMessageDic.Add(i + 1, messageDatas[i]);*/

    }
    void Update()
    {
        
    }
    MessageData[] Parse(string fileName)
    {
        List<MessageData> messageList = new List<MessageData>();
        TextAsset messageData = Resources.Load<TextAsset>("scenario/" + fileName);
        string[] data = messageData.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)
        {
            Regex splitRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            string[] row = splitRegex.Split(data[i]);
            MessageData dataList = new MessageData();
            dataList.name = row[0];
            dataList.index = row[1];
            row[2] = row[2].Trim();
            if (row[2].StartsWith("\"") && row[2].EndsWith("\""))
                row[2] = row[2].Substring(1, row[2].Length - 2);
            dataList.messageText = row[2].Replace("…", "...");
            messageList.Add(dataList);
        }
        return messageList.ToArray();
    }
}
