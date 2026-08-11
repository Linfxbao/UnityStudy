using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyMove : MonoBehaviour
{
    [Header("移动")]
    // 移动速度
    [SerializeField] private float moveSpeed;
    // 地面阻力
    [SerializeField] private float groundDrag;

    [Header("跳跃")]
    // 跳跃力
    [SerializeField] private float jumpForce;
    // 跳跃冷却
    [SerializeField] private float jumpCooldown;
    //
    [SerializeField] private float airMultiplier;
    private bool readyToJump;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;

    [Header("地面检测相关")]
    // 角色高度
    [SerializeField] private float playerHeight;
    // 地面层级
    [SerializeField] private LayerMask whatIsGround;
    // 角色当前是否在地面
    private bool grounded;

    // 被移动角色
    [SerializeField] private Transform orientation;

    // 键盘输入
    private float horizontalInput;
    private float verticalInput;
    
    // 移动方向
    private Vector3 moveDirection;

    // 刚体
    [SerializeField] private Rigidbody rb;
    
    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        // 设置可以改变对象的旋转，如果为flase，对象会像倒下
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        // 向下检测对象是否在地面上
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + .3f, whatIsGround);

        OnInput();
        SpeedControl();

        if (grounded)
        {
            // 若在地面上，则设置阻力，使对象不会滑动
            rb.drag = groundDrag;
        } else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate ()
    {
        MovePlayer();
    }

    private void OnInput()
    {
        // GetAxisRaw和GetAxis的区别
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // 检测跳跃
        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    //移动对象
    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        } else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    // 设置对象移动最大速度
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    // 跳跃
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // 重置，使能够反复跳跃
    private void ResetJump()
    {
        readyToJump = true;
    }
}
