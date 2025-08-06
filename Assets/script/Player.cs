using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private GameObject player;
    [SerializeField]
    float speed = 2.5f;

    Transform tr;
    Rigidbody2D rb;

    void Start()
    {
        tr = player.GetComponent<Transform>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            tr.position += tr.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            tr.position += -tr.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            tr.position += tr.up * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            tr.position += -tr.up * speed * Time.deltaTime;
        }

    }
}