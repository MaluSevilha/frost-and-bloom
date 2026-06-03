using UnityEngine;

public class ControlledHorizontalPlatform : MonoBehaviour
{
    public float speed = 3f;
    public LayerMask collisionLayer;

    private Rigidbody2D rb;
    private CompositeCollider2D compositeCol;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Usa o CompositeCollider2D explicitamente,
        // evitando pegar o TilemapCollider2D por engano.
        compositeCol = GetComponent<CompositeCollider2D>();
    }

    void FixedUpdate()
    {
        float direction = 0f;

        if (MobileInputState.Instance != null)
        {
            direction = MobileInputState.Instance.PlatformX;
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
                direction = -1f;
            else if (Input.GetKey(KeyCode.D))
                direction = 1f;
        }

        if (direction == 0f) return;

        Vector2 movement = new Vector2(direction * speed * Time.fixedDeltaTime, 0f);

        if (!VaiColidir(movement))
        {
            rb.MovePosition(rb.position + movement);
        }
    }

    bool VaiColidir(Vector2 movimento)
    {
        if (compositeCol == null) return false;

        // Usa os bounds do CompositeCollider2D com leve redução
        // para evitar self-detection nas bordas.
        Vector2 sizeReduzido = compositeCol.bounds.size * 0.95f;

        RaycastHit2D hit = Physics2D.BoxCast(
            compositeCol.bounds.center,
            sizeReduzido,
            0f,
            movimento.normalized,
            movimento.magnitude + 0.05f,
            collisionLayer
        );

        return hit.collider != null;
    }
}