using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class CharacterControllerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private CharacterController characterController;

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

}
