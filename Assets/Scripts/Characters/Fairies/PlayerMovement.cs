using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    public RuntimeAnimatorController bloomController;
    public RuntimeAnimatorController frostController;

    private Animator animator;
    private bool isBloom = true;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = bloomController;
    }

    void Update()
    {
        // Movimento horizontal
        moveInput = Input.GetAxis("Horizontal");

        // Atualiza animação
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Confere se o player está no chão
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Flip do personagem
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Mudar entre Bloom e Frost
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isBloom = !isBloom;

            if (isBloom)
                animator.runtimeAnimatorController = bloomController;
            else
                animator.runtimeAnimatorController = frostController;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
}