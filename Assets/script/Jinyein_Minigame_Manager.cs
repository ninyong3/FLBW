using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Jinyein_Minigame_Manager : MonoBehaviour
{
    [System.Serializable]
    public class QuizItem
    {
        [TextArea(2, 4)] public string question;
        public string optionA, optionB, optionC;
        [Range(0, 2)] public int answerIndex;
        public Sprite portrait;
    }

    [Header("UI Refs")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] choiceButtons;         // 3개
    [SerializeField] private TextMeshProUGUI[] choiceLabels; // 3개
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Game Settings")]
    [SerializeField] private int clearsNeeded = 5;

    // 인스펙터 직접 입력(있으면 유지)
    [SerializeField] private List<QuizItem> quizPool = new();

    // ⬇️ 추가: 외부 DB 에셋들(여러 개 넣어도 됨)
    [Header("Databases")]
    [SerializeField] private QuizDatabase[] databases;

    [Header("Timing")]
    [SerializeField] private float nextDelay = 0.8f;

    private int clears = 0;
    private List<QuizItem> deck;
    private int current = -1;
    private bool accepting = true;
    private bool gameOver = false;

    void Start()
    {
        // 1) 인스펙터에 넣은 기본 문제 유지
        // 2) 연결된 DB 에셋들의 items를 모두 합치기
        if (databases != null)
        {
            foreach (var db in databases)
            {
                if (db != null && db.items != null && db.items.Count > 0)
                    quizPool.AddRange(db.items);
            }
        }

        // 덱 셔플 및 UI 초기화
        deck = new List<QuizItem>(quizPool);
        Shuffle(deck);

        counterText.text = $"클리어: {clears}/{clearsNeeded}";
        if (resultText != null) resultText.gameObject.SetActive(false);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int idx = i;
            choiceButtons[i].onClick.AddListener(() => OnChoice(idx));
        }

        NextQuestion();
    }

    void Shuffle(List<QuizItem> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    void NextQuestion()
    {
        if (gameOver) return;

        if (resultText != null) resultText.gameObject.SetActive(false);
        accepting = true;

        current++;
        if (current >= deck.Count)
        {
            Shuffle(deck);
            current = 0;
        }

        var q = deck[current];

        if (portraitImage != null)
        {
            portraitImage.sprite = q.portrait;
            portraitImage.color = q.portrait ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
        }

        questionText.text = q.question;
        choiceLabels[0].text = q.optionA;
        choiceLabels[1].text = q.optionB;
        choiceLabels[2].text = q.optionC;

        SetButtonsInteractable(true);
    }

    void OnChoice(int picked)
    {
        if (!accepting || gameOver) return;
        accepting = false;

        var q = deck[current];
        bool correct = picked == q.answerIndex;

        if (correct)
        {
            clears++;
            counterText.text = $"클리어: {clears}/{clearsNeeded}";
            ShowResult("정답! 안내를 따라 이동해주세요.");
        }
        else
        {
            ShowResult("오답! 증상에 맞는 진료과를 다시 선택해보세요.");
        }

        SetButtonsInteractable(false);

        if (clears >= clearsNeeded)
        {
            gameOver = true;
            Invoke(nameof(ShowGameClear), nextDelay);
        }
        else
        {
            Invoke(nameof(NextQuestion), nextDelay);
        }
    }

    void ShowResult(string msg)
    {
        if (resultText == null) return;
        resultText.text = msg;
        resultText.gameObject.SetActive(true);
    }

    void ShowGameClear()
    {
        ShowResult("게임 클리어! 수고했어요.");
        SetButtonsInteractable(false);
        GameManager.instance.dayCount++;
        GameManager.instance.relationship_level++;
        SceneManager.LoadScene("main");
    }

    void SetButtonsInteractable(bool on)
    {
        foreach (var b in choiceButtons) if (b != null) b.interactable = on;
    }
}
