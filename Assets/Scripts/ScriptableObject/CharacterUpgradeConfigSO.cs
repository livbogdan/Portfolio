using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Defines which stats can be upgraded for a specific character,
/// with individual max levels and base costs per stat.
/// </summary>
[CreateAssetMenu(fileName = "Character Upgrade Config", menuName = "Scriptable Objects/Character Upgrade Config", order = 1)]
public class CharacterUpgradeConfigSO : ScriptableObject
{
    [Serializable]
    public class StatUpgradeConfig
    {
        [Tooltip("The stat that can be upgraded")]
        public Stats stat;
        
        [Tooltip("Maximum level for this stat upgrade")]
        [Range(1, 20)]
        public int maxLevel = 10;
        
        [Tooltip("Base cost for first upgrade (cost doubles each level)")]
        [Min(1)]
        public int baseCost = 10;
        
        [Tooltip("Bonus value per level")]
        [Min(0.1f)]
        public float bonusPerLevel = 5f;
        
        [Tooltip("Is this stat enabled for this character?")]
        public bool isEnabled = true;
    }
    
    [Header("Character Reference")]
    [Tooltip("The character this upgrade config belongs to")]
    public CharterDataSO character;
    
    [Header("Upgrade Configuration")]
    [Tooltip("List of upgradeable stats with their configurations")]
    public List<StatUpgradeConfig> upgradeableStats = new List<StatUpgradeConfig>
    {
        new StatUpgradeConfig { stat = Stats.MaxHealth, maxLevel = 10, baseCost = 10, bonusPerLevel = 5f, isEnabled = true },
        new StatUpgradeConfig { stat = Stats.Attack, maxLevel = 10, baseCost = 10, bonusPerLevel = 2f, isEnabled = true },
        new StatUpgradeConfig { stat = Stats.AttackSpeed, maxLevel = 10, baseCost = 15, bonusPerLevel = 3f, isEnabled = true },
        new StatUpgradeConfig { stat = Stats.CriticalChance, maxLevel = 10, baseCost = 20, bonusPerLevel = 2f, isEnabled = true },
        new StatUpgradeConfig { stat = Stats.MoveSpeed, maxLevel = 10, baseCost = 15, bonusPerLevel = 3f, isEnabled = true },
        new StatUpgradeConfig { stat = Stats.Armor, maxLevel = 10, baseCost = 12, bonusPerLevel = 2f, isEnabled = true },
        new StatUpgradeConfig { stat = Stats.Dodge, maxLevel = 10, baseCost = 20, bonusPerLevel = 2f, isEnabled = true },
        new StatUpgradeConfig { stat = Stats.LifeSteal, maxLevel = 10, baseCost = 25, bonusPerLevel = 2f, isEnabled = true }
    };
    
    /// <summary>
    /// Get config for a specific stat
    /// </summary>
    public StatUpgradeConfig GetStatConfig(Stats stat)
    {
        return upgradeableStats.Find(s => s.stat == stat && s.isEnabled);
    }
    
    /// <summary>
    /// Check if a stat is upgradeable for this character
    /// </summary>
    public bool IsStatUpgradeable(Stats stat)
    {
        StatUpgradeConfig config = GetStatConfig(stat);
        return config != null && config.isEnabled;
    }
    
    /// <summary>
    /// Get all enabled stats for this character
    /// </summary>
    public List<Stats> GetEnabledStats()
    {
        List<Stats> enabledStats = new List<Stats>();
        foreach (StatUpgradeConfig config in upgradeableStats)
        {
            if (config.isEnabled)
                enabledStats.Add(config.stat);
        }
        return enabledStats;
    }

}
