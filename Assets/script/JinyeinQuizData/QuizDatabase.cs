using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizDatabase", menuName = "Jinyein/Quiz Database")]
public class QuizDatabase : ScriptableObject
{
    // Jinyein_Minigame_Manager 안의 [System.Serializable] public class QuizItem 를 재사용
    public List<Jinyein_Minigame_Manager.QuizItem> items = new();
}
