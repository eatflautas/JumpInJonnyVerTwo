using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public static PlayerInput PlayerInput;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    
    public bool AttackJustPressed { get; private set; }
    public bool InteractJustPressed { get; private set; }
    public bool CrouchJustPressed { get; private set; }
    public bool CrouchIsPressed { get; private set; }
    public bool CrouchWasReleased { get; private set; }
    public bool JumpJustPressed { get; private set; }
    public bool SprintJustPressed { get; private set; }
    public bool SprintIsPressed { get; private set; }
    public bool SprintWasReleased { get; private set; }
    public bool PauseJustPressed { get; private set; }

    private InputAction _moveInputAction;
    private InputAction _lookInputAction;
    private InputAction _attackInputAction;
    private InputAction _interactInputAction;
    private InputAction _crouchInputAction;
    private InputAction _jumpInputAction;
    private InputAction _sprintInputAction;
    private InputAction _pauseInputAction;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        PlayerInput = GetComponent<PlayerInput>();

        _moveInputAction = PlayerInput.actions["Move"]; 
        _lookInputAction = PlayerInput.actions["Look"];
        _attackInputAction = PlayerInput.actions["Attack"];
        _interactInputAction = PlayerInput.actions["Interact"];
        _crouchInputAction = PlayerInput.actions["Crouch"];
        _jumpInputAction = PlayerInput.actions["Jump"];
        _sprintInputAction = PlayerInput.actions["Sprint"];
        _pauseInputAction = PlayerInput.actions["Pause"];
    }

    private void Update()
    {
        MoveInput = _moveInputAction.ReadValue<Vector2>();
        LookInput = _lookInputAction.ReadValue<Vector2>();
        AttackJustPressed = _attackInputAction.WasPressedThisFrame();
        InteractJustPressed = _interactInputAction.WasPressedThisFrame();
        CrouchJustPressed = _crouchInputAction.WasPressedThisFrame();
        CrouchIsPressed = _crouchInputAction.IsPressed();
        CrouchWasReleased = _crouchInputAction.WasReleasedThisFrame();
        JumpJustPressed = _jumpInputAction.WasPressedThisFrame();
        SprintJustPressed = _sprintInputAction.WasPressedThisFrame();
        SprintIsPressed = _sprintInputAction.IsPressed();
        SprintWasReleased = _sprintInputAction.WasReleasedThisFrame();
        PauseJustPressed = _pauseInputAction.WasPressedThisFrame();
    }
}
