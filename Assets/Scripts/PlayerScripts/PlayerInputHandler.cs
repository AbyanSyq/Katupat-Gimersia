using Unity.VisualScripting;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
#endif

public class PlayerInputHandler : MonoBehaviour
{
	[Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private PlayerInputAction _inputActions;
    [SerializeField] private PlayerController3D playerController;
    private void Awake()
    {
        _inputActions = new PlayerInputAction();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Player.Look.performed += OnLook;
        _inputActions.Player.Look.canceled += OnLook;

        _inputActions.Player.Jump.started += OnJump;
        _inputActions.Player.Jump.canceled += OnJump;

        _inputActions.Player.Throw.started += OnThrowStarted;
        _inputActions.Player.Throw.canceled += OnThrowReleased;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;

        _inputActions.Player.Look.performed -= OnLook;
        _inputActions.Player.Look.canceled -= OnLook;

        _inputActions.Player.Jump.started -= OnJump;
        _inputActions.Player.Jump.canceled -= OnJump;

        _inputActions.Player.Throw.started += OnThrowStarted;
        _inputActions.Player.Throw.canceled += OnThrowReleased;

        _inputActions.Disable();

    }

    public void SetInput(bool enable)
    {
        if (enable)
        {
            _inputActions.Player.Enable();
        }
        else
        {
            _inputActions.Player.Disable();
        }
    }

#region Input Action Callbacks
	private void OnMove(InputAction.CallbackContext ctx)
	{
		MoveInput(ctx.ReadValue<Vector2>());
	}

    private void OnLook(InputAction.CallbackContext ctx)
    {
		LookInput(ctx.ReadValue<Vector2>());
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        JumpInput(ctx.ReadValueAsButton());
    }
    private void OnThrowStarted(InputAction.CallbackContext context)
    {
       playerController.StartCharging();
    }

    private void OnThrowReleased(InputAction.CallbackContext context)
    {
        playerController.ReleaseThrow();
    }

    private void OnRestartScene(InputAction.CallbackContext context)
    {
        RestartScene();
    }
#endregion

	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	} 

	public void LookInput(Vector2 newLookDirection)
	{
		if(cursorInputForLook)
			look = newLookDirection;
	}

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }
    public void RestartScene()
    {
        SceneTransitionManager.Instance.LoadScene(SceneManager.GetActiveScene().name, TransitionEffect.Slide);
    }
}

