using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyMove : MonoBehaviour
{
    [Header("移动")]
    // 移动速度
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float slideSpeed;

    // 期望速度
    private float desiredMoveSpeed;
    // 上一个期望速度
    private float lastDesiredMoveSpeed;
    // 速度变化率
    [SerializeField] private float speedIncreaseMultiplier;
    [SerializeField] private float slopeIncreaseMultiplier;

    // 地面阻力
    [SerializeField] private float groundDrag;

    // 玩家当前状态
    private MovementStates playerState;
    private enum MovementStates
    {
        walk,
        sprint,
        fly,
        crouch,
        slide,
    }

    // 正在滑行
    [HideInInspector]
    public bool sliding;

    [Header("下蹲")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYscale;
    private float startYScale;

    [Header("跳跃")]
    // 跳跃力
    [SerializeField] private float jumpForce;
    // 跳跃冷却
    [SerializeField] private float jumpCooldown;
    //
    [SerializeField] private float airMultiplier;
    private bool readyToJump;



    [Header("地面检查")]
    // 角色高度
    [SerializeField] private float playerHeight;
    // 地面层级
    [SerializeField] private LayerMask whatIsGround;
    // 角色当前是否在地�?
    private bool grounded;

    [Header("斜坡")]
    // 斜坡最大角度，再大就不算作斜坡
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

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

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // 向下检测对象是否在地面
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + .3f, whatIsGround);

        OnInput();
        SpeedControl();
        StateHandler();

        if (grounded)
        {
            // 若在地面上，则设置阻力，使对象不会滑
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

    private void StateHandler()
    {
        // 根据玩家当前状态设置移动速度
        if (sliding)
        {
            playerState = MovementStates.slide;
            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }else if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            playerState = MovementStates.sprint;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (grounded && Input.GetKey(KeyCode.LeftControl))
        {
            playerState = MovementStates.crouch;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (grounded)
        {
            playerState = MovementStates.walk;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            playerState = MovementStates.fly;
        }

        // 如果当前速度不为0且本次期望速度与上一次期望速度之差大于4，则将速度均匀减少/增加，否则移动速度为当前期望速度
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        } else
        {
            moveSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    // 速度平滑变化
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            } else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void OnInput()
    {
        // GetAxisRaw和GetAxis的区别
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // 检测跳跃
        if (Input.GetKeyDown(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // 检测下蹲
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYscale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    //移动对象
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        } else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    // 设置对象移动最大速度
    private void SpeedControl()
    {
        // 防止在斜坡上速度更快
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }

    // 跳跃
    private void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // 重置，使能够反复跳跃
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    // 检测当前是否在斜坡上
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    //调整在斜坡上的受力方向
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
