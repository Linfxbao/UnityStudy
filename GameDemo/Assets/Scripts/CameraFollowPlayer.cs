using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player;

    private Vector3 offset;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("缺少跟随对象");
        }

        offset = transform.position - player.position;

    }

    void Update()
    {
        transform.position = player.position + offset;
    }
}
