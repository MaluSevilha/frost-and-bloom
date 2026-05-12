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

    private Vector2 posicaoDeRetorno;

    private int direction = 1;
    private float idleTimer = 0f;
    private float idleTarget = 0f;
    private bool isIdling = true;

    private float limiteEsquerdoInimigo;
    private float limiteDireitoInimigo;
    private float limiteEsquerdoArea;
    private float limiteDireitoArea;
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

        // A área precisa ficar parada no mundo.
        // Se ela estiver como filha do inimigo, ela será solta no Start.
        areaDeMovimento.transform.SetParent(null);

        Bounds area = areaDeMovimento.bounds;
        float metadeDoInimigo = corpoCollider.bounds.extents.x;

        // Limites para o corpo do inimigo não sair da área.
        limiteEsquerdoInimigo = area.min.x + metadeDoInimigo;
        limiteDireitoInimigo = area.max.x - metadeDoInimigo;

        // Limites reais da área, usados para saber se o player está dentro dela.
        limiteEsquerdoArea = area.min.x;
        limiteDireitoArea = area.max.x;

        usaAreaDeMovimento = true;
    }

    void ConfigurarPontoDeRetorno()
    {
        if (pontoDeRetorno != null)
        {
            // Se o ponto estiver como filho do inimigo,
            // ele precisa ficar parado no mundo.
            pontoDeRetorno.SetParent(null);
            posicaoDeRetorno = pontoDeRetorno.position;
        }
        else
        {
            posicaoDeRetorno = rb.position;

            Debug.LogWarning(
                "PontoDeRetorno não foi definido. Usando a posição inicial do inimigo."
            );
        }
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
                // Se o player voltar para a área durante a pausa,
                // o inimigo volta a perseguir.
                if (PlayerDentroDaArea() && PlayerNaAlturaPermitida())
                    currentState = State.Chase;
                break;

            case State.Return:
                // Se o player entrar de novo na área enquanto o inimigo retorna,
                // ele volta a perseguir.
                if (PlayerDentroDaArea() && PlayerNaAlturaPermitida())
                    currentState = State.Chase;
                break;
        }
    }

    bool PodeComecarPerseguicao()
    {
        if (player == null)
            return false;

        if (!PlayerDentroDaArea())
            return false;

        if (!PlayerNaAlturaPermitida())
            return false;

        float distance = Vector2.Distance(rb.position, player.position);

        if (distance > detectionDistance)
            return false;

        return true;
    }

    bool DevePararDePerseguir()
    {
        if (player == null)
            return true;

        // Depois que começou a perseguir, NÃO para por distância.
        // Só para se o player sair da área do inimigo ou sair da altura permitida.

        if (!PlayerDentroDaArea())
            return true;

        if (!PlayerNaAlturaPermitida())
            return true;

        return false;
    }

    bool PlayerDentroDaArea()
    {
        if (player == null)
            return false;

        if (!usaAreaDeMovimento)
            return true;

        return player.position.x >= limiteEsquerdoArea &&
               player.position.x <= limiteDireitoArea;
    }

    bool PlayerNaAlturaPermitida()
    {
        if (player == null)
            return false;

        float heightDiff = Mathf.Abs(rb.position.y - player.position.y);

        return heightDiff <= maxHeightDifference;
    }

    void Idle()
    {
        isIdling = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
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

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void IdleBeforeReturn()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

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

        float diff = posicaoDeRetorno.x - rb.position.x;

        if (Mathf.Abs(diff) <= returnThreshold)
        {
            rb.position = new Vector2(posicaoDeRetorno.x, rb.position.y);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            direction = 1;
            FlipVisual();

            currentState = State.Idle;
            return;
        }

        direction = diff > 0 ? 1 : -1;
        FlipVisual();

        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);
    }

    void MoverDentroDaArea(int direcao, float velocidade)
    {
        float proximoX = rb.position.x + direcao * velocidade * Time.fixedDeltaTime;

        if (usaAreaDeMovimento)
        {
            if (proximoX < limiteEsquerdoInimigo)
            {
                rb.position = new Vector2(limiteEsquerdoInimigo, rb.position.y);
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }

            if (proximoX > limiteDireitoInimigo)
            {
                rb.position = new Vector2(limiteDireitoInimigo, rb.position.y);
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }
        }

        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);
    }

    void FlipVisual()
    {
        if (sr != null)
            sr.flipX = direction == -1;
    }

    void UpdateAnimation()
    {
        if (anim == null)
            return;

        if (anim.runtimeAnimatorController == null)
            return;

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
}