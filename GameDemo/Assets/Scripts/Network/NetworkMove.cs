using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkMove : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private void Update()
    {
        if (!IsOwner) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(horizontal, 0, vertical);

        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }

}
