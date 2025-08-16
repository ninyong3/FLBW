using System.Collections.Generic;
using UnityEngine;

public class Block_spawner : MonoBehaviour
{
    [Header("Recipe")]
    public Block_spawn_recipe recipeAsset;

    [Header("Prefabs (sizes are preconfigured in prefab)")]
    public GameObject goalPrefab; // R = horizontal len 2
    public GameObject x2Prefab;   // horizontal len 2
    public GameObject x3Prefab;   // horizontal len 3
    public GameObject y2Prefab;   // vertical   len 2
    public GameObject y3Prefab;   // vertical   len 3

    [Header("Board")]
    [Tooltip("좌상단 '셀 중앙' 월드 좌표")]
    public Vector2 originTopLeft = new Vector2(-2.5f, 2.5f);
    [Tooltip("한 칸의 월드 크기")]
    public float cellSize = 1f;
    public Transform container; // optional

    [Tooltip("Spawn 전에 자식 정리")]
    public bool clearChildrenOnSpawn = true;

    const int N = 6;

    void Start() => SpawnFromRandomRecipe();

    public void SpawnFromRandomRecipe()
    {
        if(recipeAsset == null) { Debug.LogError("recipeAsset 미지정"); return; }

        var parent = container != null ? container : transform;
        if (clearChildrenOnSpawn)
            for (int i = parent.childCount - 1; i >= 0; i--) Destroy(parent.GetChild(i).gameObject);

        var (grid, ok, idx) = recipeAsset.GetRandomGrid();
        if (!ok || grid == null) { Debug.LogError("유효한 레시피 없음"); return; }

        Debug.Log($"랜덤 선택된 Recipe Element: {idx}");

        bool[,] visited = new bool[N, N];

        for (int r = 0; r < N; r++)
            for (int c = 0; c < N; c++)
            {
                if (visited[r, c]) continue;
                char ch = grid[r, c];
                if (ch == '.' || ch == ' ') { visited[r, c] = true; continue; }

                var cells = CollectComponent(grid, r, c, ch, visited);

                // 바운딩 박스 기준으로 길이/방향 판정
                int minR = 99, maxR = -99, minC = 99, maxC = -99;
                float sumR = 0, sumC = 0;
                foreach (var p in cells)
                {
                    if (p.r < minR) minR = p.r;
                    if (p.r > maxR) maxR = p.r;
                    if (p.c < minC) minC = p.c;
                    if (p.c > maxC) maxC = p.c;
                    sumR += p.r; sumC += p.c;
                }
                int wCells = maxC - minC + 1;
                int hCells = maxR - minR + 1;

                // 프리팹 선택 (R은 가로2 강제)
                GameObject prefab = null;
                if (ch == 'R')
                {
                    if (!(hCells == 1 && wCells == 2))
                    { Debug.LogWarning("R은 가로 2칸이어야 함"); continue; }
                    prefab = goalPrefab;
                }
                else if (hCells == 1 && (wCells == 2 || wCells == 3))
                    prefab = (wCells == 2) ? x2Prefab : x3Prefab;
                else if (wCells == 1 && (hCells == 2 || hCells == 3))
                    prefab = (hCells == 2) ? y2Prefab : y3Prefab;
                else
                {
                    Debug.LogWarning($"'{ch}' 지원하지 않는 모양 {wCells}x{hCells}");
                    continue;
                }
                if (prefab == null) { Debug.LogError("프리팹 누락"); continue; }

                // ▸ 중심 좌표(평균)만 계산해서 배치 — 스케일은 프리팹 그대로
                float meanR = sumR / cells.Count; // 2칸 → n+0.5, 3칸 → 정수
                float meanC = sumC / cells.Count;
                Vector3 center = new Vector3(
                    originTopLeft.x + meanC * cellSize,
                    originTopLeft.y - meanR * cellSize,
                    0f
                );

                var go = Instantiate(prefab, center, Quaternion.identity, parent);
                go.name = $"{ch}_{(hCells == 1 ? "H" : "V")}{(hCells == 1 ? wCells : hCells)}";
            }
    }

    // 동일 문자 flood-fill
    struct Cell { public int r, c; public Cell(int r, int c) { this.r = r; this.c = c; } }
    List<Cell> CollectComponent(char[,] g, int sr, int sc, char ch, bool[,] visited)
    {
        var q = new Queue<Cell>(); var list = new List<Cell>();
        q.Enqueue(new Cell(sr, sc)); visited[sr, sc] = true;
        while (q.Count > 0)
        {
            var p = q.Dequeue(); list.Add(p);
            Try(p.r - 1, p.c); Try(p.r + 1, p.c); Try(p.r, p.c - 1); Try(p.r, p.c + 1);
        }
        return list;

        void Try(int r, int c)
        {
            if (r < 0 || r >= N || c < 0 || c >= N) return;
            if (visited[r, c]) return;
            if (g[r, c] != ch) return;
            visited[r, c] = true; q.Enqueue(new Cell(r, c));
        }
    }
}
