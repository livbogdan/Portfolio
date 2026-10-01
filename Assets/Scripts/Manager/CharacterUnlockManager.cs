using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterUnlockRule
{
    [Header("Character Info")]
    public CharterDataSO targetCharacter; // Direct reference to character to unlock
    [Tooltip("If no specific character is set, will unlock any character of this rarity")]
    public int fallbackRarity = 4; // Fallback rarity if no specific character is set
    
    [Header("Unlock Conditions (Any ONE condition can unlock)")]
    public bool useWaveCompletion;
    public int requiredWaves = 15;
    
    public bool useBossKills;
    public int requiredBossKills = 5;
    
    public bool useEndlessWave;
    public int requiredEndlessWave = 20;
    
    public bool useTotalEnemyKills;
    public int requiredEnemyKills = 100;
    
    [Header("Multiple Conditions (ALL must be met)")]
    public bool requireMultipleConditions = false;
    
    // Track if this rule has been triggered
    [NonSerialized] public bool hasBeenUnlocked = false;
}

public enum UnlockType
{
    WaveCompletion,
    BossKills,
    EndlessWaveReached,
    TotalEnemyKills
}

public class CharacterUnlockManager : MonoBehaviour
{
    [Header("Character Unlock Rules")]
    [SerializeField] private List<CharacterUnlockRule> _unlockRules = new List<CharacterUnlockRule>();
    
    // Events
    public static Action<string, int> OnCharacterUnlocked;
    
    // Tracking data
    private int _wavesCompleted;
    private int _bossesKilled;
    private int _highestEndlessWave;
    private int _totalEnemyKills;
    
    // Save keys
    private const string WAVES_COMPLETED_KEY = "wavesCompleted";
    private const string BOSSES_KILLED_KEY = "bossesKilled";
    private const string HIGHEST_ENDLESS_WAVE_KEY = "highestEndlessWave";
    private const string TOTAL_ENEMY_KILLS_KEY = "totalEnemyKills";
    
    private void Awake()
    {
        // Subscribe to game events
        GameManager.OnWaveCompleted += OnWaveCompleted;
        GameManager.OnBossKilled += OnBossKilled;
        GameManager.OnEnemyKilled += OnEnemyKilled;
        
        SetupDefaultUnlockConditions();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        GameManager.OnWaveCompleted -= OnWaveCompleted;
        GameManager.OnBossKilled -= OnBossKilled;
        GameManager.OnEnemyKilled -= OnEnemyKilled;
    }
    
    private void SetupDefaultUnlockConditions()
    {
        if (_unlockRules.Count == 0)
        {
            // Example unlock rules - you can customize these in inspector

        }
    }
    
    private void OnWaveCompleted(int waveNumber)
    {
        _wavesCompleted++;
        CheckUnlockConditions();
    }
    
    private void OnBossKilled()
    {
        _bossesKilled++;
        CheckUnlockConditions();
    }
    
    private void OnEnemyKilled(string enemyType)
    {
        _totalEnemyKills++;
        CheckUnlockConditions();
    }
    
    private void CheckUnlockConditions()
    {
        // Check old-style rules first
        foreach (var rule in _unlockRules)
        {
            if (!rule.hasBeenUnlocked && IsRuleMet(rule))
            {
                UnlockCharacterByRule(rule);
                rule.hasBeenUnlocked = true;
            }
        }
        
        // Check character-specific rules
        CharterDataSO[] allCharacters = ResourceManager.Character;
        for (int i = 0; i < allCharacters.Length; i++)
        {
            var character = allCharacters[i];
            if (character.ProgressionOnlyUnlock && character.UnlockRule != null && 
                !character.UnlockRule.hasBeenUnlocked && !IsCharacterUnlocked(i))
            {
                if (IsRuleMet(character.UnlockRule))
                {
                    UnlockCharacter(i, character.CharterName);
                    character.UnlockRule.hasBeenUnlocked = true;
                }
            }
        }
    }
    
    private bool IsRuleMet(CharacterUnlockRule rule)
    {
        List<bool> conditions = new List<bool>();
        
        // Check each enabled condition
        if (rule.useWaveCompletion)
            conditions.Add(_wavesCompleted >= rule.requiredWaves);
            
        if (rule.useBossKills)
            conditions.Add(_bossesKilled >= rule.requiredBossKills);
            
        if (rule.useEndlessWave)
            conditions.Add(_highestEndlessWave >= rule.requiredEndlessWave);
            
        if (rule.useTotalEnemyKills)
            conditions.Add(_totalEnemyKills >= rule.requiredEnemyKills);
        
        // If no conditions are enabled, return false
        if (conditions.Count == 0)
            return false;
        
        // Check if conditions are met based on requirement type
        if (rule.requireMultipleConditions)
        {
            // ALL conditions must be true
            return conditions.TrueForAll(c => c);
        }
        else
        {
            // ANY condition can be true
            return conditions.Exists(c => c);
        }
    }
    
    private bool IsRuleMet(CharacterUnlockRuleSO rule)
    {
        List<bool> conditions = new List<bool>();
        
        // Check each enabled condition
        if (rule.useWaveCompletion)
            conditions.Add(_wavesCompleted >= rule.requiredWaves);
            
        if (rule.useBossKills)
            conditions.Add(_bossesKilled >= rule.requiredBossKills);
            
        if (rule.useEndlessWave)
            conditions.Add(_highestEndlessWave >= rule.requiredEndlessWave);
            
        if (rule.useTotalEnemyKills)
            conditions.Add(_totalEnemyKills >= rule.requiredEnemyKills);
        
        // If no conditions are enabled, return false
        if (conditions.Count == 0)
            return false;
        
        // Check if conditions are met based on requirement type
        if (rule.requireMultipleConditions)
        {
            // ALL conditions must be true
            return conditions.TrueForAll(c => c);
        }
        else
        {
            // ANY condition can be true
            return conditions.Exists(c => c);
        }
    }
    
    private void UnlockCharacterByRule(CharacterUnlockRule rule)
    {
        CharterDataSO[] allCharacters = ResourceManager.Character;
        
        // If specific character is set, try to find and unlock it
        if (rule.targetCharacter != null)
        {
            for (int i = 0; i < allCharacters.Length; i++)
            {
                if (allCharacters[i] == rule.targetCharacter)
                {
                    // Check if already unlocked
                    if (IsCharacterUnlocked(i))
                    {
                        Debug.Log($"Character {rule.targetCharacter.CharterName} is already unlocked");
                        return;
                    }
                    
                    // Unlock the specific character
                    UnlockCharacter(i, allCharacters[i].CharterName);
                    Debug.Log($"Unlocked {allCharacters[i].CharterName} (Rarity {allCharacters[i].Rarity}) via rule: {GetRuleDescription(rule)}");
                    return;
                }
            }
            
            Debug.LogWarning($"Target character {rule.targetCharacter.CharterName} not found in character array");
            return;
        }
        
        // If no specific character set, unlock any character of the fallback rarity
        for (int i = 0; i < allCharacters.Length; i++)
        {
            if (allCharacters[i].Rarity == rule.fallbackRarity && !IsCharacterUnlocked(i))
            {
                UnlockCharacter(i, allCharacters[i].CharterName);
                Debug.Log($"Unlocked {allCharacters[i].CharterName} (Rarity {rule.fallbackRarity}) via rule: {GetRuleDescription(rule)}");
                return;
            }
        }
        
        Debug.LogWarning($"No unlockable character found for rarity {rule.fallbackRarity}");
    }
    
    private string GetRuleDescription(CharacterUnlockRule rule)
    {
        List<string> conditions = new List<string>();
        
        if (rule.useWaveCompletion)
            conditions.Add($"Complete {rule.requiredWaves} waves");
        if (rule.useBossKills)
            conditions.Add($"Kill {rule.requiredBossKills} bosses");
        if (rule.useEndlessWave)
            conditions.Add($"Reach wave {rule.requiredEndlessWave} in endless");
        if (rule.useTotalEnemyKills)
            conditions.Add($"Kill {rule.requiredEnemyKills} enemies");
            
        string connector = rule.requireMultipleConditions ? " AND " : " OR ";
        return string.Join(connector, conditions);
    }
    
    private void UnlockCharacter(int characterIndex, string characterName)
    {
        // Get the character selection manager to unlock the character
        CharacterSelectionManager characterManager = FindFirstObjectByType<CharacterSelectionManager>();
        if (characterManager != null)
        {
            characterManager.UnlockCharacterByProgression(characterIndex);
        }
        
        // Get character data for rarity
        CharterDataSO[] allCharacters = ResourceManager.Character;
        int rarity = allCharacters.Length > characterIndex ? allCharacters[characterIndex].Rarity : 0;
        
        // Trigger unlock notification
        OnCharacterUnlocked?.Invoke(characterName, rarity);
        
        Debug.Log($"Character unlocked: {characterName}!");
    }
    
    private bool IsCharacterUnlocked(int characterIndex)
    {
        List<bool> unlockedStates;
        if (ES3SaveManager.Instance != null)
            unlockedStates = ES3SaveManager.Instance.Load(ES3SaveManager.KEY_UNLOCKED_CHARACTERS, new List<bool>());
        else
            unlockedStates = ES3.Load(ES3SaveManager.KEY_UNLOCKED_CHARACTERS, new List<bool>());
        
        return characterIndex < unlockedStates.Count && unlockedStates[characterIndex];
        
    }
    
    public void UpdateEndlessWaveProgress(int currentWave)
    {
        if (currentWave > _highestEndlessWave)
        {
            _highestEndlessWave = currentWave;

            CheckUnlockConditions();
        }
    }
    

    
    // Public getters for UI display
    public int GetWavesCompleted() => _wavesCompleted;
    public int GetBossesKilled() => _bossesKilled;
    public int GetHighestEndlessWave() => _highestEndlessWave;
    public int GetTotalEnemyKills() => _totalEnemyKills;
    public List<CharacterUnlockRule> GetUnlockRules() => _unlockRules;
}
