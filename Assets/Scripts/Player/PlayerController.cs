using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public static PlayerController instance;
    
    [Header("References")] 
    public Transform orientation;
    public MovementState state;
    public LayerMask whatIsWall;
    
    // Velocity
    private Vector3 playerVelocity;
    private Vector3 moveDirection;
    private Vector3 slideVelocity;
    
    // Set Speeds
    private float currentSpeed;
    private float goalSpeed;
    private float lastGoalSpeed;
    
    // Player speeds
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 7.0f;
    public float crouchSpeed = 3.5f;
    public float slideSpeed = 3.0f;
    public float wallRunSpeed = 7.0f;
    
    // Heights
    public float crouchHeight = 0.5f;
    private float startHeight = 2.0f;
    public float jumpHeight = 1.5f;
    
    // Environment
    public float gravityValue = -9.81f;
    public float slideFriction = 10.0f;
    private bool groundedPlayer;
    
    // Timers
    public float coyoteSetTime = 0.2f;
    public float coyoteTimer;
    
    // Wall Run
    public float wallCheckDistance = .7f;
    private bool isWallRunning;
    private float forwardInput;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;
    
    // Other Actions
    private bool isSliding;
    private bool isCrouching;
    private bool isSprinting;
    

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
        WallCheck();
        Slide();
        StateSetter();
        MovePlayer();
        WallRun();
    }

    void MovePlayer()
    {

        // Read input
        Vector2 input = InputManager.instance.MoveInput;
        bool jumpTriggered = InputManager.instance.JumpJustPressed;
        forwardInput = input.y;
        
        // Grounded check
        groundedPlayer = controller.isGrounded;
        
        if (groundedPlayer || isWallRunning)
        {
            coyoteTimer = coyoteSetTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
        
        // Crouch
        controller.height = isCrouching ? crouchHeight : startHeight;
        
        // Gravity
        if (!isWallRunning)
        {
            // Apply gravity
            playerVelocity.y += gravityValue * Time.deltaTime;
        }
        
        // Jump
        if (jumpTriggered && (coyoteTimer > 0f))
        {
            Jump();
        }

        // Calculate movement direction 
        moveDirection = orientation.forward * input.y + orientation.right * input.x;
        slideVelocity = Vector3.MoveTowards(slideVelocity, Vector3.zero, slideFriction * Time.deltaTime);

        // Downwards Y velocity cap
        if (playerVelocity.y < gravityValue) { 
            playerVelocity.y = gravityValue;
        }
        // Combine horizontal and vertical movement
        Vector3 finalMove = (moveDirection.normalized * currentSpeed  + slideVelocity) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);
    }

    private void Jump()
    {
        // parabolic force
        playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
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
        wallrunning,
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

        // Wall Running
        if (isWallRunning)
        {
            state = MovementState.wallrunning;
            goalSpeed = wallRunSpeed;
        }
        
        // Sliding
        else if (isSliding)
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
            isSliding = true;
            slideVelocity = (orientation.forward * slideSpeed);
            slideVelocity.y = 0f;
        }
        else if (slideVelocity.magnitude < 0.1f) 
        {
            isSliding = false;
        }
    }
    
    #region wallrunning

    private void WallCheck()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }
    
    private void WallRun()
    {
        // if a wall is detected and we are moving forward: start wall run
        if ((wallLeft || wallRight) && forwardInput > 0 && !groundedPlayer)
        {
            Debug.Log("Walls detected");
            // Start wall run
            if (!isWallRunning)
            {
                WallRunStart();
            }
        }
        else
        {
            WallRunEnd();
        }
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        playerVelocity += wallForward * wallRunSpeed;
    }

    private void WallRunJump()
    {
    }
    
    private void WallRunStart()
    {
        isWallRunning = true;
        playerVelocity.y = 0;
    }
    
    private void WallRunEnd()
    {
        isWallRunning = false;
    }
    #endregion
}
