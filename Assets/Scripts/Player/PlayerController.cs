using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    private float playerSpeed = 5.0f;
    private float jumpHeight = 1.5f;
    private float gravityValue = -9.81f;
    
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Vector3 moveDirection;
    private bool groundedPlayer;

    [Header("References")] 
    public Transform orientation;
    
    public InputActionReference jumpAction; // expects Button

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
        MovePlayer();
    }

    void MovePlayer()
    {
        // Read input
        Vector2 input = InputManager.instance.MoveInput;
        bool jumpTriggered = InputManager.instance.JumpJustPressed;
        
        // Check if player is on the floor
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        
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
        
        // Combine horizontal and vertical movement
        Vector3 finalMove = (moveDirection.normalized * playerSpeed) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);
    }
}
