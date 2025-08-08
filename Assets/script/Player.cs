using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    float speed = 2.5f;

    private Transform tr;
    Rigidbody2D rb;

    void Start()
    {
        tr = player.GetComponent<Transform>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 moveDir = Vector2.zero;

        if (Input.GetKey(KeyCode.D))
            moveDir += Vector2.right;
        if (Input.GetKey(KeyCode.A))
            moveDir += Vector2.left;
        if (Input.GetKey(KeyCode.W))
            moveDir += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            moveDir += Vector2.down;

        moveDir = moveDir.normalized;

        // 이동
        if (moveDir != Vector2.zero)
        {
            tr.position += (Vector3)(moveDir * speed * Time.deltaTime);

            // 회전 (정수리 기준이 위쪽이므로 90도 보정)
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            tr.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }
    }
}
