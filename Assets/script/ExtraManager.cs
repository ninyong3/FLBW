using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExtraManager : MonoBehaviour
{
    private PersistentData persistentData;

    [Header("UI Sprites & Objects")]
    [SerializeField] private List<Sprite> ExtraSprites;   // 인덱스별 스프라이트
    [SerializeField] private List<GameObject> ExtraObjects; // 인덱스별 Image가 붙어있는 오브젝트

    [Header("Panels (Episode, Ending, CG, Info)")]
    [SerializeField] private List<GameObject> ExtraScenes; // 최소 4개 가정

    void Start()
    {
        LoadPersistent();

        // 배열/리스트가 비었을 수 있으니 조기 가드
        if (ExtraObjects == null) ExtraObjects = new List<GameObject>();
        if (ExtraSprites == null) ExtraSprites = new List<Sprite>();
        if (ExtraScenes == null) ExtraScenes = new List<GameObject>();

        // 데이터 크기 확인 (동적 루프 범위 결정)
        int epiRows  = persistentData?.episodeClearCheck?.GetLength(0) ?? 0; // 보통 3
        int epiCols  = persistentData?.episodeClearCheck?.GetLength(1) ?? 0; // 보통 6 (0~5)
        int endCount = persistentData?.endingClearCheck?.Length      ?? 0;   // 보통 5

        int cnt = 0;

        // 0번 칸: 세 히로인 중 아무나 0번째(에피소드0) 클리어이면 오픈
        bool anyEpisode0 =
            epiRows > 0 && epiCols > 0 &&
            (
                (epiRows > 0 && persistentData.episodeClearCheck[0, 0]) ||
                (epiRows > 1 && persistentData.episodeClearCheck[1, 0]) ||
                (epiRows > 2 && persistentData.episodeClearCheck[2, 0])
            );

        if (anyEpisode0)
        {
            TrySetSprite(0, 0);
        }

        // 에피소드: i = 히로인(0..epiRows-1), j = 1..epiCols-1
        for (int i = 0; i < epiRows; i++)
        {
            for (int j = 1; j < epiCols; j++)
            {
                cnt++;
                if (persistentData.episodeClearCheck[i, j])
                {
                    TrySetSprite(cnt, cnt);
                }
            }
        }

        // 엔딩들: endCount 개
        for (int i = 0; i < endCount; i++)
        {
            cnt++;
            if (persistentData.endingClearCheck[i])
            {
                TrySetSprite(cnt, cnt);
            }
        }
    }

    void Update() { /* 필요시 사용 */ }

    // ===== Panel Switchers =====
    public void EpisodeShow()     => ShowPanel(0);
    public void EndingShow()      => ShowPanel(1);
    public void CGShow()          => ShowPanel(2);
    public void InformationShow() => ShowPanel(3);

    // ===== Scene Jumpers (잠금 해제 시에만) =====
    public void GoToNormalEnd()
    {
        if (IsEndingUnlocked(3)) SceneManager.LoadScene("normalending");
    }
    public void GoToBadEnd()
    {
        if (IsEndingUnlocked(4)) SceneManager.LoadScene("badending");
    }
    public void GoToJinYeinHappyEnd()
    {
        if (IsEndingUnlocked(0)) SceneManager.LoadScene("happyending_jinyein");
    }
    public void GoToFreyjaHappyEnd()
    {
        if (IsEndingUnlocked(1)) SceneManager.LoadScene("happyending_freyja");
    }
    public void GoToRuHappyEnd()
    {
        if (IsEndingUnlocked(2)) SceneManager.LoadScene("happyending_ru");
    }

    // ===== Helpers =====
    private void LoadPersistent()
    {
        persistentData = new PersistentData();
        string jsonFolderPath = Path.Combine(Application.persistentDataPath, "persistentSaveData");
        string jsonPath = Path.Combine(jsonFolderPath, "persistentData.json");

        if (Directory.Exists(jsonFolderPath) && File.Exists(jsonPath))
        {
            try
            {
                string jsonString = File.ReadAllText(jsonPath);
                var loaded = JsonConvert.DeserializeObject<PersistentData>(jsonString);
                if (loaded != null) persistentData = loaded;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[ExtraManager] JSON 로드 실패: {e.Message}");
            }
        }
    }

    private void ShowPanel(int activeIndex)
    {
        // 0: Episode, 1: Ending, 2: CG, 3: Info
        for (int i = 0; i < ExtraScenes.Count; i++)
        {
            SetActiveSafe(ExtraScenes[i], i == activeIndex);
        }
    }

    private bool IsEndingUnlocked(int idx)
    {
        return persistentData?.endingClearCheck != null
            && idx >= 0
            && idx < persistentData.endingClearCheck.Length
            && persistentData.endingClearCheck[idx];
    }

    private void SetActiveSafe(GameObject go, bool on)
    {
        if (go && go.activeSelf != on) go.SetActive(on);
    }

    /// <summary>
    /// ExtraObjects[index]의 Image.sprite를 ExtraSprites[spriteIndex]로 세팅 (모두 범위 체크)
    /// </summary>
    private void TrySetSprite(int index, int spriteIndex)
    {
        if (index < 0 || spriteIndex < 0) return;
        if (index >= ExtraObjects.Count) return;
        if (spriteIndex >= ExtraSprites.Count) return;

        var go = ExtraObjects[index];
        if (!go) return;

        var img = go.GetComponent<Image>();
        if (!img) return;

        img.sprite = ExtraSprites[spriteIndex];
    }
}
