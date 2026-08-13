using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("基础设置")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Rigidbody rb;
    private RigidbodyMove rm;

    [Header("滑行参数")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    [SerializeField] private float slideYScale;
    private float slideTimer;
    private float startYScale;

    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rm = GetComponent<RigidbodyMove>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // 按下F键且有移动输入，则开始滑行；松开F且正在滑行则停止滑行
        if (Input.GetKeyDown(KeyCode.F) && (horizontalInput != 0 || verticalInput != 0))
        {
            StartSlide();
        } else if (Input.GetKeyUp(KeyCode.F) && rm.sliding) {
            StopSlide();
        }

    }

    private void FixedUpdate()
    {
        if (rm.sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        rm.sliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);

        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        // 正在斜坡上滑行且向上滑行
        if (!rm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        } else
        {
            rb.AddForce(rm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer < 0)
        {
            StopSlide();
        }

    }

    private void StopSlide()
    {
        Debug.Log("停止滑行");
        rm.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);

    }

}
