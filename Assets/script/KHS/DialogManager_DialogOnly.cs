using UnityEngine;
using TMPro;
using System.Collections;

public class DialogManager_DialogOnly : MonoBehaviour
{
    [Header("DB (같은 씬 오브젝트 참조)")]
    public DBManager_DialogOnly DB;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;

    [Header("Control")]
    public KeyCode nextKey = KeyCode.Return;

    int cur = 1;

    void Reset()
    {
#if UNITY_2023_1_OR_NEWER
        DB = Object.FindFirstObjectByType<DBManager_DialogOnly>(FindObjectsInactive.Include);
#else
        DB = Object.FindObjectOfType<DBManager_DialogOnly>(true);
#endif
    }

    void OnEnable()  { StartCoroutine(Run()); }
    void OnDisable() { StopAllCoroutines(); }

    IEnumerator Run()
    {
        if (DB == null)
        {
#if UNITY_2023_1_OR_NEWER
            DB = Object.FindFirstObjectByType<DBManager_DialogOnly>(FindObjectsInactive.Include);
#else
            DB = Object.FindObjectOfType<DBManager_DialogOnly>(true);
#endif
        }
        while (DB == null || !DB.IsReady) yield return null;

        while (true)
        {
            if (!DB.Data.TryGetValue(cur, out var d)) yield break;

            // 이름
            nameText.text =
                d.name == "Narration" ? "" :
                d.name == "Player"    ? ((GameManager.instance != null) ? GameManager.instance.playerName : "Player") :
                d.name == "Jin Yein"  ? "진예인" :
                d.name == "Freyja"    ? "프레이야" :
                d.name == "Ru"        ? "루" : d.name;

            // 본문 타자 효과
            yield return StartCoroutine(TypeText(d.line));

            // 다음 진행
            bool auto = (GameManager.instance != null) ? (GameManager.instance.printSetting == 0) : false;
            if (auto)
            {
                yield return new WaitForSeconds(3f);
            }
            else
            {
                while (!Input.GetKeyDown(nextKey) && !Input.GetMouseButtonDown(0)) yield return null;
            }

            cur++;
        }
    }

    IEnumerator TypeText(string text)
    {
        // 전체 문장을 먼저 넣고, 보이는 글자수로 제어 (리치텍스트 안전)
        bodyText.text = text;
        bodyText.ForceMeshUpdate();
        int total = bodyText.textInfo.characterCount;
        bodyText.maxVisibleCharacters = 0;

        // ★ 디바운스: 시작 프레임 입력 무시 + 홀드 해제 대기
        yield return null;
        while (Input.GetMouseButton(0) || Input.GetKey(nextKey)) yield return null;

        float cps = (GameManager.instance != null) ? Mathf.Max(1f, (float)GameManager.instance.textPrintSpeed) : 30f;
        float delay = 1f / cps;
        var wait = new WaitForSeconds(delay);

        for (int i = 1; i <= total; i++)
        {
            // 스킵 입력 시 즉시 완성
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(nextKey))
            {
                bodyText.maxVisibleCharacters = total;
                yield break;
            }

            bodyText.maxVisibleCharacters = i;
            yield return wait;
        }
    }
}


