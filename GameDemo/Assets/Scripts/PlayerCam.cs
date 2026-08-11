using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("灵敏度")]
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    public Transform orientation;

    // X轴Y轴方向旋转
    private float xRotation;
    private float yRotation;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // 读取旋转
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // 设置旋转
        yRotation += mouseX;
        xRotation -= mouseY;

        // X轴最多旋转180度(上下旋转90度)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 相机旋转
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        // 对象位置旋转
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

}