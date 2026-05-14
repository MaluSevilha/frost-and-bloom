using UnityEngine;

public class ControlledHorizontalPlatform : MonoBehaviour
{
    public float speed = 3f;
    public LayerMask collisionLayer;

    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        float direction = 0f;

        if (Input.GetKey(KeyCode.A))
            direction = -1f;
        else if (Input.GetKey(KeyCode.D))
            direction = 1f;

        Vector2 movement = new Vector2(direction * speed * Time.fixedDeltaTime, 0f);

        if (!VaiColidir(movement))
        {
            rb.MovePosition(rb.position + movement);
        }
    }

    bool VaiColidir(Vector2 movimento)
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            col.bounds.center,
            col.bounds.size,
            0f,
            movimento.normalized,
            movimento.magnitude,
            collisionLayer
        );

        return hit.collider != null;
    }
}