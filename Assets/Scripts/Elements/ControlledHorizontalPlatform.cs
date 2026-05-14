using UnityEngine;

public class ControlledHorizontalPlatform : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float direction = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            direction = -1f; // A vai para a esquerda
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = 1f; // D vai para a direita
        }

        Vector2 movement = new Vector2(direction * speed * Time.fixedDeltaTime, 0f);

        if (rb != null)
        {
            rb.MovePosition(rb.position + movement);
        }
        else
        {
            transform.position += (Vector3)movement;
        }
    }
}