using System;
using UnityEngine;
using UnityEngine.Pool;

public class Chest : MonoBehaviour, ICollectable
{
    public static Action _onCollected;
    
    [Header("Audio")]
    [SerializeField] private AudioClip _chestOpenSound;
    [SerializeField]private AudioSource _audioSource;

    private static ObjectPool<Chest> _chestPool;

    private void Start()
    {
        // Initialize pool if not already done
        if (_chestPool == null)
        {
            _chestPool = new ObjectPool<Chest>(
                CreateChest,
                OnTakeChestFromPool,
                OnReturnChestToPool,
                OnDestroyChest,
                maxSize: 10
            );
        }
    }

    public void Collect(Player player)
    {
        PlayChestSound();
        _onCollected?.Invoke();
        ReturnToPool();
    }

    private void PlayChestSound()
    {
        // Play chest opening sound effect
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        _audioSource.clip = _chestOpenSound;
        _audioSource.volume = 0.5f;
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.Play();
    }

    #region Pooling
    private static Chest CreateChest()
    {
        // This would typically load from a prefab, but for dynamic creation:
        GameObject go = new GameObject("Chest");
        return go.AddComponent<Chest>();
    }

    private static void OnTakeChestFromPool(Chest chest)
    {
        chest.gameObject.SetActive(true);
    }

    private static void OnReturnChestToPool(Chest chest)
    {
        if (chest._audioSource != null)
        {
            chest._audioSource.Stop();
        }
        chest.gameObject.SetActive(false);
    }

    private static void OnDestroyChest(Chest chest)
    {
        Destroy(chest.gameObject);
    }

    public static Chest GetFromPool()
    {
        return _chestPool?.Get();
    }

    public void ReturnToPool()
    {
        if (_chestPool != null)
        {
            _chestPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
