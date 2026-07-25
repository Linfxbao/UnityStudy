using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie1 : MonoBehaviour
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
    public GameObject[] walkPoints;
    int curIndex = 0;
    public float zombieSpeed;
    float walkingPointRadius = 2;

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

        if (!isInvision && !isCanAttack) Guard();
        if (isInvision && !isCanAttack) RunToPlayer();
        if (isInvision && isCanAttack) AttackPlayer();
    }

    private void Guard()
    {
        if (Vector3.Distance(walkPoints[curIndex].transform.position, transform.position) < walkingPointRadius)
        {
            curIndex = Random.Range(0, walkPoints.Length);
            if (curIndex >= walkPoints.Length)
            {
                curIndex = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, walkPoints[curIndex].transform.position, Time.deltaTime * zombieSpeed);

        transform.LookAt(walkPoints[curIndex].transform.position);
        ani.SetBool("Walking", true);
        ani.SetBool("Running", false);
        ani.SetBool("Attacking", false);
        ani.SetBool("Died", false);
    }

    private void RunToPlayer()
    {
        if (zombieAgent.SetDestination(player.position))
        {
            ani.SetBool("Walking", false);
            ani.SetBool("Running", true);
            ani.SetBool("Attacking", false);
            ani.SetBool("Died", false);
        }
        else
        {
            ani.SetBool("Walking", false);
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
            Debug.Log(Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, attackingRadius));
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, attackingRadius))
            {
                Debug.Log("Attacking " + hitInfo.transform.name);

                PlayerScript player = hitInfo.transform.GetComponent<PlayerScript>();

                if (player != null)
                {
                    player.TakeDamage(hitDamage);
                }
                ani.SetBool("Walking", false);
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
            ani.SetBool("Walking", false);
            ani.SetBool("Running", false);
            ani.SetBool("Attacking", false);
            ani.SetBool("Died", true);
            Debug.LogError("设置僵尸死亡");
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
