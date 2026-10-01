using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class InputManager : MonoBehaviour
{

    [Header("XR Input Settings")]
    [SerializeField] private InputActionAsset _inputActionAsset;

    private InputAction _moveAction;
    private InputAction _pauseAction;
    private InputAction _leftThumbstickAction;
    private InputAction _rightThumbstickAction;
    [SerializeField] private bool _isXRActive;

    public static InputManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        // Check if XR is active
        _isXRActive = XRSettings.isDeviceActive;
        
        // Setup move action from input asset for XR
        _moveAction = _inputActionAsset.FindActionMap("Player").FindAction("Move");
        _moveAction.Enable();

        // Setup pause action
        _pauseAction = _inputActionAsset.FindActionMap("Player").FindAction("Pause");
        _pauseAction.Enable();
        _pauseAction.performed += OnPausePerformed;

        // Setup individual thumbstick actions for XR
        if (_isXRActive)
        {
            // Create individual actions for left and right thumbsticks
            _leftThumbstickAction = new InputAction(binding: "<XRController>{LeftHand}/{Primary2DAxis}");
            _rightThumbstickAction = new InputAction(binding: "<XRController>{RightHand}/{Primary2DAxis}");
            
            _leftThumbstickAction.Enable();
            _rightThumbstickAction.Enable();
        }
    }

    private void OnDestroy()
    {
        if (_pauseAction != null)
        {
            _pauseAction.performed -= OnPausePerformed;
            _pauseAction.Disable();
        }

        if (_isXRActive)
        {
            _leftThumbstickAction?.Disable();
            _rightThumbstickAction?.Disable();
        }
    }

    public Vector2 GetMoveVector()
    {
        // For XR, check both thumbsticks and use whichever has input
        if (_isXRActive && _leftThumbstickAction != null && _rightThumbstickAction != null)
        {
            Vector2 leftInput = _leftThumbstickAction.ReadValue<Vector2>();
            Vector2 rightInput = _rightThumbstickAction.ReadValue<Vector2>();

            // Use right thumbstick if it has more input magnitude, otherwise use left
            if (rightInput.sqrMagnitude > leftInput.sqrMagnitude)
                return rightInput;
            else
                return leftInput;
        }

        // Fallback to regular move action (for non-XR or if actions aren't set up)
        return _moveAction.ReadValue<Vector2>();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        // Only handle pause if we're in gameplay
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameState == GameState.GAME)
        {
            GameManager.Instance.PauseButtonCallback();
        }
    }
}