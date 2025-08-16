using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BlockX_controller : MonoBehaviour
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
    private int lengthCells = 2;       // 2칸 또는 3칸
    private int currentRow;            // 현재 행(고정)
    private bool dragging;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 길이 자동 추정 (폭 / cellSize 를 반올림)
        if (autoDetectLength)
        {
            float width = 0f;
            var col = GetComponent<BoxCollider2D>();
            if (col != null) width = col.bounds.size.x;
            else
            {
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null) width = sr.bounds.size.x * transform.localScale.x;
            }
            if (width > 0f)
                lengthCells = Mathf.Clamp(Mathf.RoundToInt(width / cellSize), 2, 3);
            else
                lengthCells = manualLengthCells;
        }
        else
        {
            lengthCells = manualLengthCells;
        }

        // 시작할 때 Y를 가장 가까운 행 중심으로 스냅(가로 블록은 행 고정)
        currentRow = NearestRowFromY(transform.position.y);
        Vector3 startPos = transform.position;
        startPos.y = RowCenterY(currentRow);
        transform.position = startPos;
    }

    void Update()
    {
        // 드래그 중이 아니면 속도 0 유지 (물리 떨림 방지)
        if (!dragging)
            body.linearVelocity = Vector2.zero;
    }

    void OnMouseDrag()
    {
        dragging = true;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float deltaX = transform.position.x;
        float dist = Mathf.Abs(mouseWorld.x - deltaX);

        float speed = (dist < deadZone) ? 0f : dist * speedFactor;
        body.linearVelocity = (mouseWorld.x > deltaX) ? Vector2.right * speed
                                                : Vector2.left * speed;

        // Y축은 고정(행 유지)
        Vector3 p = transform.position;
        p.y = RowCenterY(currentRow);
        transform.position = p;
    }

    void OnMouseUp()
    {
        dragging = false;

        // 1) 속도 정지
        body.linearVelocity = Vector2.zero;

        // 2) 현재 X를 가장 가까운 '열 중심'으로 스냅하되, 보드 밖으로 안 나가게 클램프
        float centerX = transform.position.x;
        int nearestCol = NearestColFromCenterX(centerX, lengthCells);
        nearestCol = Mathf.Clamp(nearestCol, 0, boardSize - lengthCells);

        // 3) 최종 스냅 좌표 적용 (Y는 행 중심, X는 (col + length/2) 중심)
        Vector3 snapped = new Vector3(
            ColCenterX(nearestCol, lengthCells),
            RowCenterY(currentRow),
            transform.position.z
        );
        transform.position = snapped;
    }

    // ====== 좌표 <-> 인덱스 유틸 ======

    // 특정 행의 '셀 중심' Y
    float RowCenterY(int row)
    {
        return originTopLeft.y - (row + 0.5f) * cellSize;
    }

    // 특정 열 시작 col에서 길이 len 블록의 '중앙' X
    float ColCenterX(int col, int len)
    {
        // 블록 중앙 = (col ~ col+len-1)의 중앙 = origin + (col + len/2) * cellSize
        return originTopLeft.x + (col + 0.5f * len) * cellSize;
    }

    // 현재 Y가 가장 가까운 행 인덱스
    int NearestRowFromY(float y)
    {
        float rel = (originTopLeft.y - y) / cellSize - 0.5f; // (row + 0.5) 을 역산
        return Mathf.Clamp(Mathf.RoundToInt(rel), 0, boardSize - 1);
    }

    // 현재 중앙 X에서 “블록의 시작 열(col)”을 역산
    int NearestColFromCenterX(float centerX, int len)
    {
        float rel = (centerX - originTopLeft.x) / cellSize - 0.5f * len; // (col + len/2) 역산
        return Mathf.RoundToInt(rel);
    }
}
