using UnityEngine;
using System;

public abstract class Enemy : MonoBehaviour
{
    [Header("References")]
    protected EnemyMovement _enemyMovement;

    [SerializeField] protected int _maxHealth;
    protected int _health;
    [SerializeField] protected int _xpValue = 1;

    protected Player _player;
 
    [Header("Spawn Settings")]

    [SerializeField] protected MeshRenderer _enemyRenderer;
    [SerializeField] protected MeshRenderer _enemyWeaponPrimary;
    [SerializeField] protected MeshRenderer _enemyWeaponSecondary;
    [SerializeField] protected Animator _enemyAnimator;
    [SerializeField] protected bool _hasTwoWeapons = true;
    [SerializeField] protected GameObject _spawnIndicator;
    [SerializeField] protected float _spawnIndicatorScale = 1.5f; 
    [SerializeField] protected Collider2D _enemyCollider;
    protected bool _hasSpawned;

    [Header("Audio")]
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected AudioClip _takeDamageClip;
    [SerializeField] protected AudioClip _deathClip;
    [SerializeField] protected AudioClip _attackClip; 
    [SerializeField] protected float _volumeScale = 1f;

    [Header("Effects")]  
    [SerializeField] protected ParticleSystem _passAwayVFX;

    [SerializeField] protected float _detectionRadius;

    [Header("Debug")]
    [SerializeField] protected Color _debugColor;
    [SerializeField] protected bool _showDetectionRadius;

    [Header("Actions")]
    public static Action<int, Vector3, bool> _onDamageTaken;
    public static Action<Vector3> _onPassAway;
    public static Action<Vector3> _onBossPassAway;
    protected Action _onSpawnSequenceComplete;
    public static Action<int> _onEnemyKilledWithXP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        _health = _maxHealth;
        _enemyMovement = GetComponent<EnemyMovement>();
        
        // Use cached singleton instead of expensive FindFirstObjectByType
        _player = Player.instance;
    
            // Get or create AudioSource
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("Player not found");
            Destroy(gameObject);
        }

        StartSpawnSequence();
    }

    protected bool CanAttack()
    {
        if (!_hasSpawned) return false; // prevent attacks while spawning

        if (_hasTwoWeapons && _enemyWeaponSecondary != null) // Check for null
        {
            return _enemyRenderer.enabled && _enemyWeaponPrimary.enabled && _enemyWeaponSecondary.enabled;
        }
        else
        {
            return _enemyRenderer.enabled && _enemyWeaponPrimary.enabled;
        }
    }

    private void StartSpawnSequence()
    {
        _hasSpawned = false;

        // Hide visuals
        SetRenderersVisible(false);

        // Disable gameplay impact while spawning
        if (_enemyCollider != null) _enemyCollider.enabled = false;
        if (_enemyMovement != null) _enemyMovement.enabled = false;
        if (_enemyAnimator != null) _enemyAnimator.enabled = false;

        Vector3 targetScale = _spawnIndicator.transform.localScale * _spawnIndicatorScale;
        LeanTween.scale(_spawnIndicator.gameObject, targetScale, .3f)
            .setLoopPingPong(4)
            .setOnComplete(SpawnSequenceComplete);
    }

    private void SpawnSequenceComplete()
    {
        SetRenderersVisible(true);
        _hasSpawned = true;

        // Re-enable gameplay impact
        if (_enemyCollider != null) _enemyCollider.enabled = true;
        if (_enemyMovement != null) _enemyMovement.enabled = true;
        if (_enemyAnimator != null) _enemyAnimator.enabled = true;

        if (_enemyMovement != null)
            _enemyMovement.StorePlayer(_player);

         _onSpawnSequenceComplete?.Invoke();

    }

    private void SetRenderersVisible(bool visible)
    {
        _enemyRenderer.enabled = visible;
        _enemyWeaponPrimary.enabled = visible;

        if (_hasTwoWeapons && _enemyWeaponSecondary != null) // Check for null
        {
            _enemyWeaponSecondary.enabled = visible;
        }
    }

    public void TakeDamage(int damage, bool isCriticalHit)
    {
        Debug.Log($"Enemy taking damage: {damage}, Current health: {_health}, Max health: {_maxHealth}");
        int realDamage = Mathf.Min(damage, _health);
        _health -= realDamage;

        _onDamageTaken?.Invoke(realDamage, transform.position, isCriticalHit);
        Debug.Log($"After damage: Health = {_health}");

        if (_takeDamageClip != null && _audioSource != null)
            _audioSource.PlayOneShot(_takeDamageClip, _volumeScale);

        if (_health <= 0)
            PassAway();

    }

    public virtual void PassAway()
    {
        _onPassAway?.Invoke(transform.position);
        
        // Trigger enemy kill event for unlock progression
        GameManager.OnEnemyKilled?.Invoke(GetType().Name);
        
        // Trigger XP reward event
        _onEnemyKilledWithXP?.Invoke(_xpValue);
        
        PassAwayAfterWave();
    }
    public void PassAwayAfterWave()
    {
        
        _passAwayVFX.transform.SetParent(null);
        _passAwayVFX.Play();

        Destroy(gameObject);
    }

    protected void PlayAttackSound()
    {
        if (_attackClip != null && _audioSource != null)
            _audioSource.PlayOneShot(_attackClip, _volumeScale);
    }

    public virtual void ScaleStats(float healthMultiplier, float damageMultiplier)
    {
        _maxHealth = Mathf.RoundToInt(_maxHealth * healthMultiplier);
        
        // Only set health to max if enemy is alive (health > 0)
        if (_health > 0)
        {
            _health = _maxHealth;
        }
        
        // Derived classes can override this to scale damage and other stats
    }

    protected void OnDrawGizmos()
    {
        if (!_showDetectionRadius) return;
        Gizmos.color = _debugColor;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }

    public Vector3 GetCenter()
    {
        return (Vector2)transform.position +_enemyCollider.offset;
    }

}
