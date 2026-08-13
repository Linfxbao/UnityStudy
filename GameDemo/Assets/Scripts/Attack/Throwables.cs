using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwables : MonoBehaviour
{
    [Header("基础设置")]
    [SerializeField] private Transform eyesPos;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject objectToThrow;
    
    [Header("投掷物数据")]
    [SerializeField] private int totalThrows;
    [SerializeField] private float throwCooldown;
    [SerializeField] private float throwForce;
    [SerializeField] private float throwUpwardForce;

    bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        // 按下按键 && 能够投掷 && 投掷物数量大于0
        if (Input.GetKeyDown(KeyCode.Mouse0) && readyToThrow && totalThrows > 0)
        {
            OnThrow();
        }
    }

    private void OnThrow()
    {
        readyToThrow = false;

        // 生成物体并设置旋转
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, eyesPos.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = eyesPos.transform.forward;
        RaycastHit hit;

        // 计算投掷方向
        if (Physics.Raycast(eyesPos.position, eyesPos.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;

        Invoke(nameof(ResetThrow), throwCooldown);
        Destroy(projectile, 10f);
    }

    private void ResetThrow()
    {
        // 重置投掷
        readyToThrow = true;
    }

}
