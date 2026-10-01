using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance;

    [Header("Cheat System Settings")]
    [SerializeField] private bool _cheatsEnabled = false;
    [SerializeField] private KeyCode _cheatMenuToggleKey = KeyCode.F1;

    [Header("UI References")]
    [SerializeField] private GameObject _cheatPanel;
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] private Button _cheatButtonPrefab;
    [SerializeField] private Canvas _cheatCanvas;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField _goldAmountInput;
    [SerializeField] private TMP_InputField _chestDropChanceInput;

    [Header("Cheat Status")]
    [SerializeField] private bool _infiniteHealthActive = false;
    [SerializeField] private bool _oneHitKillActive = false;
    [SerializeField] private float _originalChestDropChance = 0f;

    // System References
    private PlayerHealth _playerHealth;
    private CurrencyManager _currencyManager;
    private WaveManager _waveManager;
    private EndlessWaveManager _endlessWaveManager;
    private PlayerStatsManager _playerStatsManager;
    private DropManager _dropManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeReferences();
        SetupCheatUI();
        
        if (_cheatPanel != null)
            _cheatPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(_cheatMenuToggleKey))
        {
            ToggleCheatMenu();
        }

        // Handle infinite health
        if (_infiniteHealthActive && _playerHealth != null)
        {
            _playerHealth.UpdateStats(_playerStatsManager);
        }
    }

    private void InitializeReferences()
    {
        _playerHealth = FindFirstObjectByType<PlayerHealth>();
        _currencyManager = CurrencyManager.instance;
        _waveManager = WaveManager.instance;
        _endlessWaveManager = FindFirstObjectByType<EndlessWaveManager>();
        _playerStatsManager = FindFirstObjectByType<PlayerStatsManager>();
        _dropManager = FindFirstObjectByType<DropManager>();

        // Store original chest drop chance
        if (_dropManager != null)
        {
            var field = typeof(DropManager).GetField("_chestDropChance", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
                _originalChestDropChance = (float)field.GetValue(_dropManager);
        }
    }

    private void SetupCheatUI()
    {
        if (_buttonContainer == null || _cheatButtonPrefab == null) return;

        //CreateCheatButton("God Mode", ToggleGodMode);
        CreateCheatButton("Infinite Health", ToggleInfiniteHealth);
        CreateCheatButton("Level Up", LevelUpPlayer);
        CreateCheatButton("Add 100 Gold", () => AddGold(GetGoldAmount()));
        CreateCheatButton("Complete Wave", CompleteCurrentWave);
        CreateCheatButton("Kill All Enemies", KillAllEnemies);
        CreateCheatButton("One Hit Kill", ToggleOneHitKill);
        //CreateCheatButton("Set Chest Drop Rate", () => SetChestDropRate(GetChestDropChance()));
        //CreateCheatButton("Max All Stats", MaxAllStats);
        //CreateCheatButton("Reset All Stats", ResetAllStats);
        //CreateCheatButton("Unlock All Characters", UnlockAllCharacters);
        CreateCheatButton("Full Health Recovery", FullHealthRecovery);
        CreateCheatButton("Spawn Boss", SpawnBoss);
        //CreateCheatButton("Add 1000 Gems", () => AddGems(1000));
        CreateCheatButton("Reset Wave", ResetCurrentWave);
    }

    private void CreateCheatButton(string buttonText, System.Action buttonAction)
    {
        if (_cheatButtonPrefab == null || _buttonContainer == null) return;

        Button newButton = Instantiate(_cheatButtonPrefab, _buttonContainer);
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        newButton.onClick.AddListener(() => {
            if (_cheatsEnabled)
                buttonAction.Invoke();
        });
    }

    #region Cheat Functions

    public void ToggleInfiniteHealth()
    {
        _infiniteHealthActive = !_infiniteHealthActive;
        Debug.Log($"Infinite Health: {(_infiniteHealthActive ? "ON" : "OFF")}");
        
        if (_infiniteHealthActive && _playerHealth != null)
        {
            // Set health to max
            var healthField = typeof(PlayerHealth).GetField("_health", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var maxHealthField = typeof(PlayerHealth).GetField("_maxHealth", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (healthField != null && maxHealthField != null)
            {
                float maxHealth = (float)maxHealthField.GetValue(_playerHealth);
                healthField.SetValue(_playerHealth, maxHealth);
            }
        }
    }

    public void LevelUpPlayer()
    {
        if (_playerStatsManager != null)
        {
            var method = typeof(PlayerStatsManager).GetMethod("LevelUp", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(_playerStatsManager, null);
            Debug.Log("Player leveled up!");
        }
    }

    public void AddGold(int amount)
    {
        if (_currencyManager != null)
        {
            _currencyManager.AddCurrency(amount);
            Debug.Log($"Added {amount} gold");
        }
    }

    public void AddGems(int amount)
    {
        if (_currencyManager != null)
        {
            _currencyManager.AddGemCurrency(amount);
            Debug.Log($"Added {amount} gems");
        }
    }

    public void CompleteCurrentWave()
    {
        if (_waveManager != null)
        {
            // Force complete current wave
            var method = typeof(WaveManager).GetMethod("StartWaveTransition", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(_waveManager, null);
            Debug.Log("Current wave completed!");
        }
        else if (_endlessWaveManager != null)
        {
            var method = typeof(EndlessWaveManager).GetMethod("StartWaveTransition", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(_endlessWaveManager, null);
            Debug.Log("Endless wave completed!");
        }
    }

    public void KillAllEnemies()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            enemy.PassAway();
        }
        Debug.Log($"Killed {enemies.Length} enemies");
    }

    public void ToggleOneHitKill()
    {
        _oneHitKillActive = !_oneHitKillActive;
        Debug.Log($"One Hit Kill: {(_oneHitKillActive ? "ON" : "OFF")}");
        
        // Subscribe/unsubscribe to enemy damage events
        if (_oneHitKillActive)
        {
            // We'll need to modify enemy TakeDamage methods or use reflection
            ModifyAllEnemyHealth();
        }
    }

    private void ModifyAllEnemyHealth()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            if (_oneHitKillActive)
            {
                // Set enemy health to 1 for one-hit kill
                var healthField = typeof(Enemy).GetField("_health", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                healthField?.SetValue(enemy, 1f);
            }
        }
    }

    public void SetChestDropRate(float percentage)
    {
        if (_dropManager != null)
        {
            var field = typeof(DropManager).GetField("_chestDropChance", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(_dropManager, percentage);
            Debug.Log($"Chest drop rate set to {percentage}%");
        }
    }

    public void MaxAllStats()
    {
        if (_playerStatsManager != null)
        {
            var stats = System.Enum.GetValues(typeof(Stats)).Cast<Stats>();
            foreach (Stats stat in stats)
            {
                _playerStatsManager.AddPlayerStat(stat, 999f);
            }
            Debug.Log("All stats maxed!");
        }
    }

    public void ResetAllStats()
    {
        if (_playerStatsManager != null)
        {
            var method = typeof(PlayerStatsManager).GetMethod("ResetStats", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(_playerStatsManager, null);
            Debug.Log("All stats reset!");
        }
    }

    public void UnlockAllCharacters()
    {
        CharacterUnlockManager unlockManager = FindFirstObjectByType<CharacterUnlockManager>();
        if (unlockManager != null)
        {
            var method = typeof(CharacterUnlockManager).GetMethod("UnlockAllCharacters", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            method?.Invoke(unlockManager, null);
            Debug.Log("All characters unlocked!");
        }
    }

    public void FullHealthRecovery()
    {
        if (_playerHealth != null)
        {
            var healthField = typeof(PlayerHealth).GetField("_health", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var maxHealthField = typeof(PlayerHealth).GetField("_maxHealth", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (healthField != null && maxHealthField != null)
            {
                float maxHealth = (float)maxHealthField.GetValue(_playerHealth);
                healthField.SetValue(_playerHealth, maxHealth);
                Debug.Log("Health fully recovered!");
            }
        }
    }

    public void SpawnBoss()
    {
        if (_waveManager != null)
        {
            // This would need specific boss prefabs
            Debug.Log("Boss spawn functionality needs specific implementation");
        }
    }

    public void ResetCurrentWave()
    {
        KillAllEnemies();
        if (_waveManager != null)
        {
            var field = typeof(WaveManager).GetField("_currentWaveTime", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(_waveManager, 0f);
            Debug.Log("Current wave reset!");
        }
    }

    public void ToggleGodMode()
    {
        _infiniteHealthActive = !_infiniteHealthActive;
        _oneHitKillActive = _infiniteHealthActive;
        
        if (_infiniteHealthActive)
        {
            MaxAllStats();
            SetChestDropRate(100f);
        }
        else
        {
            SetChestDropRate(_originalChestDropChance);
        }
        
        Debug.Log($"God Mode: {(_infiniteHealthActive ? "ON" : "OFF")}");
    }

    #endregion

    #region UI Helpers

    private int GetGoldAmount()
    {
        if (_goldAmountInput != null && int.TryParse(_goldAmountInput.text, out int amount))
            return amount;
        return 200; // Default amount
    }

    private float GetChestDropChance()
    {
        if (_chestDropChanceInput != null && float.TryParse(_chestDropChanceInput.text, out float chance))
            return Mathf.Clamp(chance, 0f, 100f);
        return 10f; // Default chance
    }

    public void ToggleCheatMenu()
    {
        if (_cheatPanel != null)
        {
            _cheatPanel.SetActive(!_cheatPanel.activeSelf);
        }
    }

    public void EnableCheats()
    {
        _cheatsEnabled = true;
        Debug.Log("Cheats ENABLED");
    }

    public void DisableCheats()
    {
        _cheatsEnabled = false;
        _infiniteHealthActive = false;
        _oneHitKillActive = false;
        if (_dropManager != null)
            SetChestDropRate(_originalChestDropChance);
        Debug.Log("Cheats DISABLED");
    }

    public void ToggleCheats()
    {
        if (_cheatsEnabled)
            DisableCheats();
        else
            EnableCheats();
    }

    #endregion

    #region Properties

    public bool CheatsEnabled => _cheatsEnabled;
    public bool InfiniteHealthActive => _infiniteHealthActive;
    public bool OneHitKillActive => _oneHitKillActive;

    #endregion

}
