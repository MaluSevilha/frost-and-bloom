using UnityEngine;

public class EnemyPlatformFollow : MonoBehaviour
{
    [SerializeField] private Rigidbody2D plataformaRb;

    private Rigidbody2D rb;
    private Vector2 lastPlatformPosition;
    private bool inicializado = false;

    public Vector2 PlatformDelta { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Inicializar();
    }

    void OnEnable()
    {
        // Toda vez que o objeto é ativado (inclusive a primeira vez
        // vinda de inativo), sincroniza lastPlatformPosition com a
        // posição atual da plataforma para não acumular delta falso.
        Inicializar();
        Debug.Log($"[PlatformFollow] OnEnable {gameObject.name} | plataformaRb: {(plataformaRb != null ? plataformaRb.position.ToString() : "NULL")} | lastPos: {lastPlatformPosition}");
    }

    void Inicializar()
    {
        if (plataformaRb != null)
        {
            lastPlatformPosition = plataformaRb.position;
            inicializado = true;
        }

        PlatformDelta = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (plataformaRb == null)
        {
            PlatformDelta = Vector2.zero;
            return;
        }

        // Se por algum motivo não inicializou ainda, faz agora.
        if (!inicializado)
            Inicializar();

        PlatformDelta = plataformaRb.position - lastPlatformPosition;

        if (PlatformDelta.magnitude > 0.01f)
            Debug.Log($"[PlatformFollow] {gameObject.name} delta: {PlatformDelta} | platPos: {plataformaRb.position} | lastPos: {lastPlatformPosition}");

        lastPlatformPosition = plataformaRb.position;
    }

    public void ResetFollow()
    {
        PlatformDelta = Vector2.zero;

        if (plataformaRb != null)
            lastPlatformPosition = plataformaRb.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("MovingPlatform"))
        {
            if (IsStandingOnTop(collision))
            {
                Rigidbody2D platRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (platRb != null && plataformaRb == null)
                {
                    plataformaRb = platRb;
                    lastPlatformPosition = plataformaRb.position;
                }
            }
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