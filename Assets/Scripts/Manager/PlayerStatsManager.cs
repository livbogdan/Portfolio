using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{

    [Header("Data")]
    [SerializeField] private CharterDataSO _charterData;

    [Header("Player Stats")]
    private Dictionary<Stats, float> _addends = new Dictionary<Stats, float>();
    private Dictionary<Stats, float> _playerStats = new Dictionary<Stats, float>();
    private Dictionary<Stats, float> _objectStats = new Dictionary<Stats, float>();

    private void Awake()
    {
        CharacterSelectionManager._onCharacterSelected += CharacterSelectedCallback;
    }

    private void OnDestroy() => CharacterSelectionManager._onCharacterSelected -= CharacterSelectedCallback;

    public void AddPlayerStat(Stats stat, float value)
    {
        if (_addends.ContainsKey(stat))
        {
            _addends[stat] += value;
        }
        else
        {
            // Initialize the stat if it doesn't exist (handles meta upgrades for stats not in base)
            _addends[stat] = value;
            
            // Also initialize in other dictionaries if needed
            if (!_objectStats.ContainsKey(stat))
            {
                _objectStats[stat] = 0;
            }
            
            Debug.Log($"Initialized missing stat {stat} with value {value}");
        }

        UpdatePlayerStats();
        // NOTE: Run stats are not saved - they reset each run
    }

    public void AddObject(Dictionary<Stats, float> objectStats)
    {
        foreach (KeyValuePair<Stats, float> statKVP in objectStats)
        {
            _objectStats[statKVP.Key] += statKVP.Value;
        }

        UpdatePlayerStats();
    }
    public void RemoveObjectStats(Dictionary<Stats, float> baseStats)
    {
        foreach (KeyValuePair<Stats, float> statKVP in baseStats)
        {
            _objectStats[statKVP.Key] -= statKVP.Value;
        }

        UpdatePlayerStats();
    }
    public float GetStatsValue(Stats stat)
    { 
        if (_playerStats == null || !_playerStats.ContainsKey(stat))
        {
            Debug.LogWarning($"PlayerStatsManager not initialized or stat '{stat}' not found. Returning 0.");
            return 0f;
        }
        
        float baseValue = _playerStats[stat];
        float addendValue = _addends.ContainsKey(stat) ? _addends[stat] : 0f;
        float objectValue = _objectStats.ContainsKey(stat) ? _objectStats[stat] : 0f;
        
        return baseValue + addendValue + objectValue;
    }

    private void UpdatePlayerStats()
    {
        IEnumerable<IPlayerStatsDependency> playerStatsDependencies =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IPlayerStatsDependency>();

        foreach (IPlayerStatsDependency dependency in playerStatsDependencies)
            dependency.UpdateStats(this);
    }

    private void CharacterSelectedCallback(CharterDataSO charterData)
    {
        _charterData = charterData;
        _playerStats = _charterData.BaseStats;

        // Initialize dictionaries
        _addends.Clear();
        _objectStats.Clear();
        
        foreach (KeyValuePair<Stats, float> kvp in _playerStats)
        {
            _addends.Add(kvp.Key, 0);
            _objectStats.Add(kvp.Key, 0);
        }


        
        // Apply meta upgrades from the Meta Upgrade Shop
        ApplyMetaUpgrades();

        UpdatePlayerStats();
    }
    
    /// <summary>
    /// Apply permanent stat bonuses from the Meta Upgrade system
    /// </summary>
    private void ApplyMetaUpgrades()
    {
        if (MetaUpgradeManager.instance != null && _charterData != null)
        {
            MetaUpgradeManager.instance.ApplyMetaUpgrades(this, _charterData.CharterName);
        }
        else
        {
            if (MetaUpgradeManager.instance == null)
                Debug.LogWarning("MetaUpgradeManager not found. Meta upgrades will not be applied.");
            if (_charterData == null)
                Debug.LogWarning("CharterData not assigned. Meta upgrades will not be applied.");
        }
    }
}
