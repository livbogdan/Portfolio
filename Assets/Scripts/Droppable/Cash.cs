using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Cash : DroppableCurrency
{
    public static Action<Cash> _onCollected;
    private static ObjectPool<Cash> _cashPool;
    private static Cash _cashPrefab;
    private static Transform _poolParent;
    [SerializeField] private float _autoReturnDelay = 10f;
    private Coroutine _autoReturnCoroutine;

    public static void ConfigurePool(Cash prefab, Transform poolParent)
    {
        _cashPrefab = prefab;
        _poolParent = poolParent;

        if (_cashPool == null)
        {
            _cashPool = new ObjectPool<Cash>(
                CreateCash,
                OnTakeCashFromPool,
                OnReturnCashToPool,
                OnDestroyCash,
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 10
            );
        }
    }

    protected override void Collected()
    {
        _onCollected?.Invoke(this);
        
        ReturnToPool();
    }

    public override void OnReturnedFromPool()
    {
        base.OnReturnedFromPool();

        if (_autoReturnCoroutine != null)
        {
            StopCoroutine(_autoReturnCoroutine);
        }

        _autoReturnCoroutine = StartCoroutine(AutoReturnToPool());
    }

    public override void OnReturnedToPool()
    {
        base.OnReturnedToPool();

        if (_autoReturnCoroutine != null)
        {
            StopCoroutine(_autoReturnCoroutine);
            _autoReturnCoroutine = null;
        }
    }

    private IEnumerator AutoReturnToPool()
    {
        yield return new WaitForSeconds(_autoReturnDelay);
        _autoReturnCoroutine = null;
        ReturnToPool();
    }

    private static Cash CreateCash()
    {
        if (_cashPrefab != null)
        {
            return Instantiate(_cashPrefab, _poolParent);
        }

        GameObject go = new GameObject("Cash");
        if (_poolParent != null)
        {
            go.transform.SetParent(_poolParent);
        }

        return go.AddComponent<Cash>();
    }

    private static void OnTakeCashFromPool(Cash cash)
    {
        cash.gameObject.SetActive(true);
        cash.OnReturnedFromPool();
    }

    private static void OnReturnCashToPool(Cash cash)
    {
        cash.OnReturnedToPool();
        cash.gameObject.SetActive(false);
    }

    private static void OnDestroyCash(Cash cash)
    {
        Destroy(cash.gameObject);
    }

    public static Cash GetFromPool()
    {
        return _cashPool?.Get();
    }

    protected override void ReturnToPool()
    {
        if (!this || !TryMarkReturnedToPool())
        {
            return;
        }

        if (_cashPool != null)
        {
            _cashPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
