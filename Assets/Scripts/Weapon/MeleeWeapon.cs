using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MeleeWeapon : Weapon
{
    enum State
    {
        Idle,
        Attack
    }

    private State _state;

    [Header("Weapon Settings")]
    [SerializeField] private Transform _transformHitPoint;
    [SerializeField] private BoxCollider2D _hitCollider;


    private List<Enemy> _damagedEnemies = new List<Enemy>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        // Only run weapon logic during gameplay
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameState != GameState.GAME)
            return;
            
        switch (_state)
        {
            case State.Idle:
                AutoAim();
                break;
            case State.Attack:
                Attacking();
                break;
        }
    }

    private void Attacking()
    {
        Attack();
    }

    [Button]
    private void StartAttack()
    {
        _animator.Play("Attack");
        _state = State.Attack;

        _damagedEnemies.Clear();

        _animator.speed = 1f / _attackDelay;

        PlayAttackSound();
    }

    private void AutoAim()
    {
        Enemy closestEnemy = GetClosestEnemy();

        Vector2 targetUpVector = Vector3.up;

        if (closestEnemy != null)
        {
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
            transform.up = targetUpVector;
            ManageAttack();
        }
        transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * _aimLerp);

        IncrementAttackTimer();

    }

    private void ManageAttack()
    {

        if (_attackTimer >= _attackDelay)
        {
            _attackTimer = 0;
            StartAttack();
        }
    }

    private void IncrementAttackTimer()
    {
        _attackTimer += Time.deltaTime;
    }

    private void StopAttack()
    {
        _state = State.Idle;

        _damagedEnemies.Clear();

    }

    private void Attack()
    {
        //Collider2D[] enemies = Physics2D.OverlapCircleAll(_transformHitPoint.position, _hitDetectionRadius, _enemyLayerMask);
        Collider2D[] enemies = Physics2D.OverlapBoxAll
        (
            _transformHitPoint.position,
            _hitCollider.bounds.size,
            _transformHitPoint.localEulerAngles.z,
            _enemyLayerMask
        );
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i].GetComponent<Enemy>();
            //1. If the enemy inside of the list
            if (!_damagedEnemies.Contains(enemy))
            {
                int damage = GetDamage(out bool isCriticalHit);

                enemy.TakeDamage(damage, isCriticalHit);
                _damagedEnemies.Add(enemy);
            }

        }
    }

    public override void UpdateStats(PlayerStatsManager playerStatsManager)
    {
        ConfigureStats();
        _damage = Mathf.RoundToInt(_damage * (1 +  playerStatsManager.GetStatsValue(Stats.Attack)/100));
        _attackDelay /= 1 + playerStatsManager.GetStatsValue(Stats.AttackSpeed) / 100;

        _criticalChance = Mathf.RoundToInt(_criticalChance * (1 + playerStatsManager.GetStatsValue(Stats.CriticalChance) / 100));
        _criticalPercent += playerStatsManager.GetStatsValue(Stats.CriticalPercent);
    }
}
