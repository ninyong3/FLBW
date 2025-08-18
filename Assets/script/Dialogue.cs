using UnityEngine;
[System.Serializable]
public class Dialogue // 대사 클래스
{
    public string name; // 이름
    public string line; // 대사
    public string choiceIndex;
    public string choiceline;
    public int[] characterIndex = new int[2];
    public int backgroundIndex;
    public int BGMIndex;
    public int SFXIndex;
    public int relationship_level;
}