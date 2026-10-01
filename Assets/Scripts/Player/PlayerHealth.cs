using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHealth : MonoBehaviour, IPlayerStatsDependency, IGameStateListner
{
    [Header("Player Health")]
    [SerializeField] private float _baseMaxHealth;

    private float _armor;
    private float _maxHealth;
    private float _health;
    private float _lifeSteal;
    private float _dodgeChance;

    [Header("Recovery")]
    private float _healthRecoveryRate;
    private float _healthRecoveryValue;
    private float _healthRecoveryTimer;
    private float _healthRecoveryDuration;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _takeDamageClip;
    [SerializeField] private AudioClip _dodgeClip;

    [Header("UI")]
    [SerializeField] private Slider _healthBar;
    [SerializeField] private TextMeshProUGUI _healthText;

    public static Action onTakeDamage;
    public static Action<Vector2> onAttackDodged;
    private CharterDataSO _currentCharacter;

    private void Awake()
    {
        Enemy._onDamageTaken += EnemyTookDamage;
        CharacterSelectionManager._onCharacterSelected += OnCharacterSelected;
    }

    private void Start()
    {
        // Ensure we have valid max health
        if (_maxHealth <= 0)
            _maxHealth = _baseMaxHealth;
        
        _health = _maxHealth;
        UpdateUI();
    }

    private void Update()
    {
        // Only recover health during gameplay
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameState != GameState.GAME)
            return;
            
        if (_health < _maxHealth)
        {
            RecoverHealth();
        }
    }

    private void OnDestroy()
    {
        Enemy._onDamageTaken -= EnemyTookDamage;
    }

    private void EnemyTookDamage(int damage, Vector3 enemyPosition, bool isCritical)
    {
        if (_health >= _maxHealth)
            return;

        float lifeStealValue = damage * _lifeSteal;
        float healthToAdd = Mathf.Min(lifeStealValue, _maxHealth - _health);

        _health += healthToAdd;
        UpdateUI();
    }
    
    private void OnCharacterSelected(CharterDataSO character)
    {
        _currentCharacter = character;
    }

    public void TakeDamage(float damage)
    {
        if (ShouldDodge())
        {
            if (AudioManager._instance.IsSFXOn && _audioSource && _dodgeClip)
                _audioSource.PlayOneShot(_dodgeClip);
            onAttackDodged?.Invoke(transform.position);
            return;
        }

        float realDamage = damage * Mathf.Clamp(1 - (_armor / 1000), 0, 10000);
        realDamage = Mathf.Min(realDamage, _health);
        _health -= realDamage;

        AudioClip damageClip = _currentCharacter?.TakeDamageClip ?? _takeDamageClip;
        if (AudioManager._instance.IsSFXOn && _audioSource && damageClip)
            _audioSource.PlayOneShot(damageClip);
            
        onTakeDamage?.Invoke();
        // Trigger haptic feedback on damage
        if (HapticFeedbackManager.Instance != null)
            HapticFeedbackManager.Instance.TriggerDamageHaptic();

        UpdateUI();

        if (_health <= 0)
            PassAway();

    }

    private bool ShouldDodge()
    {
        if (_dodgeChance <= 0) return false;
        
        float dodge = UnityEngine.Random.Range(0f, 100f);
        return dodge < _dodgeChance;
    }

    private void PassAway()
    {
        GameManager.Instance.SetGameState(GameState.GAMEOVER);
    }

    private void UpdateUI()
    {
        if (_healthBar == null)
        {
            Debug.LogError("Health bar is not assigned in PlayerHealth!");
            return;
        }
        
        if (_maxHealth <= 0)
        {
            Debug.LogError("Max health is 0 or negative!");
            return;
        }
        
        float healthBarValue = (float)_health / _maxHealth;
        _healthBar.value = healthBarValue;
        
        Debug.Log($"Health: {_health}/{_maxHealth}, Bar Value: {healthBarValue}");

        if (_healthText != null)
            _healthText.text = (int)_health + " / " + (int)_maxHealth;
    }
    private void RecoverHealth()
    {
        _healthRecoveryTimer += Time.deltaTime;

        if (_healthRecoveryTimer >= _healthRecoveryDuration)
        {
            _healthRecoveryTimer = 0;

            float healthToAdd = Mathf.Min(0.1f, _maxHealth - _health);
            _health += healthToAdd;

            UpdateUI();
        }
    }

    public void ResetHealth()
    {
        _health = _maxHealth;
        UpdateUI();
        Debug.Log($"Player health reset to {_maxHealth}");
    }
    
    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.SELECTMODE)
        {
            ResetHealth();
        }
    }

    public void UpdateStats(PlayerStatsManager playerStatsManager)
    {
        float addedHealth = playerStatsManager.GetStatsValue(Stats.MaxHealth);
        _maxHealth = _baseMaxHealth + (int)addedHealth;
        _maxHealth = Mathf.Max(_maxHealth, 1);

        _health = _maxHealth;

        UpdateUI();

        _armor = playerStatsManager.GetStatsValue(Stats.Armor);
        _lifeSteal = playerStatsManager.GetStatsValue(Stats.LifeSteal) / 100;
        _dodgeChance = playerStatsManager.GetStatsValue(Stats.Dodge);

        _healthRecoveryRate = Mathf.Max(.0001f, playerStatsManager.GetStatsValue(Stats.HealthRecoverySpeed));
        _healthRecoveryDuration = 1f / _healthRecoveryRate;
    }
}
