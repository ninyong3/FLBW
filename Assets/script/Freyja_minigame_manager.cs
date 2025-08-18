using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Freyja_minigame_manager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("아이콘을 생성하는 스포너를 연결하세요.")]
    public Icon_spawner spawner;

    [Header("Game Settings")]
    [Tooltip("완료해야 하는 라운드 수")]
    public int roundsToPlay = 4;

    [Tooltip("라운드가 끝난 뒤 다음 라운드를 시작하기 전에 대기 시간(초)")]
    public float nextRoundDelay = 0.5f;

    private Queue<char> expectedKeys = new Queue<char>();      // 앞에서부터 판정할 키 시퀀스
    private Queue<GameObject> iconInstances = new Queue<GameObject>(); // 파괴할 아이콘들
    private int currentRound = 0;
    private bool acceptingInput = false;

    void Start()
    {
        // 시작 시 스포너가 자동 SpawnAll()을 호출하면,
        // ReceiveSpawnedIcons에서 초기화됩니다.
        if (spawner == null)
        {
            Debug.LogWarning("[Freyja_minigame_manager] Spawner reference is missing.");
        }
    }

    void Update()
    {
        if (!acceptingInput || expectedKeys.Count == 0) return;

        char? pressed = GetWASDPressed();
        if (pressed == null) return;

        char expected = expectedKeys.Peek();
        if (char.ToUpperInvariant(pressed.Value) == expected)
        {
            // 정답 처리 (동일)
            expectedKeys.Dequeue();
            var icon = iconInstances.Dequeue();
            if (icon != null)
            { 
                Destroy(icon); 
            }

            if (expectedKeys.Count == 0)
            {
                acceptingInput = false;
                currentRound++;
                Debug.Log($"[Manager] Round {currentRound} clear!");
                FindObjectOfType<Freyja_TextManager>().AddCompletedPlay();

                if (currentRound >= roundsToPlay) // 종료 판정
                {
                    Debug.Log("[Manager] All rounds completed!");

                    SceneManager.LoadScene("main");
                }
                else
                {
                    Invoke(nameof(RequestNextRound), nextRoundDelay);
                }
            }
        }
        else
        {
            // ===== 오답 처리: 반짝임 =====
            if (iconInstances.Count > 0)
            {
                var targetIcon = iconInstances.Peek(); // 아직 판정 대기 중인 맨 앞 아이콘
                if (targetIcon != null)
                {
                    var fl = targetIcon.GetComponent<Icon_Flash>();
                    if (fl == null) fl = targetIcon.AddComponent<Icon_Flash>();

                    // 빨간색으로 3회 빠르게 반짝
                    fl.Flash(times: 3, on: 0.07f, off: 0.07f, overrideColor: new Color(1f, 0.25f, 0.25f, 1f));
                }
            }

            Debug.Log($"[Manager] Wrong input. expected={expected}, got={pressed.Value}");
        }
    }

    private char? GetWASDPressed()
    {
        // 한 프레임에 여러 키가 들어올 수 있으므로 우선순위를 정하거나
        // 먼저 눌린 키를 반환. 여기선 W/A/S/D 순서대로 체크.
        if (Input.GetKeyDown(KeyCode.W)) return 'W';
        if (Input.GetKeyDown(KeyCode.A)) return 'A';
        if (Input.GetKeyDown(KeyCode.S)) return 'S';
        if (Input.GetKeyDown(KeyCode.D)) return 'D';
        return null;
    }

    private void RequestNextRound()
    {
        if (spawner == null)
        {
            Debug.LogWarning("[Manager] Cannot start next round: spawner missing.");
            return;
        }
        spawner.SpawnAll();
    }

    /// <summary>
    /// Icon_spawner에서 스폰이 끝나면 호출하세요.
    /// 좌->우 순서로 전달된 아이콘 리스트를 사용해 입력 시퀀스/파괴 대상을 설정합니다.
    /// </summary>
    public void ReceiveSpawnedIcons(List<Icon_spawner.IconInfo> icons)
    {
        // 초기화
        expectedKeys.Clear();
        iconInstances.Clear();

        // 전달된 순서(좌->우)대로 큐에 삽입
        foreach (var info in icons)
        {
            expectedKeys.Enqueue(char.ToUpperInvariant(info.key));
            iconInstances.Enqueue(info.instance);
        }

        Debug.Log($"[Manager] Received {icons.Count} icons. Round {currentRound + 1} start!");
        acceptingInput = true;
    }
}
