using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Hareket : MonoBehaviour
{
    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;

    public Transform ground;
    public float distance = 0.3f;

    public float speed;
    public float jumpHeight;
    public float gravity;

    public LayerMask mask;
    { controller = GetComponent<CharacterController>();

    }



private void Update()
{
    #region Movement
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");
    Vector3 move = Transform.right * horizontal + Transform.forward * vertical;
    Controller.Move(move * Speed * Time.deltaTime);
    #endregion

    #region jump
    if(Input.GetKeyDown(KeyCode.Space) && IsGrounded)
    {
        //Jump
    }
    #endregion Gravity
    IsGrounded = Physics.CheckSphere(ground.position, distance, mask);

    if(isGrounded && velocity)

    #endregion

}

} 