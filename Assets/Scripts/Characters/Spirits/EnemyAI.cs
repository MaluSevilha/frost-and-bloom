using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Transform playerGroundCheck;
    public LayerMask groundLayer;

    [Header("Velocidades")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;

    [Header("Detecção")]
    public float detectionDistance = 6f;
    public float loseChaseDistance = 8f;

    [Header("Patrulha")]
    public float patrolDistance = 3f;
    public float returnThreshold = 0.2f;

    [Header("Idle")]
    public float idleDurationMin = 1f;
    public float idleDurationMax = 2f;

    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private SpriteRenderer sr;

    private Vector2 startPosition;
    private int direction = 1;
    private float idleTimer = 0f;
    private float idleTarget = 0f;
    private bool isIdling = false;

    private enum State { Patrol, Chase, IdleBeforeReturn, Return }
    private State currentState;

    void Start()
    {
        rb       = GetComponent<Rigidbody2D>();
        col      = GetComponent<BoxCollider2D>();
        anim     = GetComponent<Animator>();
        sr       = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        currentState  = State.Patrol;
    }

    void FixedUpdate()
    {
        UpdateState();

        switch (currentState)
        {
            case State.Patrol:           Patrol();            break;
            case State.Chase:            Chase();             break;
            case State.IdleBeforeReturn: IdleBeforeReturn();  break;
            case State.Return:           ReturnToStart();     break;
        }

        UpdateAnimation();
    }

    // DETECÇÃO DE MESMA PLATAFORMA 
    bool PlayerOnSamePlatform()
    {
        // diferença de altura entre os dois
        float heightDiff = Mathf.Abs(transform.position.y - player.position.y);
        if (heightDiff > 1.2f) return false;

        // GroundCheck do próprio player
        bool playerGrounded = Physics2D.Raycast(
            playerGroundCheck.position, Vector2.down, 0.15f, groundLayer
        );

        return playerGrounded;
    }

    // MÁQUINA DE ESTADOS
    void UpdateState()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                if (PlayerOnSamePlatform() && distance < detectionDistance)
                    currentState = State.Chase;
                break;

            case State.Chase:
                if (!PlayerOnSamePlatform() || distance > loseChaseDistance)
                    StartIdleBeforeReturn();
                break;
        }
    }

    // PATROL
    void Patrol()
    {
        isIdling = false;

        float left  = startPosition.x - patrolDistance;
        float right = startPosition.x + patrolDistance;

        if (direction == 1  && transform.position.x >= right) direction = -1;
        if (direction == -1 && transform.position.x <= left)  direction =  1;

        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);
        FlipVisual();
    }

    // CHASE
    void Chase()
    {
        isIdling = false;

        int chaseDir = (player.position.x > transform.position.x) ? 1 : -1;
        direction = chaseDir;
        FlipVisual();

        if (!IsGroundAhead())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(direction * runSpeed, rb.linearVelocity.y);
    }

    // IDLE ANTES DE RETORNAR
    void StartIdleBeforeReturn()
    {
        currentState = State.IdleBeforeReturn;
        isIdling     = true;
        idleTimer    = 0f;
        idleTarget   = Random.Range(idleDurationMin, idleDurationMax);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void IdleBeforeReturn()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        idleTimer += Time.fixedDeltaTime;
        if (idleTimer >= idleTarget)
        {
            isIdling     = false;
            currentState = State.Return;
        }
    }

    // RETURN
    void ReturnToStart()
    {
        isIdling = false;
        float diff = startPosition.x - transform.position.x;

        if (Mathf.Abs(diff) < returnThreshold)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            direction    = 1;
            currentState = State.Patrol;
            return;
        }

        direction = (diff > 0) ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);
        FlipVisual();
    }

    // HELPERS

    // Raycast de borda
    bool IsGroundAhead()
    {
        float offsetX = direction * (col.bounds.extents.x + 0.12f);
        float offsetY = -(col.bounds.extents.y);          // nível do pé

        Vector2 origin = new Vector2(
            col.bounds.center.x + offsetX,
            col.bounds.center.y + offsetY
        );

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.25f, groundLayer);
        return hit.collider != null;
    }

    void FlipVisual()
    {
        if (sr != null) sr.flipX = (direction == -1);
    }

    void UpdateAnimation()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);
        anim.SetFloat("Speed", speed);
        anim.SetBool("IsIdle", isIdling);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
            if (pm != null) pm.Die();
        }
    }
    
}