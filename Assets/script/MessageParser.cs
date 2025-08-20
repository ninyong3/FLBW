using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MessageParser : MonoBehaviour
{
    [SerializeField] List<string> fileNameList;

    public Dictionary<int, MessageData> jinyeinMessageDic = new Dictionary<int, MessageData>();
    public Dictionary<int, MessageData> freyjaMessageDic  = new Dictionary<int, MessageData>();
    public Dictionary<int, MessageData> ruMessageDic      = new Dictionary<int, MessageData>();

    void Start()
    {
        if (fileNameList == null || fileNameList.Count < 1)
        {
            Debug.LogError("[MessageParser] fileNameList 가 비어있습니다.");
            return;
        }

        // 0: 진예인, 1: 프레이야, 2: 루  (부족하면 건너뜀)
        if (fileNameList.Count >= 1)
            FillDict(ParseSafe(fileNameList[0]), jinyeinMessageDic, "Jin");

        if (fileNameList.Count >= 2)
            FillDict(ParseSafe(fileNameList[1]), freyjaMessageDic, "Freyja");

        if (fileNameList.Count >= 3)
            FillDict(ParseSafe(fileNameList[2]), ruMessageDic, "Ru");
    }

    static void FillDict(MessageData[] arr, Dictionary<int, MessageData> dict, string tag)
    {
        if (arr == null)
        {
            Debug.LogWarning($"[MessageParser] {tag} 데이터가 없습니다.");
            return;
        }
        dict.Clear();
        for (int i = 0; i < arr.Length; i++)
            dict[i + 1] = arr[i];
        Debug.Log($"[MessageParser] {tag} {arr.Length} rows loaded.");
    }

    MessageData[] ParseSafe(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Debug.LogError("[MessageParser] 파일명이 비어있습니다.");
            return null;
        }

        TextAsset messageData = Resources.Load<TextAsset>("scenario/" + fileName.Trim());
        if (messageData == null)
        {
            Debug.LogError($"[MessageParser] Resources/scenario/{fileName} 로드 실패 (확장자 넣지 마세요).");
            return null;
        }

        var messageList = new List<MessageData>();
        string[] lines = messageData.text.Split('\n');

        // CSV 1행이 헤더라면 i=1부터 맞음. 아니라면 0부터로 바꾸세요.
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd('\r'); // CRLF 대응
            if (string.IsNullOrWhiteSpace(line)) continue;

            // 따옴표 안의 콤마는 유지
            Regex splitRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            string[] row = splitRegex.Split(line);

            if (row.Length < 3)
            {
                Debug.LogWarning($"[MessageParser] 열(칼럼) 부족: line {i+1} -> \"{line}\"");
                continue;
            }

            var md = new MessageData();
            md.name  = row[0].Trim();
            md.index = row[1].Trim();

            string text = row[2].Trim();
            if (text.StartsWith("\"") && text.EndsWith("\"") && text.Length >= 2)
                text = text.Substring(1, text.Length - 2);
            md.messageText = text.Replace("…", "...");

            messageList.Add(md);
        }

        return messageList.ToArray();
    }
}
