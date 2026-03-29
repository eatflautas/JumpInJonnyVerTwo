using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public MovementState state;
    
    private float currentSpeed = 5.0f;
    private float goalSpeed = 5.0f;
    private float lastGoalSpeed = 5.0f;
    
    // Player speeds
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 7.0f;
    public float crouchSpeed = 3.5f;
    public float slideSpeed = 3.0f;
    
    // Heights
    public float crouchHeight = 0.5f;
    private float startHeight = 2.0f;
    public float jumpHeight = 1.5f;
    
    public float gravityValue = -9.81f;
    public float skinWidth = .05f;
    
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Vector3 moveDirection;
    private Vector3 slideVelocity;
    private bool groundedPlayer;
    private bool isCrouching;
    private bool isSprinting;

    [Header("References")] 
    public Transform orientation;
    
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        controller = gameObject.AddComponent<CharacterController>();
    }

    
    void Update()
    {
        Slide();
        StateSetter();
        MovePlayer();
    }

    void MovePlayer()
    {

        // Read input
        Vector2 input = InputManager.instance.MoveInput;
        bool jumpTriggered = InputManager.instance.JumpJustPressed;
        

        // Grounded check
        groundedPlayer = controller.isGrounded;

        RaycastHit hit;
        // thank you norm_thegamer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.3f))
        {
            playerVelocity.y = -5f;
        }
        
        
        // Crouch
        controller.height = isCrouching ? crouchHeight : startHeight;

        // Jump
        if (jumpTriggered && groundedPlayer)
        {
            // parabolic force
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Calculate movement direction 
        moveDirection = orientation.forward * input.y + orientation.right * input.x;
        
        // Add friction to the slide
        slideVelocity = Vector3.MoveTowards(slideVelocity, Vector3.zero, 50 * Time.deltaTime); 

        // Combine horizontal and vertical movement
        Vector3 finalMove = (moveDirection.normalized * currentSpeed  + slideVelocity) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);
    }

    private IEnumerator SmoothlyLerpPlayerSpeed()
    {
        // smoothly lerp player speed to goal speed
        float time = 0;
        float difference = Mathf.Abs(goalSpeed - currentSpeed);
        float startValue = currentSpeed;

        while (time < difference)
        {
            currentSpeed = Mathf.Lerp(startValue, goalSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }
        
        currentSpeed = goalSpeed;
    }



#region movement states

    public enum MovementState
    {
        sliding,
        crouching,
        sprinting,
        walking,
        air
    }
    
    private void StateSetter()
    {
        bool crouchHeldDown = InputManager.instance.CrouchIsPressed;
        bool sprintHeldDown = InputManager.instance.SprintIsPressed;
        
        // Crouching check
        isCrouching = crouchHeldDown;
        isSprinting = sprintHeldDown;

        if (slideVelocity.magnitude > 0.1f)
        {
            state = MovementState.sliding;
        }
        
        // Crouching
        else if (isCrouching)
        {
            state = MovementState.crouching;
            goalSpeed = crouchSpeed;
        }
        
        // Sprinting
        else if (isSprinting)
        {
            state = MovementState.sprinting;
            goalSpeed = sprintSpeed;
        }
        
        // Walking
        else if (groundedPlayer)
        {
            state = MovementState.walking;
            goalSpeed = walkSpeed;
        }

        // In air
        else
        {
            state = MovementState.air;
        }

        if (Mathf.Abs(goalSpeed - lastGoalSpeed) > 2f && currentSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpPlayerSpeed());
        }
        else
        {
            currentSpeed = goalSpeed;
        }

        lastGoalSpeed = goalSpeed;
    }
    
    #endregion
    
    void Slide()
    {
        bool slideTriggered = InputManager.instance.CrouchJustPressed;
        if (slideTriggered && groundedPlayer && isSprinting && currentSpeed > 6)
        {
            slideVelocity = (orientation.forward * slideSpeed);
        }
    }
}
