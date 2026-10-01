using UnityEngine;

[RequireComponent(typeof(EnemyMovement), typeof(RangeEnemyAttack))]
public class RangeEnemy : Enemy
{ 
    private RangeEnemyAttack _rangeEnemyAttack;
    
    //[Tab("Range Attack Settings")]
    [SerializeField] private float _minAttackDistance = 3f; // Minimum distance to maintain
    //[SerializeField] private float _maxAttackDistance = 8f; // Maximum attack range
    protected override void Start()
    {
        base.Start();
        _rangeEnemyAttack = GetComponent<RangeEnemyAttack>();

        _rangeEnemyAttack.StorePlayer(_player);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanAttack())
            return;

        ManageAttack();

        transform.localScale = _player.transform.position.x > transform.position.x ?  Vector3.one : new Vector3(-1, 1, 1);
    }

    private void ManageAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, _player.transform.position);

        if (distanceToPlayer > _detectionRadius)
        {
            _enemyMovement.FollowPlayer(); // Follow if too far
        }
        else if (distanceToPlayer < _minAttackDistance) // Add minimum attack distance
        {
            _enemyMovement.MoveAwayFromPlayer(); // Move away if too close
        }
        else
        {
            _enemyMovement.StopMovement(); // Stop and attack from optimal distance
            TryAttack();
        }
        
    }

    private void TryAttack()
    {
        _rangeEnemyAttack.AutoAttack();
    }
}
