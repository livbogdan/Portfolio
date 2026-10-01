using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D _rb;
    private Collider2D _collider;

    [Header("Bullet Settings")]
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _bulletLifeTime = 5;
    [SerializeField] private LayerMask _enemyLayer;
    private int _damage;
    private RangeWeapon _rangeWeapon;
    private bool _isCriticalHit;
    private Enemy _target;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        //LeanTween.delayedCall(gameObject, 5, () => _rangeEnemyAttack.ReturnToPool(this));
    }

    public void Shoot(int damage, Vector3 direction, bool isCriticalHit)
    {
        //Invoke("Release", 1);

        this._damage = damage;
        this._isCriticalHit = isCriticalHit;

        transform.right = direction;
        _rb.linearVelocity = direction * _movementSpeed;
    }
    public void Reload()
    {
        _target = null;
        
        _rb.linearVelocity = Vector2.zero;
        _collider.enabled = true;

        CancelInvoke();
        Invoke("Release", _bulletLifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_target != null)
        {
           return;
        }

        if (IsInLayerMask(collider.gameObject.layer, _enemyLayer))
        {
            _target = collider.GetComponent<Enemy>();

            _collider.enabled = false;

            CancelInvoke();

            Attack(_target);
            Release();
        }
    }

    private bool IsInLayerMask(int layer, LayerMask enemyLayer)
    {
        return (enemyLayer.value & (1 << layer)) != 0;
    }

    private void Attack(Enemy enemy)
    {
        enemy.TakeDamage(_damage, _isCriticalHit);
    }

    private void Release()
    {
        if (!gameObject.activeSelf)
            return;
        
        if (_rangeWeapon != null)
        {
            _rangeWeapon.ReturnToPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Configure(RangeWeapon rangeWeapon)
    {
        this._rangeWeapon = rangeWeapon;
    }

}
