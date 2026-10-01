using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IPlayerStatsDependency
{
    private Rigidbody2D _rigidbody;

    [Header("Settings")]
    [SerializeField] private float _baseMoveSpeed;
    private float _moveSpeed;
    
    // External input (from joystick or other sources)
    private Vector2 _externalMoveInput = Vector2.zero;
    private bool _useExternalInput = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = Vector2.right;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameState != GameState.GAME)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        // Use joystick input if available, otherwise use InputManager
        Vector2 moveVector = _useExternalInput ? _externalMoveInput : InputManager.instance.GetMoveVector();
        _rigidbody.linearVelocity = moveVector * _moveSpeed;
    }
    
    /// <summary>
    /// Set movement input from external sources like joystick or VR controllers
    /// </summary>
    public void SetExternalMoveInput(Vector2 moveInput)
    {
        _externalMoveInput = moveInput;
        _useExternalInput = moveInput != Vector2.zero;
    }

    public void UpdateStats(PlayerStatsManager statsManager)
    {
        float moveSpeedPercent = statsManager.GetStatsValue(Stats.MoveSpeed) / 100;
        _moveSpeed = _baseMoveSpeed * (1 + moveSpeedPercent );
    }
}
