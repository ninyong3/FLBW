using System;
using UnityEngine;
[Serializable]
public class PersistentData
{
    public bool[ , ] episodeClearCheck = new bool[3, 6];
    public bool[] endingClearCheck = new bool[5];
}
