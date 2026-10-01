using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages permanent stat upgrades purchased with gems between runs.
/// This provides the classic roguelite meta-progression loop.
/// </summary>
public class MetaUpgradeManager : MonoBehaviour
{
    public static MetaUpgradeManager instance;

    // Track upgrade levels for each character and stat
    // Key: CharacterName, Value: Dictionary of stat upgrades
    private Dictionary<string, Dictionary<Stats, int>> _characterUpgrades = new Dictionary<string, Dictionary<Stats, int>>();
    
    // Current active character config
    private CharacterUpgradeConfigSO _currentCharacterConfig;
    private string _currentCharacterName = "";

    // Events
    public static Action OnUpgradesChanged;

    private const string SAVE_KEY = "MetaUpgradeLevels_PerCharacter";
    
    private bool _isLoaded = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeUpgradeLevels();
    }
    

    /// <summary>
    /// Set the current active character for upgrades
    /// Call this when a character is selected
    /// </summary>
    public void SetCurrentCharacter(CharterDataSO characterData)
    {
        if (characterData == null)
        {
            Debug.LogError("Character data is null!");
            return;
        }
        
        _currentCharacterName = characterData.CharterName;
        _currentCharacterConfig = characterData.UpgradeConfig;
        
        if (_currentCharacterConfig == null)
        {
            Debug.LogWarning($"No upgrade config found for {_currentCharacterName}. Character will have no upgrades available.");
            return;
        }
        
        // Initialize upgrades for this character if not exists
        if (!_characterUpgrades.ContainsKey(_currentCharacterName))
        {
            _characterUpgrades[_currentCharacterName] = new Dictionary<Stats, int>();
        }
        
        // Always ensure all stats are initialized (handles loaded saves missing new stats)
        InitializeUpgradeLevelsForCharacter(_currentCharacterName);
        
        OnUpgradesChanged?.Invoke();
    }
    
    /// <summary>
    /// Set current character by name (legacy support)
    /// </summary>
    public void SetCurrentCharacter(string characterName)
    {
        _currentCharacterName = characterName;
        _currentCharacterConfig = null;
        
        // Try to find config from ResourceManager if available
        CharterDataSO[] allCharacters = ResourceManager.Character;
        if (allCharacters != null)
        {
            CharterDataSO characterData = System.Array.Find(allCharacters, c => c.CharterName == characterName);
            if (characterData != null)
            {
                _currentCharacterConfig = characterData.UpgradeConfig;
            }
        }
        
        if (!_characterUpgrades.ContainsKey(characterName))
        {
            _characterUpgrades[characterName] = new Dictionary<Stats, int>();
        }
        
        // Always ensure all stats are initialized (handles loaded saves missing new stats)
        InitializeUpgradeLevelsForCharacter(characterName);
        
        OnUpgradesChanged?.Invoke();
    }

    /// <summary>
    /// Get the current active character name
    /// </summary>
    public string GetCurrentCharacterName()
    {
        return _currentCharacterName;
    }

    private void InitializeUpgradeLevels()
    {
        // This will be called per character now
    }
    
    private void InitializeUpgradeLevelsForCharacter(string characterName)
    {
        Dictionary<Stats, int> characterUpgrades = _characterUpgrades[characterName];
        
        // If we have a config, use it to initialize stats
        if (_currentCharacterConfig != null)
        {
            List<Stats> enabledStats = _currentCharacterConfig.GetEnabledStats();
            foreach (Stats stat in enabledStats)
            {
                if (!characterUpgrades.ContainsKey(stat))
                    characterUpgrades[stat] = 0;
            }
        }
        else
        {
            // Fallback: Initialize all default stats
            Stats[] upgradeableStats = new Stats[]
            {
                Stats.MaxHealth,
                Stats.Attack,
                Stats.AttackSpeed,
                Stats.CriticalChance,
                Stats.MoveSpeed,
                Stats.Armor,
                Stats.Dodge,
                Stats.LifeSteal,
                Stats.Range,
                Stats.HealthRecoverySpeed,
                Stats.Luck
            };

            foreach (Stats stat in upgradeableStats)
            {
                if (!characterUpgrades.ContainsKey(stat))
                    characterUpgrades[stat] = 0;
            }
        }
    }

    /// <summary>
    /// Get the current upgrade level for a stat (for current character)
    /// </summary>
    public int GetUpgradeLevel(Stats stat)
    {
        if (string.IsNullOrEmpty(_currentCharacterName))
            return 0;
            
        if (!_characterUpgrades.ContainsKey(_currentCharacterName))
            return 0;
            
        Dictionary<Stats, int> characterUpgrades = _characterUpgrades[_currentCharacterName];
        return characterUpgrades.ContainsKey(stat) ? characterUpgrades[stat] : 0;
    }
    
    /// <summary>
    /// Get upgrade level for a specific character
    /// </summary>
    public int GetUpgradeLevelForCharacter(string characterName, Stats stat)
    {
        if (!_characterUpgrades.ContainsKey(characterName))
            return 0;
            
        Dictionary<Stats, int> characterUpgrades = _characterUpgrades[characterName];
        return characterUpgrades.ContainsKey(stat) ? characterUpgrades[stat] : 0;
    }

    /// <summary>
    /// Get the cost for the next upgrade level of a stat
    /// Cost increases exponentially based on character config
    /// </summary>
    public int GetUpgradeCost(Stats stat)
    {
        int level = GetUpgradeLevel(stat);
        
        // Use config if available
        if (_currentCharacterConfig != null)
        {
            CharacterUpgradeConfigSO.StatUpgradeConfig config = _currentCharacterConfig.GetStatConfig(stat);
            if (config == null)
                return -1; // Stat not upgradeable for this character
                
            if (level >= config.maxLevel)
                return -1; // Max level reached
                
            return config.baseCost * (int)Mathf.Pow(2, level);
        }
        
        // Fallback to default values
        if (level >= 10)
            return -1;
            
        return 10 * (int)Mathf.Pow(2, level);
    }

    /// <summary>
    /// Check if player can afford an upgrade
    /// </summary>
    public bool CanAffordUpgrade(Stats stat)
    {
        int cost = GetUpgradeCost(stat);
        return cost > 0 && CurrencyManager.instance.HasEnoughtGemCurrency(cost);
    }

    /// <summary>
    /// Check if upgrade has reached max level
    /// </summary>
    public bool IsMaxLevel(Stats stat)
    {
        int currentLevel = GetUpgradeLevel(stat);
        
        // Use config if available
        if (_currentCharacterConfig != null)
        {
            CharacterUpgradeConfigSO.StatUpgradeConfig config = _currentCharacterConfig.GetStatConfig(stat);
            if (config == null)
                return true; // Not upgradeable = max level
                
            return currentLevel >= config.maxLevel;
        }
        
        // Fallback
        return currentLevel >= 10;
    }
    
    /// <summary>
    /// Get max level for a stat
    /// </summary>
    public int GetMaxLevel(Stats stat)
    {
        if (_currentCharacterConfig != null)
        {
            CharacterUpgradeConfigSO.StatUpgradeConfig config = _currentCharacterConfig.GetStatConfig(stat);
            if (config != null)
                return config.maxLevel;
        }
        return 10; // Fallback
    }

    /// <summary>
    /// Purchase an upgrade with gems (for current character)
    /// </summary>
    public bool PurchaseUpgrade(Stats stat)
    {
        if (string.IsNullOrEmpty(_currentCharacterName))
        {
            Debug.LogError("No character selected for upgrade!");
            return false;
        }
        
        if (!_characterUpgrades.ContainsKey(_currentCharacterName))
        {
            Debug.LogError($"Character {_currentCharacterName} not found in upgrades dictionary!");
            return false;
        }
        
        if (IsMaxLevel(stat))
        {
            Debug.LogWarning($"Stat {stat} is already at max level for {_currentCharacterName}!");
            return false;
        }

        int cost = GetUpgradeCost(stat);
        if (!CurrencyManager.instance.HasEnoughtGemCurrency(cost))
        {
            Debug.LogWarning($"Not enough gems to purchase {stat} upgrade!");
            return false;
        }

        // Deduct gems
        CurrencyManager.instance.UseGemCurrency(cost);

        // Ensure the stat key exists before incrementing
        if (!_characterUpgrades[_currentCharacterName].ContainsKey(stat))
        {
            _characterUpgrades[_currentCharacterName][stat] = 0;
        }
        
        // Increase upgrade level for current character
        _characterUpgrades[_currentCharacterName][stat]++;

        // Notify listeners
        OnUpgradesChanged?.Invoke();

        Debug.Log($"Purchased {stat} upgrade for {_currentCharacterName}! New level: {_characterUpgrades[_currentCharacterName][stat]}");
        return true;
    }

    /// <summary>
    /// Get the total bonus value for a stat based on upgrade level
    /// </summary>
    public float GetStatBonus(Stats stat)
    {
        int level = GetUpgradeLevel(stat);
        
        // Use config if available
        if (_currentCharacterConfig != null)
        {
            CharacterUpgradeConfigSO.StatUpgradeConfig config = _currentCharacterConfig.GetStatConfig(stat);
            if (config != null)
            {
                return level * config.bonusPerLevel;
            }
        }
        
        // Fallback to defaults
        return level * 5f;
    }

    /// <summary>
    /// Apply meta upgrades to the player at the start of a run
    /// This should be called by PlayerStatsManager when the character is selected
    /// </summary>
    public void ApplyMetaUpgrades(PlayerStatsManager statsManager, string characterName)
    {
        if (statsManager == null)
        {
            Debug.LogError("PlayerStatsManager is null! Cannot apply meta upgrades.");
            return;
        }

        if (string.IsNullOrEmpty(characterName))
        {
            Debug.LogError("Character name is null or empty! Cannot apply meta upgrades.");
            return;
        }

        if (!_characterUpgrades.ContainsKey(characterName))
        {
            Debug.Log($"No upgrades found for {characterName}. Starting fresh.");
            return;
        }

        Dictionary<Stats, int> characterUpgrades = _characterUpgrades[characterName];
        foreach (KeyValuePair<Stats, int> upgrade in characterUpgrades)
        {
            if (upgrade.Value > 0)
            {
                float bonus = GetStatBonusForCharacter(characterName, upgrade.Key);
                statsManager.AddPlayerStat(upgrade.Key, bonus);
                Debug.Log($"Applied meta upgrade for {characterName}: {upgrade.Key} +{bonus} (Level {upgrade.Value})");
            }
        }
    }
    
    /// <summary>
    /// Get the stat bonus for a specific character (used by UI to display stats with upgrades)
    /// </summary>
    public float GetStatBonusForCharacter(string characterName, Stats stat)
    {
        int level = GetUpgradeLevelForCharacter(characterName, stat);
        
        // Try to get character's config
        CharacterUpgradeConfigSO config = null;
        CharterDataSO[] allCharacters = ResourceManager.Character;
        if (allCharacters != null)
        {
            CharterDataSO characterData = System.Array.Find(allCharacters, c => c.CharterName == characterName);
            if (characterData != null && characterData.UpgradeConfig != null)
            {
                config = characterData.UpgradeConfig;
                CharacterUpgradeConfigSO.StatUpgradeConfig statConfig = config.GetStatConfig(stat);
                if (statConfig != null)
                {
                    return level * statConfig.bonusPerLevel;
                }
            }
        }
        
        // Fallback
        return level * 5f;
    }

    /// <summary>
    /// Get upgrade info for UI display
    /// </summary>
    public string GetUpgradeDescription(Stats stat)
    {
        int currentLevel = GetUpgradeLevel(stat);
        int maxLevel = GetMaxLevel(stat);
        float currentBonus = GetStatBonus(stat);
        float nextBonus = GetStatBonus(stat) + GetBonusPerLevel(stat);
        
        if (IsMaxLevel(stat))
        {
            return $"MAX LEVEL ({currentLevel}/{maxLevel})\n+{currentBonus:F1} {stat}";
        }

        return $"Level {currentLevel}/{maxLevel}\nCurrent: +{currentBonus:F1}\nNext: +{nextBonus:F1}";
    }

    private float GetBonusPerLevel(Stats stat)
    {
        if (_currentCharacterConfig != null)
        {
            CharacterUpgradeConfigSO.StatUpgradeConfig config = _currentCharacterConfig.GetStatConfig(stat);
            if (config != null)
            {
                return config.bonusPerLevel;
            }
        }
        return 5f; // Fallback
    }


    #region Debug Methods

    [ContextMenu("Reset All Upgrades")]
    private void ResetAllUpgrades()
    {
        _characterUpgrades.Clear();
        OnUpgradesChanged?.Invoke();
        Debug.Log("All meta upgrades reset for all characters!");
    }
    
    [ContextMenu("Reset Current Character Upgrades")]
    private void ResetCurrentCharacterUpgrades()
    {
        if (string.IsNullOrEmpty(_currentCharacterName))
        {
            Debug.LogWarning("No character selected!");
            return;
        }
        
        if (_characterUpgrades.ContainsKey(_currentCharacterName))
        {
            _characterUpgrades.Remove(_currentCharacterName);
            OnUpgradesChanged?.Invoke();
            Debug.Log($"All upgrades reset for {_currentCharacterName}!");
        }
    }

    [ContextMenu("Print Current Upgrades")]
    private void PrintCurrentUpgrades()
    {
        if (string.IsNullOrEmpty(_currentCharacterName))
        {
            Debug.LogWarning("No character selected!");
            return;
        }
        
        Debug.Log($"=== Meta Upgrades for {_currentCharacterName} ===");
        
        if (!_characterUpgrades.ContainsKey(_currentCharacterName))
        {
            Debug.Log("No upgrades found for this character.");
            return;
        }
        
        Dictionary<Stats, int> characterUpgrades = _characterUpgrades[_currentCharacterName];
        foreach (KeyValuePair<Stats, int> upgrade in characterUpgrades)
        {
            if (upgrade.Value > 0)
            {
                Debug.Log($"{upgrade.Key}: Level {upgrade.Value} (+{GetStatBonus(upgrade.Key)})");
            }
        }
    }
    
    [ContextMenu("Print All Characters Upgrades")]
    private void PrintAllCharactersUpgrades()
    {
        Debug.Log($"=== Meta Upgrades for ALL Characters ({_characterUpgrades.Count}) ===");
        
        foreach (KeyValuePair<string, Dictionary<Stats, int>> characterData in _characterUpgrades)
        {
            Debug.Log($"\n--- {characterData.Key} ---");
            foreach (KeyValuePair<Stats, int> upgrade in characterData.Value)
            {
                if (upgrade.Value > 0)
                {
                    float bonus = GetStatBonusForCharacter(characterData.Key, upgrade.Key);
                    Debug.Log($"{upgrade.Key}: Level {upgrade.Value} (+{bonus})");
                }
            }
        }
    }

    #endregion
}
