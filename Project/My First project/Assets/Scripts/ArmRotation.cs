using UnityEngine;
using UnityEngine.InputSystem;

public class ArmRotation : MonoBehaviour
{
    [Header("旋转设置")]
    public Vector2 pivotOffsetLocal = Vector2.zero; // 武器握把相对当前物体原点的局部偏移
    public float angleOffset = 0f; // 贴图朝向补偿角度（例如默认朝上可填 -90）

    private Transform arm;
    private Camera mainCam;

    void Awake()
    {
        arm = transform;
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null) return;
        }

        if (Mouse.current == null) return;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        mouseScreenPos.z = Mathf.Abs(arm.position.z - mainCam.transform.position.z);
        Vector3 mousePosition = mainCam.ScreenToWorldPoint(mouseScreenPos);

        // 旋转前：计算当前“握把点”世界坐标
        Vector3 pivotWorldBefore = arm.TransformPoint((Vector3)pivotOffsetLocal);

        Vector2 direction = mousePosition - pivotWorldBefore;
        if (direction.sqrMagnitude < 0.0001f) return;

        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;
        arm.rotation = Quaternion.Euler(0f, 0f, rotZ);

        // 旋转后回推位置，让握把点保持不动，实现“绕握把旋转”
        Vector3 pivotWorldAfter = arm.TransformPoint((Vector3)pivotOffsetLocal);
        arm.position += pivotWorldBefore - pivotWorldAfter;
    }
}