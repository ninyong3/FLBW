using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BlockY_controller : MonoBehaviour
{
    [Header("Grid")]
    [Tooltip("보드 좌상단 '모서리'의 월드 좌표")]
    public Vector2 originTopLeft = new Vector2(-3f, 3f);
    [Tooltip("한 칸의 월드 크기")]
    public float cellSize = 1f;
    [Tooltip("보드 한 변의 칸 수")]
    public int boardSize = 6;

    [Header("Block")]
    [Tooltip("블록 길이를 자동으로 추정할지 여부 (BoxCollider2D 또는 SpriteRenderer 기준)")]
    public bool autoDetectLength = true;
    [Tooltip("자동 추정 해제 시, 수동 설정(2 또는 3)")]
    [Range(2, 3)] public int manualLengthCells = 2;

    [Header("Drag")]
    [Tooltip("속도 보정 계수 (마우스-블록 거리 x 이 값)")]
    public float speedFactor = 1.5f;
    [Tooltip("이 값보다 가까우면 정지로 간주")]
    public float deadZone = 0.05f;

    private Rigidbody2D body;
    private int lengthCells = 2;  // 2칸 또는 3칸
    private int currentCol;       // 현재 열(고정)
    private bool dragging;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 길이 자동 추정 (세로 블록은 높이 / cellSize)
        if (autoDetectLength)
        {
            float height = 0f;
            var colli = GetComponent<BoxCollider2D>();
            if (colli != null) height = colli.bounds.size.y;
            else
            {
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null) height = sr.bounds.size.y * transform.localScale.y;
            }
            if (height > 0f)
                lengthCells = Mathf.Clamp(Mathf.RoundToInt(height / cellSize), 2, 3);
            else
                lengthCells = manualLengthCells;
        }
        else
        {
            lengthCells = manualLengthCells;
        }

        // 시작할 때 X를 가장 가까운 열 중심으로 스냅(세로 블록은 열 고정)
        currentCol = NearestColFromX(transform.position.x);
        Vector3 startPos = transform.position;
        startPos.x = ColCenterX(currentCol);
        transform.position = startPos;
    }

    void Update()
    {
        if (!dragging)
            body.linearVelocity = Vector2.zero;
    }

    void OnMouseDrag()
    {
        dragging = true;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float deltaY = transform.position.y;
        float dist = Mathf.Abs(mouseWorld.y - deltaY);

        float speed = (dist < deadZone) ? 0f : dist * speedFactor;
        body.linearVelocity = (mouseWorld.y > deltaY) ? Vector2.up * speed
                                                : Vector2.down * speed;

        // X축은 고정(열 유지)
        Vector3 p = transform.position;
        p.x = ColCenterX(currentCol);
        transform.position = p;
    }

    void OnMouseUp()
    {
        dragging = false;
        body.linearVelocity = Vector2.zero;

        // 현재 중앙 Y → 가장 가까운 시작 행(row)으로 환산 후 클램프
        float centerY = transform.position.y;
        int nearestRow = NearestRowFromCenterY(centerY, lengthCells);
        nearestRow = Mathf.Clamp(nearestRow, 0, boardSize - lengthCells);

        // 최종 스냅 좌표 적용 (X는 열 중심, Y는 (row + len/2) 중앙)
        Vector3 snapped = new Vector3(
            ColCenterX(currentCol),
            RowCenterY(nearestRow, lengthCells),
            transform.position.z
        );
        transform.position = snapped;
    }

    // ====== 좌표 <-> 인덱스 유틸 ======

    // 열의 '셀 중심' X (단일 셀 중심)
    float ColCenterX(int col)
    {
        return originTopLeft.x + (col + 0.5f) * cellSize;
    }

    // 시작 행(row)에서 길이 len 블록의 '중앙' Y
    float RowCenterY(int row, int len)
    {
        // 블록 중앙 = (row ~ row+len-1)의 중앙 = origin - (row + len/2) * cellSize
        return originTopLeft.y - (row + 0.5f * len) * cellSize;
    }

    // 현재 X에서 가장 가까운 열 인덱스(단일 셀 중심 기준)
    int NearestColFromX(float x)
    {
        float rel = (x - originTopLeft.x) / cellSize - 0.5f; // (col + 0.5) 역산
        return Mathf.Clamp(Mathf.RoundToInt(rel), 0, boardSize - 1);
    }

    // 현재 중앙 Y에서 “블록의 시작 행(row)”을 역산
    int NearestRowFromCenterY(float centerY, int len)
    {
        float rel = (originTopLeft.y - centerY) / cellSize - 0.5f * len; // (row + len/2) 역산
        return Mathf.RoundToInt(rel);
    }
}
