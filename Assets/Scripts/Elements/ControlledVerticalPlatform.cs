using UnityEngine;

public class ControlledVerticalPlatform : MonoBehaviour
{
    public float speed = 3f;

    [Header("Limites")]
    public string upperLimitTag = "UpperPlatformLimit";
    public string lowerLimitTag = "LowerPlatformLimit";

    private Rigidbody2D rb;
    private bool blockedUp = false;
    private bool blockedDown = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float direction = 0f;

        if (MobileInputState.Instance != null)
        {
            direction = MobileInputState.Instance.PlatformY;
        }
        else
        {
            if (Input.GetKey(KeyCode.W) && !blockedUp)
                direction = 1f;
            else if (Input.GetKey(KeyCode.S) && !blockedDown)
                direction = -1f;
        }

        if (blockedUp && direction > 0f)
            direction = 0f;

        if (blockedDown && direction < 0f)
            direction = 0f;

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

        if (other.CompareTag(lowerLimitTag))
        {
            blockedDown = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {   
        if (other.CompareTag(upperLimitTag))
        {
            blockedUp = false;
        }

        if (other.CompareTag(lowerLimitTag))
        {
            blockedDown = false;
        }
    }
}