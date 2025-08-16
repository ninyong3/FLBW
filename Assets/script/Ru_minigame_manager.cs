using UnityEngine;
using TMPro;
using System.Collections;

public class Ru_minigame_manager : MonoBehaviour
{
    public TMP_Text messageText;   // Canvas의 TMP 텍스트
    public float slowTime = 0.2f;

    private bool finished = false;

    private void Start()
    {
        // 게임 시작 안내 출력
        if (messageText != null)
        {
            messageText.alignment = TextAlignmentOptions.Center; // 중앙 정렬
            StartCoroutine(ShowStartMessage());
        }
    }

    private IEnumerator ShowStartMessage()
    {
        messageText.text = "알바 시작!\n택배박스를 꺼내오세요!";
        messageText.enabled = true;

        yield return new WaitForSeconds(3f);

        messageText.enabled = false;
    }

    public void Clear()
    {
        if (finished) return;
        finished = true;

        // 게임 종료 연출
        Time.timeScale = slowTime;

        if (messageText != null)
        {
            messageText.alignment = TextAlignmentOptions.Center;
            messageText.text = "클리어! 택배 박스를 꺼냈습니다!";
            messageText.enabled = true;
        }
    }
}
