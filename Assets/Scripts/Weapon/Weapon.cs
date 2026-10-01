using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public abstract class Weapon : MonoBehaviour, IPlayerStatsDependency
{
    [Header("Weapon Data")]
    [field: SerializeField] public WeaponDataSO WeaponData { get; private set; }

    [Header("Weapon Stats")]
    [SerializeField] [Tooltip("The base damage dealt by the weapon.")] protected int _damage;
    [SerializeField] [Tooltip("The range of the weapon's attack.")] protected float _range;
    [SerializeField] [Tooltip("The chance for a critical hit (0-100).")] protected float _criticalChance;
    [SerializeField] [Tooltip("The damage multiplier for a critical hit.")] protected float _criticalPercent;
    [SerializeField] [Tooltip("The speed at which the weapon aims.")] protected float _aimLerp;
    [SerializeField] [Tooltip("The delay between attacks.")] protected float _attackDelay;

    [Header("Level")]
    [field: SerializeField] public int Level { get; private set; }

    protected AudioSource _audioSource;

    [Space(5)]
    protected float _attackTimer;
    [SerializeField] protected LayerMask _enemyLayerMask;
    [SerializeField] protected Animator _animator;
    [Space(5)]
    [Header("Debug")]
    [SerializeField] protected bool _showDetectionRadius;
    [SerializeField] protected Color _gizmosColor;

    protected void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.clip = WeaponData.AttackSound;

        if (_animator != null && WeaponData.AnimatorOverrideController != null)
        {
            _animator.runtimeAnimatorController = WeaponData.AnimatorOverrideController;
        }
    }

    protected void PlayAttackSound()
    {
        if(!AudioManager._instance.IsSFXOn)
            return;
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioSource.volume = 0.5f;
        _audioSource.Play();
    }

    protected Enemy GetClosestEnemy()
    {
        Enemy closestEnemy = null;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, _range, _enemyLayerMask);

        if (enemies.Length <= 0)
            return null;

        float closestEnemyDistance = _range;

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy currentEnemy = enemies[i].GetComponent<Enemy>();

            float distance = Vector2.Distance(transform.position, currentEnemy.transform.position);

            if (distance < closestEnemyDistance)
            {
                closestEnemy = currentEnemy;
                closestEnemyDistance = distance;
            }
        }

        return closestEnemy;

    }

    protected int GetDamage(out bool isCriticalHit)
    {
        isCriticalHit = false;

        if (Random.Range(0, 101) <= _criticalChance)
        {
            isCriticalHit = true;
            return Mathf.RoundToInt(_damage * _criticalPercent);
        }

        return _damage;
    }

    private void OnDrawGizmos()
    {
        if (!_showDetectionRadius)
            return;
        Gizmos.color = _gizmosColor;
        Gizmos.DrawWireSphere(transform.position, _range);

        if (_aimLerp > 0)
            return;
        Gizmos.color = _gizmosColor;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    public abstract void UpdateStats(PlayerStatsManager playerStatsManager);

    protected void ConfigureStats()
    {
        Dictionary<Stats, float> calculatedStats = WeaponStatsCalculator.GetStats(WeaponData, Level);

        _damage             = Mathf.RoundToInt(calculatedStats[Stats.Attack]);
        _attackDelay        = 1f/calculatedStats[Stats.AttackSpeed];
        _range              = Mathf.RoundToInt(calculatedStats[Stats.Range]);
        _criticalChance     = calculatedStats[Stats.CriticalChance];        
        _criticalPercent    = calculatedStats[Stats.CriticalPercent];

    }

    public void UpgradeWeaponTo(int weaponLevel)
    {
        Level = weaponLevel;
        ConfigureStats();
    }
    public int GetRecyclePrice()
    {
        return WeaponStatsCalculator.GetRecyclePrice(WeaponData, Level);
    }

    public void Upgrade()
    {
        Level++;
        ConfigureStats();
    }
}
