using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(RangeEnemyAttack))]
public class BossEnemy : Enemy
{
    [Header("Health Bar")]
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Slider _healthBar;

    [Header("Armor")]
    [SerializeField] private float _armorReduction = 0.2f; // 20% damage reduction

    enum State { None, Idle, Moving, Attacking, Dead }

    private State _state;
    private float _timer;
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _maxIdleDuration = 1f;
    [SerializeField] private Animator _animator;

    [Header("Attacking")]
    private float _attackCounter;
    private float _idleDuration;
    private Vector3 _targetPosition;
    private RangeEnemyAttack _attack;


    void Awake()
    {

        _state = State.None;

        _healthBar.gameObject.SetActive(false);
        _onSpawnSequenceComplete += BossSpawnSequenceComplete;
        _onDamageTaken += DamageTakenCallback;
    }

    void OnDestroy()
    {
        _onSpawnSequenceComplete -= BossSpawnSequenceComplete;
        _onDamageTaken -= DamageTakenCallback;
    }

    protected override void Start()
    {
        base.Start();
        _attack = GetComponent<RangeEnemyAttack>();
    }

    void Update()
    {
        ManageStates();
    }

    private void ManageStates()
    {
        switch (_state)
        {
            case State.Idle:
                ManageIdleState();
                break;
            case State.Moving:
                ManageMovingState();
                break;
            case State.Attacking:
                ManageAttackingState();
                break;
            default:
                break;
        }
    }

    private void SetIdleState()
    {
        _state = State.Idle;
        _idleDuration = Random.Range(1f, _maxIdleDuration);

        _animator.Play("Idle");
    }

    private void ManageIdleState()
    {
        _timer += Time.deltaTime;

        if (_timer >= _idleDuration)
        {
            _timer = 0f;
            StartMovingState();
        }
    }

    private void StartMovingState()
    {
        _state = State.Moving;

        _targetPosition = GetRandomPosition();

        _animator.Play("Moving");
    }


    private void ManageAttackingState()
    {
        Attacking();
    }

    private void Attacking()
    {
        Vector2 direction = Quaternion.Euler(0, 0, -45 * _attackCounter) * Vector2.up;
        PlayAttackSound();
        _attack.InstantShoot(direction);
        _attackCounter++;
    }

    private void ManageMovingState()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            StartAttackingState();

    }

    private void StartAttackingState()
    {
        _state = State.Attacking;
        _attackCounter = 0;
        _animator.Play("Attack");
    }

    private void BossSpawnSequenceComplete()
    {
        _healthBar.gameObject.SetActive(true);
        UpdateHealthBar();

        SetIdleState();
    }


    private void UpdateHealthBar()
    {
        _healthBar.value = (float)_health / _maxHealth;
        _healthText.text = $"{_health}/{_maxHealth}";
    }

    private void DamageTakenCallback(int damage, Vector3 position, bool isCriticalHit)
    {
        // Armor reduces incoming damage
        int reducedDamage = Mathf.Max(1, Mathf.RoundToInt(damage * (1f - _armorReduction)));
        _health -= (reducedDamage - damage); // Apply the reduction
        UpdateHealthBar();
    }
    public override void PassAway()
    {
        _onBossPassAway?.Invoke(transform.position);
        PassAwayAfterWave();
    }

    private Vector3 GetRandomPosition()
    {
        Vector2 targetPosition = Vector2.zero;

        targetPosition.x = Random.Range(-14, 14);
        targetPosition.y = Random.Range(-6, 6);

        return targetPosition;
    }

}
