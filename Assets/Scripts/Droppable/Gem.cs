using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Gem : DroppableCurrency
{
    public static Action<Gem> _onCollected;
    private static ObjectPool<Gem> _gemPool;

    private void Start()
    {
        // Initialize pool if not already done
        if (_gemPool == null)
        {
            _gemPool = new ObjectPool<Gem>(
                CreateGem,
                OnTakeGemFromPool,
                OnReturnGemToPool,
                OnDestroyGem,
                maxSize: 20
            );
        }
    }

    protected override void Collected()
    {
        _onCollected?.Invoke(this);
        ReturnToPool();
    }

    #region Pooling
    private static Gem CreateGem()
    {
        // This would typically load from a prefab, but for dynamic creation:
        GameObject go = new GameObject("Gem");
        return go.AddComponent<Gem>();
    }

    private static void OnTakeGemFromPool(Gem gem)
    {
        gem.OnReturnedFromPool();
        gem.gameObject.SetActive(true);
    }

    private static void OnReturnGemToPool(Gem gem)
    {
        gem.OnReturnedToPool();
        gem.gameObject.SetActive(false);
    }

    private static void OnDestroyGem(Gem gem)
    {
        Destroy(gem.gameObject);
    }

    public static Gem GetFromPool()
    {
        return _gemPool?.Get();
    }

    protected override void ReturnToPool()
    {
        if (!this || !TryMarkReturnedToPool())
        {
            return;
        }

        if (_gemPool != null)
        {
            _gemPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
