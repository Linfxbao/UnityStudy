using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("基础设置")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float rotationSpeed;

    [Header("视角")]
    [SerializeField] private GameObject thirdPersonCam;
    [SerializeField] private GameObject combatCam;
    [SerializeField] private GameObject topDownCam;

    // 视角类型
    [SerializeField] private Transform combatLookAt;
    private CameraStyle currentStyle;
    private enum CameraStyle
    {
        Basic,
        Combat,
        Topdown,
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // 获得角色视角朝向
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // 根据不同视角，用不同的移动玩家旋转角度方法
        if (currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        } else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;
        }

        

    }

    private void SwitchCamera(CameraStyle currentStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);
        switch (currentStyle)
        {
            default:
            case CameraStyle.Basic:
                thirdPersonCam.SetActive(true);
                break;
            case CameraStyle.Topdown:
                topDownCam.SetActive(true);
                break;
            case CameraStyle.Combat:
                combatCam.SetActive(true);
                break;
        }
    }

}
