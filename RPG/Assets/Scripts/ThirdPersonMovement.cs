using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpHeight = 3f;
    [Header("External References")]
    [SerializeField] private Transform cam;

    private CharacterController controller;
    private Animator anim;
    private Vector2 movementValue;
    private float jumpValue;
    private bool isGrounded;
    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool canMove = true;
    private bool canJump = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (canMove) Move();

        if (canJump) Jump();
                
        SimulateGravity();
    }

    private void OnMove(InputValue value)
    {
        movementValue = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        jumpValue = value.Get<float>();
    }

    private void Move()
    {
        float horizontal = movementValue.x;
        float vertical = movementValue.y;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        anim.SetFloat("MoveMagnitude", direction.magnitude);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    private void Jump()
    {
        if (jumpValue == 1.0f && isGrounded)
        {
            anim.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void SimulateGravity()
    {
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
