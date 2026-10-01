using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private GameObject _priceContainer;
    [SerializeField] private Transform _statsParent;

    [field: SerializeField] public Button BuyButton { get; private set; }
    
    private CharterDataSO _currentCharterData;
    private bool _isUnlocked;

    private void OnEnable()
    {
        // Subscribe to upgrade changes to refresh display
        MetaUpgradeManager.OnUpgradesChanged += RefreshDisplay;
    }
    
    private void OnDisable()
    {
        MetaUpgradeManager.OnUpgradesChanged -= RefreshDisplay;
    }

    public void Configure(CharterDataSO charterData, bool isUnlocked)
    {
        _currentCharterData = charterData;
        _isUnlocked = isUnlocked;

        _nameText.text = charterData.CharterName;
        _priceText.text = charterData.CharterPrice.ToString();

        _priceContainer.SetActive(!isUnlocked);

        // Get stats with meta upgrade bonuses included
        Dictionary<Stats, float> statsWithUpgrades = GetStatsWithMetaUpgrades(charterData);
        StatContainerManager.GenerateStatsContainer(statsWithUpgrades, _statsParent);
    }
    
    /// <summary>
    /// Refresh the display when upgrades change
    /// </summary>
    private void RefreshDisplay()
    {
        if (_currentCharterData != null)
        {
            Configure(_currentCharterData, _isUnlocked);
        }
    }
    
    /// <summary>
    /// Get character stats with meta upgrade bonuses applied
    /// </summary>
    private Dictionary<Stats, float> GetStatsWithMetaUpgrades(CharterDataSO charterData)
    {
        Dictionary<Stats, float> result = new Dictionary<Stats, float>();
        
        // Start with base stats (only non-neutral ones for display)
        foreach (KeyValuePair<Stats, float> kvp in charterData.NonNeutralStats)
        {
            float baseValue = kvp.Value;
            float upgradeBonus = 0f;
            
            // Add meta upgrade bonus if available
            if (MetaUpgradeManager.instance != null)
            {
                upgradeBonus = MetaUpgradeManager.instance.GetStatBonusForCharacter(charterData.CharterName, kvp.Key);
            }
            
            result[kvp.Key] = baseValue + upgradeBonus;
        }
        
        // Also check for stats that have upgrades but might be 0 in base stats
        if (MetaUpgradeManager.instance != null)
        {
            Dictionary<Stats, float> allBaseStats = charterData.BaseStats;
            foreach (KeyValuePair<Stats, float> kvp in allBaseStats)
            {
                if (!result.ContainsKey(kvp.Key))
                {
                    float upgradeBonus = MetaUpgradeManager.instance.GetStatBonusForCharacter(charterData.CharterName, kvp.Key);
                    if (upgradeBonus > 0)
                    {
                        // This stat has upgrades even though base is 0
                        result[kvp.Key] = kvp.Value + upgradeBonus;
                    }
                }
            }
        }
        
        return result;
    }
}

