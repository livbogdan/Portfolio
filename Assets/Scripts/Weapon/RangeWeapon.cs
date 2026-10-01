using System;
using UnityEngine;
using UnityEngine.Pool;

public class RangeWeapon : Weapon
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _shootingPoint;

    [Header("Pooling")]
    private ObjectPool<Bullet> _bulletPool;
    void Start()
    {
        _bulletPool = new ObjectPool<Bullet>(CreateBullet, OnTakeFromPool, OnReturnToPool, OnDestroyBullet);
    }

    void Update()
    {
        // Only run weapon logic during gameplay
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameState != GameState.GAME)
            return;
            
        AutoAim();
    }

#region Pooling
    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(_bulletPrefab, _shootingPoint.position, Quaternion.identity);
        bullet.Configure(this);
        return bullet;
    }
    private void OnTakeFromPool(Bullet bullet)
    {
        bullet.Reload();
        bullet.transform.position = _shootingPoint.position;
        bullet.gameObject.SetActive(true);
    }

    private void OnReturnToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    public void ReturnToPool(Bullet bullet)
    {
        if (_bulletPool != null)
        {
            _bulletPool.Release(bullet);
        }
        else
        {
            Destroy(bullet.gameObject);
        }
    }
    private void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
    
#endregion
    private void AutoAim()
    {
        Enemy closestEnemy = GetClosestEnemy();

        Vector2 targetUpVector = Vector3.up;

        if (closestEnemy != null)
        {
            targetUpVector = (closestEnemy.GetCenter() - transform.position).normalized;
            transform.up = targetUpVector;
            
            ManageShooting();
            return;
        }
        
        transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * _aimLerp);


    }  
    private void ManageShooting()
    {
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackDelay)
        {
            _attackTimer = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        int damage = GetDamage(out bool isCriticalHit);

        Bullet bullet = _bulletPool.Get();
        bullet.Shoot(damage, transform.up, isCriticalHit);

        PlayAttackSound();
    }

    public override void UpdateStats(PlayerStatsManager playerStatsManager)
    {
        ConfigureStats();
        _damage = Mathf.RoundToInt(_damage * (1 +  playerStatsManager.GetStatsValue(Stats.Attack)/100));
        _attackDelay /= 1 + playerStatsManager.GetStatsValue(Stats.AttackSpeed) / 100;

        _criticalChance = Mathf.RoundToInt(_criticalChance * (1 + playerStatsManager.GetStatsValue(Stats.CriticalChance) / 100));
        _criticalPercent += playerStatsManager.GetStatsValue(Stats.CriticalPercent);

        _range += playerStatsManager.GetStatsValue(Stats.Range) / 15;
    }

}
