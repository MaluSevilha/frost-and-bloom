using UnityEngine;

public class ControlledVerticalPlatform : MonoBehaviour
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

        if (Input.GetKey(KeyCode.W))
        {
            direction = -1f; // W desce
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction = 1f; // S sobe
        }

        Vector2 movement = new Vector2(0f, direction * speed * Time.fixedDeltaTime);

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