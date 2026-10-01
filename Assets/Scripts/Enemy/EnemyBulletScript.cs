using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBulletScript : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private RangeEnemyAttack _rangeEnemyAttack;

    [Header("Bullet Settings")]
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _bulletLifeTime = 5f;
    [SerializeField] private float _angularVelocity;
    private int _damage = 1;
    private bool _isActive = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

    }
    public void Shoot(Vector2 direction, int damage)
    {
        this._damage = damage;
        this._isActive = true;

        if (Mathf.Abs(direction.x + 1) < 0.01f)
            direction.y += 0.01f;
        
        transform.right = direction;
        _rb.linearVelocity = direction * _movementSpeed;
        _rb.angularVelocity = _angularVelocity;
        
        // Set auto-release timer
        LeanTween.delayedCall(gameObject, _bulletLifeTime, () => {
            if (_isActive && _rangeEnemyAttack != null)
            {
                _rangeEnemyAttack.ReleaseBullet(this);
            }
        });
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isActive) return;

        if (collider.TryGetComponent(out Player player))
        {
            _isActive = false;
            LeanTween.cancel(gameObject);

            player.TakeDamage(_damage);
            this._collider.enabled = false;

            _rangeEnemyAttack.ReleaseBullet(this);
        }
    }

    public void Configure(RangeEnemyAttack rangeEnemyAttack)
    {
        this._rangeEnemyAttack = rangeEnemyAttack;
    }

    public void Reload()
    {
        _isActive = false;
        _rb.linearVelocity = Vector2.zero; // Reset velocity
        _rb.angularVelocity = 0f; // Reset angular velocity
        _collider.enabled = true;
        
        // Cancel any pending LeanTween calls
        LeanTween.cancel(gameObject);
        
        // Reset position and rotation
        transform.rotation = Quaternion.identity;
    }

    public void PassAwayAfterWave()
    {
        // Cancel any pending LeanTween calls
        LeanTween.cancel(gameObject);
        
        // Mark as inactive to prevent further interactions
        _isActive = false;
        
        // Return to pool instead of destroying (for object pooling)
        if (_rangeEnemyAttack != null)
        {
            _rangeEnemyAttack.ReleaseBullet(this);
        }
        else
        {
            // Fallback: destroy if no pool reference
            Destroy(gameObject);
        }
    }
}
