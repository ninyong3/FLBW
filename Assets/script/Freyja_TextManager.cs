using System.Collections;
using UnityEngine;
using TMPro;   // TextMeshProUGUI 사용

public class Freyja_TextManager : MonoBehaviour
{
    [Header("UI 참조")]
    public TextMeshProUGUI fixedText;    // 게임 시작 시 5초 표시할 텍스트
    public TextMeshProUGUI progressText; // 진행 상황 텍스트

    private int totalPlays = 4;   // 총 연주 횟수 (예시)
    private int completedPlays = 0; // 현재 완료 횟수

    void Start()
    {
        // 시작 텍스트 출력
        if (fixedText != null)
        {
            fixedText.text = "W/A/S/D를  눌러서 리코더를 연주해보자!";
            fixedText.gameObject.SetActive(true);
            StartCoroutine(HideFixedTextAfterDelay(5f));
        }

        // 진행 상황 초기화
        UpdateProgress();
    }

    IEnumerator HideFixedTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (fixedText != null)
        {
            fixedText.gameObject.SetActive(false);
        }
    }

    // 외부에서 "연주 완료" 이벤트가 발생할 때 호출
    public void AddCompletedPlay()
    {
        completedPlays++;
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        if (progressText != null)
        {
            progressText.text = $"{totalPlays}번 연주 중 {completedPlays}번 완료";
        }
    }
}
