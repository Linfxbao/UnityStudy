using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("玩家基本信息")]
    public float health = 120f;
    public float presentHealth;
    public HealthBar healthBar;

    [Header("玩家移动")]
    public float playerSpeed = 3f;
    public float playerSprint = 6f;

    [Header("玩家镜头")]
    public Transform playerCamera;
    public GameObject endGameMenuUI;

    [Header("玩家动画")]
    public Animator ani;

    [Header("角色控制器")]
    public CharacterController cC;
    public GameObject playerDamage;

    [Header("玩家转向速度")]
    public float turnCalmTime = 0.1f;
    private float turnCalmVelocity;

    [Header("玩家重力")]
    public float gravity = -9.81f;

    [Header("玩家跳跃")]
    public float jumpRange = 1f;
    public Transform surfaceCheck;
    public float surfaceDistance = 0.4f;
    public LayerMask surfaceMask;
    private Vector3 velocity;
    private bool onSurface;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        presentHealth = health;

        healthBar.SetMaxHealth(presentHealth);
    }
    private void Update()
    {
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);

        if (onSurface && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        cC.Move(velocity * Time.deltaTime);

        playerMove(playerSpeed);
        Sprint(playerSprint);
        Jump();
    }

    public void playerMove(float movespeed)
    {
        float horizontal_axis = Input.GetAxisRaw("Horizontal");
        float vertical_axis = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal_axis, 0, vertical_axis).normalized;

        if (direction.magnitude >= 0.1f)
        {

            ani.SetBool("Idle", false);
            ani.SetBool("Walk", true);
            ani.SetBool("Running", false);
            ani.SetBool("RifleWalk", false);
            ani.SetBool("IdleAim", false);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            cC.Move(moveDirection * movespeed * Time.deltaTime);
        }
        else
        {
            ani.SetBool("Idle", true);
            ani.SetBool("Walk", false);
            ani.SetBool("Running", false);
        }
    }

    public void Jump()
    {
        if (Input.GetButtonDown("Jump") && onSurface)
        {
            ani.SetBool("Idle", false);
            ani.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpRange * -1 * gravity);
        }
        else
        {
            ani.SetBool("Idle", true);
            ani.ResetTrigger("Jump");
        }
    }

    private void Sprint(float movespeed)
    {
        if (Input.GetButton("Sprint") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) && onSurface)
        {
            
            float horizontal_axis = Input.GetAxisRaw("Horizontal");
            float vertical_axis = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal_axis, 0, vertical_axis).normalized;

            if (direction.magnitude >= 0.1f)
            {
                ani.SetBool("Walk", false);
                ani.SetBool("Running", true);

                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                transform.rotation = Quaternion.Euler(0, angle, 0);

                Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                cC.Move(moveDirection * movespeed * Time.deltaTime);
            } else
            {
                ani.SetBool("Walk", true);
                ani.SetBool("Running", false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        presentHealth -= damage;

        StartCoroutine(PlayerDamage());

        healthBar.SetHealth(presentHealth);

        if (presentHealth <= 0)
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        endGameMenuUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Object.Destroy(gameObject, 1f);
    }

    IEnumerator PlayerDamage()
    {
        playerDamage.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        playerDamage.SetActive(false);
    }
}
