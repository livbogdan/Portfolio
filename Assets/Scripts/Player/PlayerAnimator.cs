using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private Rigidbody2D _rigidbody;
    private float _startTime;
    private float _lastAudioTime;
    private float _audioCooldown = 0.5f;

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _startTime = Time.time;
    }

    private void FixedUpdate()
    {
        // Only animate during gameplay
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameState != GameState.GAME)
            return;
            
        if (_rigidbody.linearVelocity.magnitude < 0.001f)
        {
            _animator.Play("Idle");
            if (Time.time - _startTime > 0.1f && Time.time - _lastAudioTime > _audioCooldown)
            {
                AudioManager._instance?.PlayPlayerSFX(AudioManager._instance.GetPlayerIdleClip());
                _lastAudioTime = Time.time;
            }
        }
        else
        {
            _animator.Play("Move");
            if (Time.time - _startTime > 0.1f && Time.time - _lastAudioTime > _audioCooldown)
            {
                AudioManager._instance?.PlayPlayerSFX(AudioManager._instance.GetPlayerMoveClip());
                _lastAudioTime = Time.time;
            }
        }
    }

}
