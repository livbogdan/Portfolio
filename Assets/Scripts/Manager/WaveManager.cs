using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public enum WaveMode
{
    Structured,  // Traditional wave-based with predefined enemies
    Endless      // Infinite waves with scaling difficulty
}

//TODO: Refactor BOSS UI

[RequireComponent(typeof(WaveManagerUI))]
public class WaveManager : MonoBehaviour, IGameStateListner
{
    public static WaveManager instance;
    
    [Header("Mode Selection")]
    [SerializeField] private WaveMode _waveMode = WaveMode.Structured;
    
    [Header("References")]
    [SerializeField] private Player _player;

    [Header("Spawn Settings")]
    [Tooltip("The minimum distance from the player where enemies can spawn")]
    [SerializeField] private float _minOfset = 5f;
    [Tooltip("The maximum distance from the player where enemies can spawn")]
    [SerializeField] private float _maxOffset = 15f;
    
    // Shared variables
    private bool _isTimerOn;
    private int _currentWaveIndex;
    private WaveManagerUI _ui;

    #region Structured Mode Settings
    [Header("Structured Mode")]
    [ShowIf("_waveMode", WaveMode.Structured)]
    [SerializeField] private Wave[] _waves;
    [ShowIf("_waveMode", WaveMode.Structured)]
    [Tooltip("Base gold amount per wave (multiplied by wave number)")]
    [SerializeField] private int _goldPerWave = 50;
    
    // Structured mode variables
    private List<float> _localCounters = new List<float>();
    private float _currentWaveTime;
    private float _currentSegmentDuration;
    private int _currentSegmentIndex;
    private BossEnemy _currentBoss;
    private bool _waitingForBossDefeat;
    #endregion
    
    #region Endless Mode Settings
    [Header("Endless Mode")]
    [ShowIf("_waveMode", WaveMode.Endless)]
    [SerializeField] private GameObject[] _normalEnemyPrefabs;
    [ShowIf("_waveMode", WaveMode.Endless)]
    [SerializeField] private GameObject[] _bossPrefabs;
    [ShowIf("_waveMode", WaveMode.Endless)]
    [SerializeField] private float _baseWaveDuration = 30f;
    [ShowIf("_waveMode", WaveMode.Endless)]
    [SerializeField] private float _bossWaveDuration = 60f;
    [ShowIf("_waveMode", WaveMode.Endless)]
    [SerializeField] private int _bossInterval = 5;
    
    [Header("Endless Mode - Difficulty Scaling")]
    [ShowIf("_waveMode", WaveMode.Endless)]
    [SerializeField] private float _difficultyScalePerWave = 0.1f;
    [ShowIf("_waveMode", WaveMode.Endless)]
    [SerializeField] private float _spawnRateIncreasePerWave = 0.05f;
    [ShowIf("_waveMode", WaveMode.Endless)]
    [SerializeField] private float _healthScalePerWave = 0.15f;
    [ShowIf("_waveMode", WaveMode.Endless)]
    [SerializeField] private float _damageScalePerWave = 0.1f;
    
    // Endless mode variables
    private float _waveTime;
    private bool _isBossWave;
    private bool _bossSpawned;
    private float _currentSpawnRate;
    private float _lastSpawnTime;
    private float _baseSpawnRate = 2f;
    private float _currentWaveDuration;
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
            
        _ui = GetComponent<WaveManagerUI>();
    }
    
    private void Start()
    {
        if (_waveMode == WaveMode.Endless)
        {
            _currentSpawnRate = _baseSpawnRate;
            _currentWaveIndex = 1; // Endless mode starts at wave 1
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isTimerOn)
            return;
        
        if (_waveMode == WaveMode.Structured)
            UpdateStructuredMode();
        else
            UpdateEndlessMode();
    }
    
    #region Structured Mode Logic
    private void UpdateStructuredMode()
    {
        // If waiting for boss defeat, don't progress the timer
        if (_waitingForBossDefeat)
        {
            // Check if boss is still alive
            if (_currentBoss == null || _currentBoss.gameObject == null)
            {
                _waitingForBossDefeat = false;
                _currentBoss = null;
                
                // Trigger boss kill event for unlock progression
                GameManager.OnBossKilled?.Invoke();
                
                
                // Move to next segment immediately after boss defeat
                _currentSegmentIndex++;
                if (_currentSegmentIndex < _waves[_currentWaveIndex]._segments.Count)
                {
                    StartNextSegment();
                }
                else
                {
                    StartStructuredWaveTransition();
                }
            }
            return;
        }

        // Guard: Don't process if timer isn't running
        if (!_isTimerOn)
            return;

        if (_currentWaveTime < _currentSegmentDuration)
        {
            ManageStructuredWave();

            // Only show timer for non-boss segments
            if (!_waves[_currentWaveIndex]._segments[_currentSegmentIndex]._spawnOnce)
            {
                string timerString = ((int)(_currentSegmentDuration - _currentWaveTime)).ToString();
                _ui.UpdateTimerText(timerString);
            }
            else
            {
                _ui.HideTimer();
            }

            _currentWaveTime += Time.deltaTime;
        }
        else
        {
            _currentSegmentIndex++;
            if (_currentSegmentIndex < _waves[_currentWaveIndex]._segments.Count)
            {
                StartNextSegment();
            }
            else
            {
                StartStructuredWaveTransition();
            }
        }
    }

    private void StartNextSegment()
    {
        _currentSegmentDuration = _waves[_currentWaveIndex]._waveDuration;
        _currentWaveTime = 0f;
        _isTimerOn = true;
        
        // Update UI for the new segment
        UpdateStructuredWaveUI();
    }
    
    private void StartStructuredWaveTransition()
    {
        _isTimerOn = false;
        _currentWaveTime = 0f;  // Reset timer to prevent double execution
        _currentSegmentIndex = 0;  // Reset segment index for next wave
        
        DefeatAllEnemies();

        // Give wave completion reward
        GiveWaveCompletionReward(_currentWaveIndex + 1); // +1 because waves are 1-based

        _currentWaveIndex++;

        if (_currentWaveIndex >= _waves.Length)
        {
            _ui.ShowWavesCompleted();
            GameManager.Instance.SetGameState(GameState.STAGECOMPLETE);
        }
        else
            GameManager.Instance.WaveCompletedCallback();
    }

    private void StartStructuredWave(int waveIndex)
    {
        _localCounters.Clear();
        for (int i = 0; i < _waves[waveIndex]._segments.Count; i++)
        {
            _localCounters.Add(1);
        }

        _currentSegmentIndex = 0;
        _waitingForBossDefeat = false; // Reset boss state
        _currentBoss = null;

        StartNextSegment(); // Start the first segment of the wave
        UpdateStructuredWaveUI(); // Update UI after segment is started so _isTimerOn is set
    }

    private void UpdateStructuredWaveUI()
    {
        // Check if current segment has a boss
        if (_currentSegmentIndex < _waves[_currentWaveIndex]._segments.Count && 
            _waves[_currentWaveIndex]._segments[_currentSegmentIndex]._spawnOnce)
        {
            _ui.ShowBossWave(_currentWaveIndex + 1, _waves.Length);
        }
        else
        {
            _ui.ResetWaveTextFormatting();
            _ui.UpdateWaveText("Wave: " + (_currentWaveIndex + 1) + " / " + _waves.Length);
        }

        // Update segment name if available
        if (_currentSegmentIndex < _waves[_currentWaveIndex]._segments.Count)
        {
            string segmentName = _waves[_currentWaveIndex]._segments[_currentSegmentIndex]._name;
            if (!string.IsNullOrEmpty(segmentName))
            {
                _ui.UpdateSegmentText(segmentName);
            }
        }
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

    private void ManageStructuredWave()
    {
        Wave currentWave = _waves[_currentWaveIndex];
        WaveSegment segment = currentWave._segments[_currentSegmentIndex];

        float spawnDelay = 1f / segment._spawnSequence;

        if (_currentWaveTime / spawnDelay > _localCounters[_currentSegmentIndex])
        {
            GameObject enemyPrefab = GetRandomEnemyFromSegment(segment);
            GameObject spawnedEnemy = Instantiate(
                enemyPrefab,
                GetSpawnPosition(),
                Quaternion.identity,
                transform);
            _localCounters[_currentSegmentIndex]++;

            if (segment._spawnOnce)
            {
                _localCounters[_currentSegmentIndex] += Mathf.Infinity;
                // This is a boss enemy - wait for its defeat
                _waitingForBossDefeat = true;
                _currentBoss = spawnedEnemy.GetComponent<BossEnemy>();
                _isTimerOn = false; // Stop the timer until boss is defeated
            }
        }
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = Random.onUnitSphere;
        Vector2 offset = direction.normalized * Random.Range(_minOfset, _maxOffset);
        Vector2 targetPosition = (Vector2)_player.transform.position + offset;

        if(!GameManager.Instance.UseInfiniteMap)
        {
            targetPosition.x = Math.Clamp(targetPosition.x, -16, 19);
            targetPosition.y = Math.Clamp(targetPosition.y, -9, 9);
        }

        return targetPosition;
    }

    private GameObject GetRandomEnemyFromSegment(WaveSegment segment)
    {
        if (segment._enemies == null || segment._enemies.Length == 0)
            return null;

        if (segment._enemies.Length == 1)
            return segment._enemies[0]._enemyPrefab;

        // Calculate total weight
        float totalWeight = 0f;
        foreach (var enemy in segment._enemies)
        {
            totalWeight += enemy._spawnWeight;
        }

        // Random selection based on weights
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var enemy in segment._enemies)
        {
            currentWeight += enemy._spawnWeight;
            if (randomValue <= currentWeight)
            {
                return enemy._enemyPrefab;
            }
        }

        // Fallback to first enemy if something goes wrong
        return segment._enemies[0]._enemyPrefab;
    }
    
    private void GiveWaveCompletionReward(int waveNumber)
    {
        if (CurrencyManager.instance == null)
        {
            Debug.LogError("WaveManager: CurrencyManager.instance is null! Cannot give wave reward.");
            return;
        }
        
        int goldAmount = waveNumber * _goldPerWave;
        CurrencyManager.instance.AddCurrency(goldAmount);
        Debug.Log($"Wave {waveNumber} completed! Earned {goldAmount} gold.");
    }
    #endregion
    
    #region Endless Mode Logic
    private void UpdateEndlessMode()
    {
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
                    GameManager.OnBossKilled?.Invoke();
                    StartEndlessWaveTransition();
                }
            }
            else if (!_isBossWave)
            {
                StartEndlessWaveTransition();
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
    
    private void StartEndlessWave()
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
    
    private void HandleNormalWave()
    {
        if (Time.time - _lastSpawnTime >= (1f / _currentSpawnRate))
        {
            SpawnRandomNormalEnemy();
            _lastSpawnTime = Time.time;
        }
    }
    
    private void HandleBossWave()
    {
        if (!_bossSpawned)
        {
            SpawnBoss();
            _bossSpawned = true;
        }
        
        // Spawn some minions during boss fight
        if (Time.time - _lastSpawnTime >= (2f / _currentSpawnRate))
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
        
        ScaleEnemyStats(enemy);
    }
    
    private void SpawnBoss()
    {
        if (_bossPrefabs.Length == 0) return;
        
        GameObject bossPrefab = _bossPrefabs[Random.Range(0, _bossPrefabs.Length)];
        GameObject boss = Instantiate(bossPrefab, GetSpawnPosition(), Quaternion.identity, transform);
        
        ScaleBossStats(boss);
    }
    
    private void ScaleEnemyStats(GameObject enemy)
    {
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent == null) return;
        
        float healthMultiplier = 1f + (_healthScalePerWave * (_currentWaveIndex - 1));
        float damageMultiplier = 1f + (_damageScalePerWave * (_currentWaveIndex - 1));
        
        enemyComponent.ScaleStats(healthMultiplier, damageMultiplier);
    }
    
    private void ScaleBossStats(GameObject boss)
    {
        Enemy bossComponent = boss.GetComponent<Enemy>();
        if (bossComponent == null) return;
        
        float healthMultiplier = 1f + (_healthScalePerWave * (_currentWaveIndex - 1) * 2f);
        float damageMultiplier = 1f + (_damageScalePerWave * (_currentWaveIndex - 1) * 1.5f);
        
        bossComponent.ScaleStats(healthMultiplier, damageMultiplier);
    }
    
    private void StartEndlessWaveTransition()
    {
        _isTimerOn = false;
        DefeatAllEnemies();
        
        // Give wave completion reward before incrementing wave number
        GiveWaveCompletionReward(_currentWaveIndex);
        
        _currentWaveIndex++;
        
        // Notify InfiniteMap about wave completion
        InfiniteMap infiniteMap = FindFirstObjectByType<InfiniteMap>();
        if (infiniteMap != null)
        {
            infiniteMap.OnWaveCompleted(_currentWaveIndex);
        }
        
        GameManager.Instance.WaveCompletedCallback();
    }
    #endregion
    
    #region Shared Methods
    public void GameStateChangedCallback(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.GAME:
                if (_waveMode == WaveMode.Structured)
                    StartStructuredWave(_currentWaveIndex);
                else
                    StartEndlessWave();
                break;

            case GameState.GAMEOVER:
                _isTimerOn = false;
                _waitingForBossDefeat = false;
                _currentBoss = null;
                DefeatAllEnemies();
                break;
        }
    }
    
    public int GetCurrentWaveNumber()
    {
        if (_waveMode == WaveMode.Structured)
            return _currentWaveIndex + 1; // +1 because index is 0-based
        else
            return _currentWaveIndex; // Endless mode already starts at 1
    }
    
    public bool IsBossWave()
    {
        if (_waveMode == WaveMode.Endless)
            return _isBossWave;
        else
            return _currentSegmentIndex < _waves[_currentWaveIndex]._segments.Count && 
                   _waves[_currentWaveIndex]._segments[_currentSegmentIndex]._spawnOnce;
    }
    
    public float GetDifficultyMultiplier()
    {
        if (_waveMode == WaveMode.Endless)
            return 1f + (_difficultyScalePerWave * (_currentWaveIndex - 1));
        else
            return 1f;
    }
    
    public WaveMode GetCurrentMode() => _waveMode;
    
    public void SetWaveMode(WaveMode mode)
    {
        _waveMode = mode;
        _currentWaveIndex = _waveMode == WaveMode.Endless ? 1 : 0;
    }
    
    /// <summary>
    /// Resets the wave manager to its initial state for a new game run
    /// </summary>
    public void ResetWaveState()
    {
        _isTimerOn = false;
        _waitingForBossDefeat = false;
        _currentBoss = null;
        
        if (_waveMode == WaveMode.Structured)
        {
            _currentWaveIndex = 0;
            _currentSegmentIndex = 0;
            _currentWaveTime = 0f;
            _localCounters.Clear();
        }
        else // Endless mode
        {
            _currentWaveIndex = 1;
            _waveTime = 0f;
            _bossSpawned = false;
            _isBossWave = false;
            _lastSpawnTime = Time.time;
        }
        
        DefeatAllEnemies();
        Debug.Log("Wave Manager state reset for new run");
    }
    #endregion
}

[Serializable]
public struct Wave
{
    public float _waveDuration; // Duration of this segment
    public string _name;
    public List<WaveSegment> _segments;
}

[Serializable]
public struct WaveSegment
{
    public string _name; // Segment name for UI display
    [MinMaxSlider(0, 100)] public Vector2 _TStartEnd; // Now a percentage of the segment duration
    public float _spawnSequence;
    public EnemySpawnData[] _enemies; // Array of enemy types with weights
    public bool _spawnOnce;
}

[Serializable]
public struct EnemySpawnData
{
    public GameObject _enemyPrefab;
    [Range(0f, 100f)] public float _spawnWeight; // Percentage chance to spawn this enemy
}