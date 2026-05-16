using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Transform playerGroundCheck;
    public LayerMask groundLayer;

    [Header("Ponto de Retorno")]
    public Transform pontoDeRetorno;

    [Header("Área do Inimigo")]
    public BoxCollider2D areaDeMovimento;

    [Header("Colliders")]
    public BoxCollider2D corpoCollider;

    [Header("Controller Inicial")]
    public RuntimeAnimatorController[] controllersDisponiveis;
    public int controllerEscolhido = 0;

    [Header("Velocidades")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;

    [Header("Detecção")]
    public float detectionDistance = 6f;
    public float maxHeightDifference = 1.5f;

    [Header("Retorno")]
    public float returnThreshold = 0.05f;

    [Header("Idle antes de retornar")]
    public float idleDurationMin = 1f;
    public float idleDurationMax = 2f;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private EnemyPlatformFollow platformFollow;

    private int direction = 1;
    private float idleTimer = 0f;
    private float idleTarget = 0f;
    private bool isIdling = true;

    private bool usaAreaDeMovimento = false;

    private enum State
    {
        Idle,
        Chase,
        IdleBeforeReturn,
        Return
    }

    private State currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        platformFollow = GetComponent<EnemyPlatformFollow>();

        if (corpoCollider == null)
            corpoCollider = GetComponent<BoxCollider2D>();

        AplicarControllerEscolhido();
        ConfigurarAreaDeMovimento();
        ConfigurarPontoDeRetorno();

        currentState = State.Idle;
    }

    void FixedUpdate()
    {
        UpdateState();

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
            case State.IdleBeforeReturn:
                IdleBeforeReturn();
                break;
            case State.Return:
                ReturnToStart();
                break;
        }

        UpdateAnimation();
    }

    void ConfigurarAreaDeMovimento()
    {
        if (areaDeMovimento == null || corpoCollider == null)
        {
            usaAreaDeMovimento = false;
            Debug.LogWarning("AreaDeMovimento ou CorpoCollider não foi definido.");
            return;
        }

        areaDeMovimento.isTrigger = true;

        // NÃO soltamos mais a área do inimigo — ela fica filha
        // e acompanha a plataforma junto com ele.
        // areaDeMovimento.transform.SetParent(null); <- REMOVIDO

        usaAreaDeMovimento = true;
    }

    // Calcula os limites da área dinamicamente a cada consulta,
    // refletindo a posição atual da área (que se move com a plataforma).
    void GetAreaLimites(out float esqInimigo, out float dirInimigo,
                        out float esqArea,    out float dirArea)
    {
        Bounds area = areaDeMovimento.bounds;
        float metadeDoInimigo = corpoCollider.bounds.extents.x;

        esqInimigo = area.min.x + metadeDoInimigo;
        dirInimigo = area.max.x - metadeDoInimigo;
        esqArea    = area.min.x;
        dirArea    = area.max.x;
    }

    void ConfigurarPontoDeRetorno()
    {
        if (pontoDeRetorno == null)
            Debug.LogWarning("PontoDeRetorno não foi definido no EnemyAI.");
    }

    void AplicarControllerEscolhido()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        if (controllersDisponiveis == null || controllersDisponiveis.Length == 0)
            return;

        if (controllerEscolhido < 0 || controllerEscolhido >= controllersDisponiveis.Length)
            controllerEscolhido = 0;

        anim.runtimeAnimatorController = controllersDisponiveis[controllerEscolhido];
    }

    public void EscolherController(int index)
    {
        controllerEscolhido = index;
        AplicarControllerEscolhido();
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case State.Idle:
                if (PodeComecarPerseguicao())
                    currentState = State.Chase;
                break;
            case State.Chase:
                if (DevePararDePerseguir())
                    StartIdleBeforeReturn();
                break;
            case State.IdleBeforeReturn:
                if (PlayerDentroDaArea() && PlayerNaAlturaPermitida())
                    currentState = State.Chase;
                break;
            case State.Return:
                if (PlayerDentroDaArea() && PlayerNaAlturaPermitida())
                    currentState = State.Chase;
                break;
        }
    }

    bool PodeComecarPerseguicao()
    {
        if (player == null) return false;
        if (!PlayerDentroDaArea()) return false;
        if (!PlayerNaAlturaPermitida()) return false;

        float distance = Vector2.Distance(rb.position, player.position);
        if (distance > detectionDistance) return false;

        return true;
    }

    bool DevePararDePerseguir()
    {
        if (player == null) return true;
        if (!PlayerDentroDaArea()) return true;
        if (!PlayerNaAlturaPermitida()) return true;
        return false;
    }

    bool PlayerDentroDaArea()
    {
        if (player == null) return false;
        if (!usaAreaDeMovimento) return true;

        GetAreaLimites(out _, out _, out float esqArea, out float dirArea);

        return player.position.x >= esqArea && player.position.x <= dirArea;
    }

    bool PlayerNaAlturaPermitida()
    {
        if (player == null) return false;

        float heightDiff = Mathf.Abs(rb.position.y - player.position.y);
        return heightDiff <= maxHeightDifference;
    }

    Vector2 GetPlatformDelta()
    {
        if (platformFollow == null) return Vector2.zero;
        return platformFollow.PlatformDelta;
    }

    void Idle()
    {
        isIdling = true;
        Vector2 delta = GetPlatformDelta();
        rb.linearVelocity = new Vector2(delta.x / Time.fixedDeltaTime, rb.linearVelocity.y);
    }

    void Chase()
    {
        isIdling = false;
        direction = player.position.x > rb.position.x ? 1 : -1;
        FlipVisual();
        MoverDentroDaArea(direction, runSpeed);
    }

    void StartIdleBeforeReturn()
    {
        currentState = State.IdleBeforeReturn;
        isIdling = true;
        idleTimer = 0f;
        idleTarget = Random.Range(idleDurationMin, idleDurationMax);

        Vector2 delta = GetPlatformDelta();
        rb.linearVelocity = new Vector2(delta.x / Time.fixedDeltaTime, rb.linearVelocity.y);
    }

    void IdleBeforeReturn()
    {
        Vector2 delta = GetPlatformDelta();
        rb.linearVelocity = new Vector2(delta.x / Time.fixedDeltaTime, rb.linearVelocity.y);

        idleTimer += Time.fixedDeltaTime;
        if (idleTimer >= idleTarget)
        {
            currentState = State.Return;
            isIdling = false;
        }
    }

    void ReturnToStart()
    {
        isIdling = false;

        if (pontoDeRetorno == null)
        {
            currentState = State.Idle;
            return;
        }

        Vector2 alvo = pontoDeRetorno.position;
        float diff = alvo.x - rb.position.x;

        if (Mathf.Abs(diff) <= returnThreshold)
        {
            rb.position = new Vector2(alvo.x, rb.position.y);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            direction = 1;
            FlipVisual();
            currentState = State.Idle;
            return;
        }

        direction = diff > 0 ? 1 : -1;
        FlipVisual();

        Vector2 delta = GetPlatformDelta();
        float velX = direction * walkSpeed + delta.x / Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(velX, rb.linearVelocity.y);
    }

    void MoverDentroDaArea(int direcao, float velocidade)
    {
        GetAreaLimites(out float esqInimigo, out float dirInimigo, out _, out _);

        float proximoX = rb.position.x + direcao * velocidade * Time.fixedDeltaTime;

        if (usaAreaDeMovimento)
        {
            if (proximoX < esqInimigo)
            {
                rb.position = new Vector2(esqInimigo, rb.position.y);
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }

            if (proximoX > dirInimigo)
            {
                rb.position = new Vector2(dirInimigo, rb.position.y);
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }
        }

        Vector2 delta = GetPlatformDelta();
        float velX = direcao * velocidade + delta.x / Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(velX, rb.linearVelocity.y);
    }

    void FlipVisual()
    {
        if (sr != null)
            sr.flipX = direction == -1;
    }

    void UpdateAnimation()
    {
        if (anim == null) return;
        if (anim.runtimeAnimatorController == null) return;

        float speed = Mathf.Abs(rb.linearVelocity.x);
        anim.SetFloat("Speed", speed);
        anim.SetBool("IsIdle", isIdling);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.Die();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (areaDeMovimento != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(areaDeMovimento.bounds.center, areaDeMovimento.bounds.size);
        }

        if (pontoDeRetorno != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pontoDeRetorno.position, 0.12f);
        }
    }

    public void ResetEnemyState()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        currentState = State.Idle;
        isIdling = true;
        idleTimer = 0f;
        idleTarget = 0f;

        direction = 1;
        FlipVisual();
        UpdateAnimation();
    }

    public void RecalcularPontoDeRetorno(Transform novoPonto)
    {
        pontoDeRetorno = novoPonto;
    }
}