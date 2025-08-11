using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockY_controller : MonoBehaviour
{
    private Vector2 mousePosition;   // 마우스 위치
    private float deltaY, distance;  // 블럭의 y좌표, 거리
    public float speed;              // 블럭 이동속도
    public Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    }

    private void OnMouseDrag()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 현재 마우스 위치
        deltaY = transform.position.y;
        distance = Mathf.Abs(mousePosition.y - deltaY);

        if (distance < 0.05f) // 거리가 작으면 이동하지 않도록 속도 0
            speed = 0f;
        else
            speed = distance * 1.5f; // 거리 * 1.5 를 속도로

        if (mousePosition.y > deltaY)
            body.linearVelocity = Vector3.up * speed;
        else if (mousePosition.y < deltaY)
            body.linearVelocity = Vector3.down * speed;
    }

    private void OnMouseUp()
    {
        body.linearVelocity = Vector3.zero; // 마우스에서 떼면 정지
    }
}
