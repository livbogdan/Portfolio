using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(AudioSource))]
public class MeleeEnemy : Enemy
{

    [Header("Attack Settings")]
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _attackRate = 1f;

    private float _attackDelay;
    private float _attackTimer;

    protected override void Start()
    {
        base.Start();

        _attackDelay = 1f / _attackRate;
    }

    // Update is called once per frame
    void Update()
    {       
        if (!CanAttack())
            return;
        
        ManagePositionAndAttack();
    }

    private void ManagePositionAndAttack()
    {
        // Use sqrMagnitude for cheaper distance comparison (avoids sqrt)
        float sqrDistance = (transform.position - _player.transform.position).sqrMagnitude;
        float sqrDetectionRadius = _detectionRadius * _detectionRadius;

        // Manage movement based on distance
        if (sqrDistance > sqrDetectionRadius)
        {
            // Too far - move closer
            _enemyMovement.FollowPlayer();
        }
        else if (sqrDistance < sqrDetectionRadius * 0.8f) // Small tolerance to prevent jittering
        {
            // Too close - move away
            _enemyMovement.MoveAwayFromPlayer();
        }
        else
        {
            // At optimal distance - stop and attack
            _enemyMovement.StopMovement();
        }
        
        // Handle attacking - reuse cached distance
        if (_attackTimer >= _attackDelay && sqrDistance <= sqrDetectionRadius)
        {
            Attack();
        }
        else
        {
            Wait();
        }
    }

    private void TryAttack()
    {
        // Use sqrMagnitude for cheaper distance comparison
        float sqrDistance = (transform.position - _player.transform.position).sqrMagnitude;

        // Attack if within preferred distance + tolerance
        if (sqrDistance <= _detectionRadius * _detectionRadius)
        {
            Attack();
        }
    }

    private void Wait()
    {
        _attackTimer += Time.deltaTime;
    }

    private void Attack()
    {
        _attackTimer = 0;
        PlayAttackSound();
        _player.TakeDamage(_damage);
    }
}
