using UnityEngine;

public class EnemyPlatformFollow : MonoBehaviour
{
    private Rigidbody2D rb;

    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        DetectarPlataformaInicial();
    }

    void DetectarPlataformaInicial(){
        Collider2D col = GetComponent<Collider2D>();

        RaycastHit2D hit = Physics2D.BoxCast(
            col.bounds.center,
            col.bounds.size,
            0f,
            Vector2.down,
            0.1f
        );

        if (hit.collider != null && hit.transform.CompareTag("MovingPlatform"))
        {
            currentPlatform = hit.transform;
            lastPlatformPosition = currentPlatform.position;
        }
    }

    void FixedUpdate()
    {
        if (currentPlatform != null)
        {
            Vector3 delta = currentPlatform.position - lastPlatformPosition;
            rb.position += new Vector2(delta.x, delta.y);

            lastPlatformPosition = currentPlatform.position;
        }

        if (currentPlatform == null)
        {
            DetectarPlataformaInicial();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("MovingPlatform"))
        {
            if (IsStandingOnTop(collision))
            {
                currentPlatform = collision.transform;
                lastPlatformPosition = currentPlatform.position;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform == currentPlatform)
        {
            currentPlatform = null;
        }
    }

    bool IsStandingOnTop(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
                return true;
        }
        return false;
    }
}