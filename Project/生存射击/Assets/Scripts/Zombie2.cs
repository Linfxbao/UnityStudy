using UnityEngine;
using UnityEngine.AI;

public class Zombie2 : MonoBehaviour
{
    [Header("敌人基本信息")]
    private float zombieHealth = 100f;
    private float presentHealth;
    public float hitDamage = 5f;
    public HealthBar healthBar;

    [Header("敌人组件")]
    public NavMeshAgent zombieAgent;
    public Transform LookPoint;
    public Camera cam;
    public LayerMask playerLayer;
    public Transform player;

    [Header("攻击变量")]
    public float timeBtwAttack;
    bool previouslyAttack;

    [Header("敌人动画")]
    public Animator ani;

    [Header("敌人移动设置")]
    
    public float zombieSpeed;
    

    [Header("敌人状态")]
    public float visionRadius;
    public float attackingRadius;
    public bool isInvision;
    public bool isCanAttack;

    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
        presentHealth = zombieHealth;
        healthBar.SetMaxHealth(presentHealth);
    }

    private void Update()
    {
        isInvision = Physics.CheckSphere(transform.position, visionRadius, playerLayer);
        isCanAttack = Physics.CheckSphere(transform.position, attackingRadius, playerLayer);

        if (!isInvision && !isCanAttack) Idle();
        if (isInvision && !isCanAttack) RunToPlayer();
        if (isInvision && isCanAttack) AttackPlayer();
    }

    private void Idle()
    {
        zombieAgent.SetDestination(transform.position);
        ani.SetBool("Idle", true);
        ani.SetBool("Running", false);
    }

    private void RunToPlayer()
    {
        if (zombieAgent.SetDestination(player.position))
        {
            ani.SetBool("Idle", false);
            ani.SetBool("Running", true);
            ani.SetBool("Attacking", false);
            ani.SetBool("Died", false);
        }
        else
        {
            ani.SetBool("Idle", false);
            ani.SetBool("Running", false);
            ani.SetBool("Attacking", false);
            ani.SetBool("Died", true);
        }
    }

    private void AttackPlayer()
    {
        zombieAgent.SetDestination(transform.position);
        transform.LookAt(LookPoint);
        if (!previouslyAttack)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, attackingRadius))
            {
                Debug.Log("Attacking " + hitInfo.transform.name);

                PlayerScript player = hitInfo.transform.GetComponent<PlayerScript>();

                if (player != null)
                {
                    player.TakeDamage(hitDamage);
                }

                ani.SetBool("Idle", false);
                ani.SetBool("Running", false);
                ani.SetBool("Attacking", true);
                ani.SetBool("Died", false);
            }

            previouslyAttack = true;
            Invoke(nameof(ActiveAttacking), timeBtwAttack);
        }
    }

    private void ActiveAttacking()
    {
        previouslyAttack = false;
    }

    public void zombieHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;

        healthBar.SetHealth(presentHealth);
        Debug.LogError("僵尸当前血量: " + presentHealth);

        if (presentHealth <= 0) 
        {
            ani.SetBool("Idle", false);
            ani.SetBool("Running", false);
            ani.SetBool("Attacking", false);
            ani.SetBool("Died", true);
            zombieDie();
        }
    }

    private void zombieDie()
    {
        zombieAgent.SetDestination(transform.position);
        zombieSpeed = 0f;
        attackingRadius = 0f;
        visionRadius = 0f;
        isInvision = false;
        isCanAttack = false;

        Object.Destroy(gameObject, 5f);
    }
}
