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

    private Animator anim;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private bool isBloom = true;
    private bool isDead = false;

    void Start()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = bloomController;
    }

    void Update()
    {
        if (isDead) return;

        moveInput  = Input.GetAxis("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        if (moveInput > 0)      transform.localScale = new Vector3( 1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isBloom = !isBloom;
            anim.runtimeAnimatorController = isBloom ? bloomController : frostController;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.SetTrigger("Die");
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