using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the Meta Upgrade Shop UI panel
/// This shop allows players to spend gems on permanent stat boosts
/// </summary>
public class MetaUpgradeShopUI : MonoBehaviour, IGameStateListner
{
    [Header("References")]
    [SerializeField] private Transform _upgradeButtonContainer;
    [SerializeField] private MetaUpgradeButton _upgradeButtonPrefab;
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private TMPro.TextMeshProUGUI _characterNameText;

    [Header("Stats to Display")]
    [SerializeField] private List<Stats> _upgradeableStats = new List<Stats>
    {
        Stats.MaxHealth,
        Stats.Attack,
        Stats.AttackSpeed,
        Stats.CriticalChance,
        Stats.MoveSpeed,
        Stats.Armor,
        Stats.Dodge,
        Stats.LifeSteal
    };

    private List<MetaUpgradeButton> _spawnedButtons = new List<MetaUpgradeButton>();

    private void Start()
    {
        SetupUpgradeButtons();
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.UPGRADES)
        {
            UpdateCharacterName();
            SetupUpgradeButtons();
        }
    }

    private void UpdateCharacterName()
    {
        if (_characterNameText != null && MetaUpgradeManager.instance != null)
        {
            string characterName = MetaUpgradeManager.instance.GetCurrentCharacterName();
            if (!string.IsNullOrEmpty(characterName))
            {
                _characterNameText.text = $"{characterName}";
            }
            else
            {
                _characterNameText.text = "Character Upgrades";
            }
        }
    }

    private void SetupUpgradeButtons()
    {
        // Clear any existing buttons
        foreach (MetaUpgradeButton button in _spawnedButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        _spawnedButtons.Clear();

        // Get upgradeable stats from character's config
        List<Stats> statsToDisplay = GetUpgradeableStatsForCurrentCharacter();
        
        // Create new buttons
        foreach (Stats stat in statsToDisplay)
        {
            MetaUpgradeButton button = Instantiate(_upgradeButtonPrefab, _upgradeButtonContainer);
            button.Configure(stat);
            _spawnedButtons.Add(button);
        }
    }
    
    private List<Stats> GetUpgradeableStatsForCurrentCharacter()
    {
        // Try to get stats from character's config
        if (MetaUpgradeManager.instance != null)
        {
            string characterName = MetaUpgradeManager.instance.GetCurrentCharacterName();
            if (!string.IsNullOrEmpty(characterName))
            {
                CharterDataSO[] allCharacters = ResourceManager.Character;
                if (allCharacters != null)
                {
                    CharterDataSO characterData = System.Array.Find(allCharacters, c => c.CharterName == characterName);
                    if (characterData != null && characterData.UpgradeConfig != null)
                    {
                        return characterData.UpgradeConfig.GetEnabledStats();
                    }
                }
            }
        }
        
        // Fallback to default list
        return _upgradeableStats;
    }

}
