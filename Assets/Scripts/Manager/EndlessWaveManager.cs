using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WaveManagerUI))]
public class EndlessWaveManager : MonoBehaviour, IGameStateListner
{
    public static EndlessWaveManager instance;
    
    [Header("References")]
    [SerializeField] private Player _player;
    [SerializeField] private GameObject[] _normalEnemyPrefabs;
    [SerializeField] private GameObject[] _bossPrefabs;
    
    [Header("Settings")]
    [SerializeField] private float _baseWaveDuration = 30f;
    [SerializeField] private float _bossWaveDuration = 60f;
    [SerializeField] private float _minOffset = 5f;
    [SerializeField] private float _maxOffset = 15f;
    [SerializeField] private int _bossInterval = 5; // Boss every 5 levels
    
    [Header("Difficulty Scaling")]
    [SerializeField] private float _difficultyScalePerWave = 0.1f;
    [SerializeField] private float _spawnRateIncreasePerWave = 0.05f;
    [SerializeField] private float _healthScalePerWave = 0.15f;
    [SerializeField] private float _damageScalePerWave = 0.1f;
    
    private float _waveTime;
    private bool _isTimerOn;
    private int _currentWaveIndex = 1;
    private bool _isBossWave;
    private bool _bossSpawned;
    
    private WaveManagerUI _ui;
    private float _currentSpawnRate;
    private float _lastSpawnTime;
    
    // Base values for scaling
    private float _baseSpawnRate = 2f; // Enemies per second
    private float _currentWaveDuration;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
            
        _ui = GetComponent<WaveManagerUI>();
    }
    
    void Start()
    {
        _currentSpawnRate = _baseSpawnRate;
    }
    
    void Update()
    {
        if (!_isTimerOn)
            return;
            
        _waveTime += Time.deltaTime;
        
        // Update UI timer
        string timerString = ((int)(_currentWaveDuration - _waveTime)).ToString();
        _ui.UpdateTimerText(timerString, _isBossWave);
        
        // Handle wave progression
        if (_waveTime >= _currentWaveDuration)
        {
            if (_isBossWave && !_bossSpawned)
            {
                // Wait for boss to be killed before progressing
                if (transform.childCount == 0)
                {
                    // Trigger boss kill event for unlock progression
                    GameManager.OnBossKilled?.Invoke();
                    StartWaveTransition();
                }
            }
            else if (!_isBossWave)
            {
                StartWaveTransition();
            }
        }
        else
        {
            // Spawn enemies during wave
            if (_isBossWave)
            {
                HandleBossWave();
            }
            else
            {
                HandleNormalWave();
            }
        }
    }
    
    private void HandleNormalWave()
    {
        // Spawn normal enemies based on current spawn rate
        if (Time.time - _lastSpawnTime >= (1f / _currentSpawnRate))
        {
            SpawnRandomNormalEnemy();
            _lastSpawnTime = Time.time;
        }
    }
    
    private void HandleBossWave()
    {
        // Spawn boss once at the beginning
        if (!_bossSpawned)
        {
            SpawnBoss();
            _bossSpawned = true;
        }
        
        // Spawn some minions during boss fight
        if (Time.time - _lastSpawnTime >= (2f / _currentSpawnRate)) // Slower spawn rate for minions
        {
            SpawnRandomNormalEnemy();
            _lastSpawnTime = Time.time;
        }
    }
    
    private void SpawnRandomNormalEnemy()
    {
        if (_normalEnemyPrefabs.Length == 0) return;
        
        GameObject enemyPrefab = _normalEnemyPrefabs[Random.Range(0, _normalEnemyPrefabs.Length)];
        GameObject enemy = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity, transform);
        
        // Scale enemy stats based on current wave
        ScaleEnemyStats(enemy);
    }
    
    private void SpawnBoss()
    {
        if (_bossPrefabs.Length == 0) return;
        
        GameObject bossPrefab = _bossPrefabs[Random.Range(0, _bossPrefabs.Length)];
        GameObject boss = Instantiate(bossPrefab, GetSpawnPosition(), Quaternion.identity, transform);
        
        // Scale boss stats more aggressively
        ScaleBossStats(boss);
    }
    
    private void ScaleEnemyStats(GameObject enemy)
    {
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent == null) return;
        
        float healthMultiplier = 1f + (_healthScalePerWave * (_currentWaveIndex - 1));
        float damageMultiplier = 1f + (_damageScalePerWave * (_currentWaveIndex - 1));
        
        // Apply scaling (this would need to be implemented in Enemy class)
        enemyComponent.ScaleStats(healthMultiplier, damageMultiplier);
    }
    
    private void ScaleBossStats(GameObject boss)
    {
        Enemy bossComponent = boss.GetComponent<Enemy>();
        if (bossComponent == null) return;
        
        // Boss scaling is more aggressive
        float healthMultiplier = 1f + (_healthScalePerWave * (_currentWaveIndex - 1) * 2f);
        float damageMultiplier = 1f + (_damageScalePerWave * (_currentWaveIndex - 1) * 1.5f);
        
        bossComponent.ScaleStats(healthMultiplier, damageMultiplier);
    }
    
    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = Random.onUnitSphere;
        Vector2 offset = direction.normalized * Random.Range(_minOffset, _maxOffset);
        Vector2 targetPosition = (Vector2)_player.transform.position + offset;
        
        if (!GameManager.Instance.UseInfiniteMap)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, -16, 19);
            targetPosition.y = Mathf.Clamp(targetPosition.y, -9, 9);
        }
        
        return targetPosition;
    }
    
    private void StartWaveTransition()
    {
        _isTimerOn = false;
        DefeatAllEnemies();
        _currentWaveIndex++;
        
        // Notify InfiniteMap about wave completion
        InfiniteMap infiniteMap = FindFirstObjectByType<InfiniteMap>();
        if (infiniteMap != null)
        {
            infiniteMap.OnWaveCompleted(_currentWaveIndex);
        }
        
        GameManager.Instance.WaveCompletedCallback();
    }
    
    private void StartWave()
    {
        // Determine if this is a boss wave
        _isBossWave = (_currentWaveIndex % _bossInterval == 0);
        _bossSpawned = false;
        
        // Set wave duration
        _currentWaveDuration = _isBossWave ? _bossWaveDuration : _baseWaveDuration;
        
        // Scale spawn rate based on wave
        _currentSpawnRate = _baseSpawnRate * (1f + (_spawnRateIncreasePerWave * (_currentWaveIndex - 1)));

        // Update unlock manager with current endless wave progress

        CharacterUnlockManager unlockManager = FindFirstObjectByType<CharacterUnlockManager>();

        if (unlockManager != null)
        {
            unlockManager.UpdateEndlessWaveProgress(_currentWaveIndex);
        }
        
        // Update UI
        string waveText = _isBossWave ? 
            $"Boss Wave: {_currentWaveIndex}" : 
            $"Wave: {_currentWaveIndex}";
        _ui.UpdateWaveText(waveText, _isBossWave);
        
        // Start the wave
        _waveTime = 0f;
        _lastSpawnTime = Time.time;
        _isTimerOn = true;
        
        Debug.Log($"Started {(_isBossWave ? "Boss " : "")}Wave {_currentWaveIndex}");
    }
    
    private void DefeatAllEnemies()
    {
        foreach (Enemy enemy in transform.GetComponentsInChildren<Enemy>())
            enemy.PassAwayAfterWave();
            
        // Also return any active enemy bullets to their pools
        EnemyBulletScript[] enemyBullets = FindObjectsByType<EnemyBulletScript>(FindObjectsSortMode.None);
        foreach (EnemyBulletScript bullet in enemyBullets)
        {
            if (bullet != null && bullet.gameObject.activeInHierarchy)
                bullet.PassAwayAfterWave();
        }
    }
    
    public void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GAME:
                StartWave();
                break;
                
            case GameState.GAMEOVER:
                _isTimerOn = false;
                DefeatAllEnemies();
                break;
        }
    }
    
    public int GetCurrentWave() => _currentWaveIndex;
    public int GetCurrentWaveNumber() => _currentWaveIndex;
    public bool IsBossWave() => _isBossWave;
    public float GetDifficultyMultiplier() => 1f + (_difficultyScalePerWave * (_currentWaveIndex - 1));
    
    /// <summary>
    /// Resets the endless wave manager to its initial state for a new game run
    /// </summary>
    public void ResetWaveState()
    {
        _isTimerOn = false;
        _currentWaveIndex = 1;
        _waveTime = 0f;
        _isBossWave = false;
        _bossSpawned = false;
        _lastSpawnTime = Time.time;
        _currentSpawnRate = _baseSpawnRate;
        
        DefeatAllEnemies();
        Debug.Log("Endless Wave Manager state reset for new run");
    }
}
