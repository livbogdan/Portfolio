using System;
using UnityEngine.Pool;
using UnityEngine;

public class RangeEnemyAttack : MonoBehaviour
{   
    
    private Player _player;

    
    [Header("Attack Settings")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private EnemyBulletScript _bulletPrefab;
    [SerializeField] private int _damage;
    [SerializeField] private float _attackRate;

    private float _attackDelay;
    private float _attackTimer;

    [Header("Bullet Pooling")]
    private ObjectPool<EnemyBulletScript> _bulletPool;

    void Start()
    {
        _attackDelay = 1f/ _attackRate;
        _attackTimer = _attackDelay;

        _bulletPool = new ObjectPool<EnemyBulletScript>(CreateBullet, ActionOnGet, ActionOnRelease, ActionOnDestroy);
    }
    
#region Bullet Pooling

    private EnemyBulletScript CreateBullet()
    {
        EnemyBulletScript bullet = Instantiate(_bulletPrefab, _attackPoint.position, Quaternion.identity);
        bullet.Configure(this);
        return bullet;
    }
    private void ActionOnGet(EnemyBulletScript bullet)
    {
        bullet.Reload(); // This should reset everything
        bullet.transform.position = _attackPoint.position;
        bullet.transform.rotation = Quaternion.identity; // Reset rotation
        bullet.gameObject.SetActive(true);
    }

    private void ActionOnRelease(EnemyBulletScript bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.Reload();
    }

    private void ActionOnDestroy(EnemyBulletScript bullet)
    {
        Destroy(bullet.gameObject);
    }

    public void ReleaseBullet(EnemyBulletScript bullet)
    {
        _bulletPool.Release(bullet);
    }

#endregion

    public void StorePlayer(Player player)
    {
        _player = player;
    }
    public void AutoAttack()
    {
        ManageShooting();
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

    Vector2 _gizmoDirection;
    private void Shoot()
    {
        Vector3 targetPosition = _player.transform.position;
        Vector3 direction = (targetPosition - _attackPoint.position).normalized;

        InstantShoot(direction);

    }

    public void InstantShoot(Vector2 direction)
    {
        EnemyBulletScript bulletInstance = _bulletPool.Get();

        bulletInstance.Shoot(direction, _damage);

        bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = direction * 5f;
        _gizmoDirection = direction;
    }

     private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_attackPoint.position, (Vector2)_attackPoint.position + _gizmoDirection * 3);
    }

}
