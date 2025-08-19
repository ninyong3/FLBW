using System;
using System.Collections.Generic;
using UnityEngine;

public class Icon_spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject Icon_W;
    public GameObject Icon_A;
    public GameObject Icon_S;
    public GameObject Icon_D;

    [Header("Spawn Settings")]
    public float spawnY = 2f;
    public float xStart = -7f;   // 포함
    public float xEnd = 7f;   // 포함
    public float xStep = 2f;   // 간격 (= 2)

    [Tooltip("생성된 아이콘을 이 트랜스폼의 자식으로 둡니다 (선택).")]
    public Transform container;

    [Header("Manager")]
    [Tooltip("생성 결과를 전달할 Freyja_minigame_manager를 지정하세요.")]
    public Freyja_minigame_manager freyjaManager;

    [Serializable]
    public struct IconInfo
    {
        public char key;            // 'W','A','S','D'
        public GameObject instance; // 실제 인스턴스
        public Vector2 position;    // 스폰된 2D 위치
    }

    private readonly System.Random _rng = new System.Random();

    void Start()
    {
        SpawnAll();
    }

    /// <summary>
    /// xStart ~ xEnd (step xStep)에 맞춰 총 8개 아이콘을 무작위 프리팹으로 생성.
    /// 생성된 아이콘 정보를 Freyja_minigame_manager로 전달.
    /// </summary>
    public void SpawnAll()
    {
        // 방어 로직: 필수 프리팹 체크
        if (!Icon_W || !Icon_A || !Icon_S || !Icon_D)
        {
            Debug.LogError("[Icon_spawner] 프리팹(Icon_W/A/S/D) 레퍼런스가 비어있습니다.");
            return;
        }

        var pool = new List<(char key, GameObject prefab)>
        {
            ('W', Icon_W),
            ('A', Icon_A),
            ('S', Icon_S),
            ('D', Icon_D),
        };

        var results = new List<IconInfo>();
        var parent = container ? container : transform;

        // -7, -5, -3, -1, 1, 3, 5, 7
        for (float x = xStart; x <= xEnd + 0.001f; x += xStep)
        {
            // 무작위 프리팹 선택
            int idx = _rng.Next(pool.Count);
            var (key, prefab) = pool[idx];

            Vector3 pos = new Vector3(x, spawnY, 0f);
            GameObject go = Instantiate(prefab, pos, Quaternion.identity, parent);

            // 보기 좋게 이름 지정
            go.name = $"{key}_Icon ({x:0})";

            results.Add(new IconInfo
            {
                key = key,
                instance = go,
                position = new Vector2(pos.x, pos.y)
            });
        }

        // 매니저로 전달
        if (freyjaManager != null)
        {
            try
            {
                freyjaManager.ReceiveSpawnedIcons(results);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Icon_spawner] 매니저로 전달 중 예외: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("[Icon_spawner] freyjaManager가 비어있어 결과를 전달하지 못했습니다.");
        }
    }
}
