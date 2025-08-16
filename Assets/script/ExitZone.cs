using UnityEngine;

public class ExitZone : MonoBehaviour
{
    public Ru_minigame_manager gameManager; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GoalBox"))
        {
            gameManager?.Clear();
        }
    }
}
    