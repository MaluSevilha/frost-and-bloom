using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public static System.Action OnPlayerJump;

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

    [Header("Input Mobile")]
    [SerializeField] private MobileInputState mobileInput;

    private Animator anim;
    private Rigidbody2D rb;

    [Header("Sons")]
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource switchSound;
    [SerializeField] private AudioSource deathSound;

    private bool isGrounded;
    private float moveInput;
    private bool isDead = false;

    private WorldStateManager worldStateManager;
    private PlayerControlFlags controlFlags;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        controlFlags = GetComponent<PlayerControlFlags>();

        if (mobileInput == null)
            mobileInput = FindFirstObjectByType<MobileInputState>();

        worldStateManager = FindFirstObjectByType<WorldStateManager>();

        if (worldStateManager != null)
        {
            worldStateManager.OnStateChanged += UpdateVisual;
            UpdateVisual(worldStateManager.CurrentState);
        }
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
            float keyboardInput = 0f;
            if (Input.GetKey(KeyCode.LeftArrow))
                keyboardInput -= 1f;
            if (Input.GetKey(KeyCode.RightArrow))
                keyboardInput += 1f;

            float touchInput = mobileInput != null ? mobileInput.PlayerMoveX : 0f;
            moveInput = Mathf.Abs(touchInput) > 0.01f ? touchInput : keyboardInput;

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

        bool jumpPressed =
            Input.GetKeyDown(KeyCode.Space) ||
            (mobileInput != null && mobileInput.ConsumeJump());

        if (canJump && jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (jumpSound != null)
                jumpSound.Play();

            OnPlayerJump?.Invoke();
        }

        bool switchPressed =
            Input.GetKeyDown(KeyCode.Q) ||
            (mobileInput != null && mobileInput.ConsumeSwitch());

        if (canSwitch && switchPressed)
        {
            if (WorldStateManager.Instance != null)
                WorldStateManager.Instance.ToggleState();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    private void OnDisable()
    {
        if (worldStateManager != null)
            worldStateManager.OnStateChanged -= UpdateVisual;
    }

    private void UpdateVisual(WorldState state)
    {
        ApplyWorldVisual(state);

        if (switchEffectAnimator != null)
            switchEffectAnimator.SetTrigger(effectTrigger);

        if (switchSound != null)
            switchSound.Play();
    }

    private void ApplyWorldVisual(WorldState state)
    {
        anim.runtimeAnimatorController =
            state == WorldState.Bloom ? bloomController : frostController;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        anim.SetTrigger("Die");

        if (deathSound != null)
            deathSound.Play();

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        PlayerPrefs.SetString("LastLevel", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        StartCoroutine(LoadDeathMenu());
    }

    private IEnumerator LoadDeathMenu()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Menu_Derrota");
    }
}