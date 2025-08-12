using System;
using System.Collections.Generic;
using UnityEngine;

public class Block_spawner : MonoBehaviour
{
    [Header("Grid")]
    public Transform origin;          // 보드 좌상단(모서리) 기준점
    public float cellSize = 1f;       // 셀 간격
    public bool invertY = true;       // 아래로 갈수록 row 증가면 true

    [Header("Prefabs")]
    public GameObject Block_X;    // 가로2
    public GameObject Block_X_3;  // 가로3
    public GameObject Block_Y;    // 세로2
    public GameObject Block_Y_3;  // 세로3
    // Box_Goal(레드카)는 씬에 미리 배치되어 있다고 가정. 여기선 생성하지 않음.

    [Header("Spawn Controls")]
    [Range(0, 20)] public int extraVehicles = 10;  // 생성할 추가 블록 수(레드카 제외)
    public Transform container;                    // 인스턴스 부모(선택)

    [Header("Len=2 Position Nudge (cells)")]
    public float len2HorizontalNudge = 0.5f; // 가로2는 중앙을 +0.5칸 보정
    public float len2VerticalNudge = 0.5f; // 세로2는 중앙을 +0.5칸 보정

    const int SIZE = 6; // 6x6 보드

    enum Orient { H, V }

    class Vehicle
    {
        public int row, col;    // head(좌/상단) 셀
        public Orient orient;   // 가로/세로
        public int len;         // 2 or 3
    }

    void Start()
    {
        SpawnRandomLayout();
    }

    public void SpawnRandomLayout()
    {
        // 부모 정리
        if (container != null)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
                Destroy(container.GetChild(i).gameObject);
        }

        // 점유 맵: -1=빈칸, 0=레드카(Box_Goal) 고정
        int[,] occ = new int[SIZE, SIZE];
        for (int r = 0; r < SIZE; r++)
            for (int c = 0; c < SIZE; c++)
                occ[r, c] = -1;

        // 레드카(Box_Goal) 고정 점유 (2,0)-(2,1)
        occ[2, 0] = 0;
        occ[2, 1] = 0;

        // 랜덤 배치
        System.Random rng = new System.Random();
        var spawned = new List<Vehicle>();
        int nextId = 1; // 1부터 사용자 블록

        for (int k = 0; k < extraVehicles; k++)
        {
            bool placed = false;
            for (int trial = 0; trial < 200 && !placed; trial++)
            {
                var orient = (rng.NextDouble() < 0.5) ? Orient.H : Orient.V;
                int len = (rng.NextDouble() < 0.35) ? 3 : 2;

                // 길이 3은 "중앙 인덱스"가 0 또는 SIZE(=6)가 되지 않도록: 중앙은 1..SIZE-2에 위치
                // 가로3: centerCol = col+1 ∈ [1..SIZE-2] -> col ∈ [0..SIZE-3] (자동 충족)
                // 세로3: centerRow = row+1 ∈ [1..SIZE-2] -> row ∈ [0..SIZE-3] (자동 충족)
                // → 그래도 명시적으로 중앙 인덱스 체크

                if (orient == Orient.H)
                {
                    int colMin = 0;
                    int colMax = SIZE - len; // inclusive start
                    int row = rng.Next(0, SIZE);
                    int col = rng.Next(colMin, colMax + 1);

                    // 중앙 인덱스(가로)는 col + len/2 (len=3 -> col+1). 1..SIZE-2 유지
                    int centerCol = col + (len / 2);
                    if (len == 3 && (centerCol <= 0 || centerCol >= SIZE - 0)) // 사용자가 0/6 언급해서 가드
                    {
                        continue; // 이론상 안 걸리지만 명시적으로 가드
                    }

                    if (CanPlace(occ, row, col, orient, len))
                    {
                        var v = new Vehicle { row = row, col = col, orient = orient, len = len };
                        spawned.Add(v);
                        Mark(occ, v, nextId++);
                        placed = true;
                    }
                }
                else // V
                {
                    int rowMin = 0;
                    int rowMax = SIZE - len;
                    int row = rng.Next(rowMin, rowMax + 1);
                    int col = rng.Next(0, SIZE);

                    int centerRow = row + (len / 2);
                    if (len == 3 && (centerRow <= 0 || centerRow >= SIZE - 0))
                    {
                        continue;
                    }

                    if (CanPlace(occ, row, col, orient, len))
                    {
                        var v = new Vehicle { row = row, col = col, orient = orient, len = len };
                        spawned.Add(v);
                        Mark(occ, v, nextId++);
                        placed = true;
                    }
                }
            }
        }

        // 인스턴스 생성
        foreach (var v in spawned)
        {
            GameObject prefab =
                (v.orient == Orient.H)
                    ? (v.len == 2 ? Block_X : Block_X_3)
                    : (v.len == 2 ? Block_Y : Block_Y_3);

            if (prefab == null)
            {
                Debug.LogError($"Prefab 누락: orient={v.orient}, len={v.len}");
                continue;
            }

            SpawnOne(prefab, v.row, v.col, v.orient == Orient.H, v.len);
        }
    }

    // ---------- 배치/검사 유틸 ----------
    bool CanPlace(int[,] occ, int row, int col, Orient o, int len)
    {
        if (o == Orient.H)
        {
            if (col + len - 1 >= SIZE) return false;
            for (int i = 0; i < len; i++)
                if (occ[row, col + i] != -1) return false;
        }
        else
        {
            if (row + len - 1 >= SIZE) return false;
            for (int i = 0; i < len; i++)
                if (occ[row + i, col] != -1) return false;
        }
        return true;
    }

    void Mark(int[,] occ, Vehicle v, int val)
    {
        if (v.orient == Orient.H)
            for (int i = 0; i < v.len; i++) occ[v.row, v.col + i] = val;
        else
            for (int i = 0; i < v.len; i++) occ[v.row + i, v.col] = val;
    }

    // ---------- 스폰(크기·콜라이더·좌표 보정 포함) ----------
    void SpawnOne(GameObject prefab, int row, int col, bool horizontal, int len)
    {
        var parent = container != null ? container : (origin != null ? origin : transform);
        var go = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);

        // A) 스케일 초기화
        go.transform.localScale = Vector3.one;

        // B) 목표 월드 크기(셀 단위 정확히)
        float targetW = (horizontal ? len : 1) * cellSize;
        float targetH = (horizontal ? 1 : len) * cellSize;

        // C) 스프라이트 크기에 맞춰 스케일 보정
        var sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            var cur = sr.bounds.size;            // 월드 기준
            var lossy = go.transform.lossyScale;
            float sx = (cur.x > 0f) ? (targetW / cur.x) * lossy.x : 1f;
            float sy = (cur.y > 0f) ? (targetH / cur.y) * lossy.y : 1f;
            go.transform.localScale = new Vector3(sx, sy, 1f);
        }

        // D) 콜라이더 강제 정렬
        var bc = go.GetComponent<BoxCollider2D>();
        if (bc != null)
        {
            bc.offset = Vector2.zero;
            bc.size = new Vector2(targetW, targetH);
        }

        // E) 좌상단 -> 중앙 좌표
        var basePos = origin != null ? origin.position : Vector3.zero;
        Vector3 topLeft = new Vector3(col * cellSize,
                                      (invertY ? -row : row) * cellSize,
                                      0f);
        Vector3 center = topLeft + new Vector3(targetW * 0.5f,
                                                (invertY ? -targetH : targetH) * 0.5f,
                                                0f);

        // F) ★ 2칸 블록은 중앙을 ±0.5칸 보정(칸 겹침 방지)
        if (len == 2)
        {
            if (horizontal)
                center.x += len2HorizontalNudge * cellSize; // 기본 +0.5칸
            else
            {
                float sign = invertY ? -1f : 1f;
                center.y += sign * len2VerticalNudge * cellSize; // 기본 +0.5칸
            }
        }

        // G) 배치(스냅)
        go.transform.position = Snap(basePos + center);
    }

    Vector3 Snap(Vector3 v, float snap = 0.0001f)
    {
        return new Vector3(
            Mathf.Round(v.x / snap) * snap,
            Mathf.Round(v.y / snap) * snap,
            v.z
        );
    }
}
