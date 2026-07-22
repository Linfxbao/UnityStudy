using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 move;
    private bool isPlatForm;
    private SpriteRenderer sr;
    private AudioManager audioManager;

    public Animator ani;

    [SerializeField]
    private float moveSpeed = 5f;

    public float jumpForce = 5f;
    public string landingFootstepsName = "LandingFootsteps";

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("Freak out! No audioManager in the scene!");
        }
    }

    void OnJump(InputValue value)
    {
        if (rb == null || !value.isPressed || !isPlatForm)
        {
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isPlatForm = false;

        if (ani != null)
        {
            ani.SetTrigger("Jump");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Platform"))
        {
            return;
        }

        isPlatForm = true;

        if (audioManager != null)
        {
            audioManager.PlaySound(landingFootstepsName);
        }
    }

    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
        move.y = 0f;
    }

    void Update()
    {
        if (sr != null)
        {
            if (move.x > 0f) sr.flipX = false;
            else if (move.x < 0f) sr.flipX = true;
        }

        if (ani != null)
        {
            ani.SetBool("Move", Mathf.Abs(move.x) > 0.1f);
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        float currentMoveSpeed = PlayerStats.instance != null ? PlayerStats.instance.moveSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(move.x * currentMoveSpeed, rb.linearVelocity.y);
    }

    void OnDisable()
    {
        move = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (ani != null)
        {
            ani.SetBool("Move", false);
        }
    }
}
