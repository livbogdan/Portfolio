using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Pool;

public class DropManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Cash _cashPrefab;
    [SerializeField] private Gem _gemPrefab;
    [SerializeField] private Chest _chestPrefab;

    [Header("Setting")]
    
    [SerializeField, Range(0,100)] private float _gemDropChance;
    [SerializeField, Range(0,100)] private float _chestDropChance;
    private float _cashDropChance = 100f;
    private ObjectPool<Gem> _gemPool;

    private void Awake()
    {
        Enemy._onPassAway   += EnemyPassedAwayCallback;
        Enemy._onBossPassAway += BossEnemyPassedAwayCallback;

        Gem._onCollected    += ReleaseGem;
    }


    private void OnDestroy()
    {
        Enemy._onPassAway   -= EnemyPassedAwayCallback;
        Enemy._onBossPassAway -= BossEnemyPassedAwayCallback;

        Gem._onCollected    -= ReleaseGem;
    }

    void Start()
    {
        Cash.ConfigurePool(_cashPrefab, transform);

        _gemPool = new ObjectPool<Gem>(
            CreateGem, 
            GemActionOnGet, 
            GemActionOnRelease, 
            GemActionOnDestroy);

    }
    
    private Gem CreateGem()                         => Instantiate(_gemPrefab, transform);
    private void GemActionOnGet(Gem gem)            => gem.gameObject.SetActive(true);
    private void GemActionOnRelease(Gem gem)        => gem.gameObject.SetActive(false);
    private void GemActionOnDestroy(Gem gem)        => Destroy(gem.gameObject);


    private void EnemyPassedAwayCallback(Vector3 enemyPosition)
    {
        DroppableCurrency droppableObject;
        
        float randomValue = Random.Range(0f, 100f);
        if (randomValue <= _gemDropChance)
            droppableObject = _gemPool.Get();
        else if (randomValue <= _gemDropChance + _cashDropChance)
            droppableObject = Cash.GetFromPool();
        else
            droppableObject = null;
        
        Vector3 randomOffset = new Vector3(
            Random.Range(-2f, 2f), 
            Random.Range(-2f, 2f), 
            0
        );
        
        droppableObject.transform.position = enemyPosition + randomOffset;

        TryDropChest(enemyPosition);
    }
    private void BossEnemyPassedAwayCallback(Vector3 bossPosition)
    {
        Instantiate(_chestPrefab, bossPosition, Quaternion.identity, transform);
        DropChest(bossPosition);
    }

    private void TryDropChest(Vector3 enemyPosition)    
    {
        bool shouldDropChest = Random.Range(0, 101) <= _chestDropChance;

        if (!shouldDropChest)
            return;

        DropChest(enemyPosition);
    }

    private void DropChest(Vector3 enemyPosition)
    {
        Instantiate(_chestPrefab, enemyPosition, Quaternion.identity, transform);
    }
    private void ReleaseGem(Gem gem) => _gemPool.Release(gem);

}