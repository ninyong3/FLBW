using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockX_controller : MonoBehaviour
{

    private Vector2 mousePosition; //마우스 위치
    private float deltaX, distance; //블럭의 x좌표, 거리
    public float speed; //블럭 이동속도
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
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //현재 마우스의 위치
        deltaX = transform.position.x; 
        distance = Mathf.Abs(mousePosition.x - deltaX); 

        if (distance < 0.05) //거리가 작으면 이동하지 않도록 속도를 0으로 설정
        {
            speed = 0;
        }
        else
        {
            speed = distance * 1.5f; //거리 * 1.5를 속도로 설정
        }

        if (mousePosition.x > deltaX) 
        {
            body.linearVelocity = Vector3.right * speed; 
        }
        else if (mousePosition.x < deltaX)
        {
            body.linearVelocity = Vector3.left * speed;
        }
    }

    private void OnMouseUp()
    {
        body.linearVelocity = new Vector3(0, 0, 0); //마우스에서 손을 떼면 블럭의 속도 0으로 설정(이후에 보드칸에 맞춰 이동하도록 재설정)
    }

}