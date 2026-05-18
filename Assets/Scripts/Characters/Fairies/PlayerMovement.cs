using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Personagens")]
    public RuntimeAnimatorController bloomController;
    public RuntimeAnimatorController frostController;

    [Header("Efeito de troca (overlay)")]
    public Animator switchEffectAnimator;
    public string effectTrigger = "Play";

    private Animator anim;
    private Rigidbody2D rb;

    // sons
    private AudioSource[] sounds;
    private AudioSource jumpSound;
    private AudioSource switchSound;

    private bool isGrounded;
    private float moveInput;
    private bool isBloom = true;
    private bool isDead = false;

    private PlayerControlFlags controlFlags;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // pega os dois Audio Sources do player
        sounds = GetComponents<AudioSource>();

        // 0 = jump
        // 1 = transicao
        jumpSound = sounds[0];
        switchSound = sounds[1];

        anim.runtimeAnimatorController = bloomController;

        controlFlags = GetComponent<PlayerControlFlags>();
    }

    void Update()
    {
        if (isDead) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        bool canMove = controlFlags == null || controlFlags.canMove;
        bool canJump = controlFlags == null || controlFlags.canJump;
        bool canSwitch = controlFlags == null || controlFlags.canSwitchState;

        moveInput = 0f;

        if (canMove)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                moveInput = -1f;

            else if (Input.GetKey(KeyCode.RightArrow))
                moveInput = 1f;

            anim.SetFloat("Speed", Mathf.Abs(moveInput));

            if (moveInput > 0)
                transform.localScale = new Vector3(1, 1, 1);

            else if (moveInput < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            anim.SetFloat("Speed", 0f);
        }

        // pulo
        if (canJump && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            jumpSound.Play();
        }

        // troca de mundo
        if (canSwitch && Input.GetKeyDown(KeyCode.Q))
        {
            isBloom = !isBloom;

            anim.runtimeAnimatorController =
                isBloom ? bloomController : frostController;

            if (switchEffectAnimator != null)
                switchEffectAnimator.SetTrigger(effectTrigger);

            switchSound.Play();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.linearVelocity =
            new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        anim.SetTrigger("Die");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        PlayerPrefs.SetString(
            "LastLevel",
            SceneManager.GetActiveScene().name
        );

        PlayerPrefs.Save();

        StartCoroutine(LoadDeathMenu());
    }

    private IEnumerator LoadDeathMenu()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Menu_Derrota");
    }
}