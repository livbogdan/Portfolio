using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public abstract class DroppableCurrency : MonoBehaviour, ICollectable
{
    [SerializeField] private float _rotationSpeed = 180; // Degrees per second
    [SerializeField] private GameObject _droppableCurrency;
    [SerializeField] private AudioClip _collectionSFX;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _collectionSpeed = 1.5f;
    private bool _collected;
    private bool _returnedToPool;
    private Coroutine _moveCoroutine;

    private void OnEnable()
    {
        _collected = false;
        _returnedToPool = false;
        OnReturnedFromPool();
    }

    public void Collect(Player player)
    {
        if (_collected || _returnedToPool)
            return;

        _collected = true;

        if (_audioSource != null && _collectionSFX != null)
        {
            _audioSource.PlayOneShot(_collectionSFX);
        }

        _moveCoroutine = StartCoroutine(MoveToPlayer(player));
    }


    private void Update()
    {
        if (_droppableCurrency != null)
        {
            _droppableCurrency.transform.Rotate(Vector3.down, _rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator MoveToPlayer(Player player)
    {
        float timer = 0;
        Vector2 initialPosition = transform.position;

        while(timer<1)
        {
            Vector2 targetPosition = player.GetCenter();
            
            transform.position = Vector2.Lerp(initialPosition, targetPosition, timer);
            timer += Time.deltaTime*_collectionSpeed;
            yield return null;
        }

        _moveCoroutine = null;
        Collected();
    }

    #region Pooling
    public virtual void OnReturnedFromPool()
    {
        // Reset state when taken from pool
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    public virtual void OnReturnedToPool()
    {
        // Stop any active coroutines when returned to pool
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        // Reset visual state
        if (_droppableCurrency != null)
        {
            _droppableCurrency.transform.rotation = Quaternion.identity;
        }
    }

    protected bool TryMarkReturnedToPool()
    {
        if (_returnedToPool)
        {
            return false;
        }

        _returnedToPool = true;
        return true;
    }
    #endregion

    protected abstract void ReturnToPool();
    protected abstract void Collected();
}
