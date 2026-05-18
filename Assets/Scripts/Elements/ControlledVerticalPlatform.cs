using UnityEngine;

public class ControlledVerticalPlatform : MonoBehaviour
{
    public float speed = 3f;

    [Header("Limites")]
    public string upperLimitTag = "UpperPlatformLimit";

    private Rigidbody2D rb;
    private bool blockedUp = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float direction = 0f;

        // SUBIR
        if (Input.GetKey(KeyCode.W) && !blockedUp)
        {
            direction = 1f;
        }

        // DESCER
        else if (Input.GetKey(KeyCode.S))
        {
            direction = -1f;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(upperLimitTag))
        {
            blockedUp = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(upperLimitTag))
        {
            blockedUp = false;
        }
    }
}