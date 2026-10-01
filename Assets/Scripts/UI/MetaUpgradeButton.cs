using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI component for a single meta upgrade button in the upgrade shop
/// </summary>
public class MetaUpgradeButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _statNameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _bonusText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Button _purchaseButton;
    [SerializeField] private GameObject _maxLevelIndicator;

    [Header("Colors")]
    [SerializeField] private Color _affordableColor = Color.green;
    [SerializeField] private Color _notAffordableColor = Color.red;
    [SerializeField] private Color _maxLevelColor = Color.yellow;

    private Stats _stat;

    private void Awake()
    {
        if (_purchaseButton != null)
            _purchaseButton.onClick.AddListener(OnPurchaseClicked);

        // Subscribe to events
        CurrencyManager.OnCurrencyChanged += OnCurrencyChanged;
        MetaUpgradeManager.OnUpgradesChanged += UpdateDisplay;
    }

    private void OnDestroy()
    {
        if (_purchaseButton != null)
            _purchaseButton.onClick.RemoveListener(OnPurchaseClicked);

        CurrencyManager.OnCurrencyChanged -= OnCurrencyChanged;
        MetaUpgradeManager.OnUpgradesChanged -= UpdateDisplay;
    }

    /// <summary>
    /// Configure this button for a specific stat
    /// </summary>
    public void Configure(Stats stat)
    {
        _stat = stat;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (MetaUpgradeManager.instance == null)
            return;

        // Set stat name
        if (_statNameText != null)
            _statNameText.text = Enums.FormatStateName(_stat);

        // Set icon
        if (_iconImage != null)
            _iconImage.sprite = ResourceManager.GetStatsIcon(_stat);

        // Get upgrade info
        int currentLevel = MetaUpgradeManager.instance.GetUpgradeLevel(_stat);
        int maxLevel = MetaUpgradeManager.instance.GetMaxLevel(_stat);
        float currentBonus = MetaUpgradeManager.instance.GetStatBonus(_stat);
        int cost = MetaUpgradeManager.instance.GetUpgradeCost(_stat);
        bool isMaxLevel = MetaUpgradeManager.instance.IsMaxLevel(_stat);

        // Update level text
        if (_levelText != null)
            _levelText.text = $"Lv. {currentLevel}/{maxLevel}";

        // Update bonus text
        if (_bonusText != null)
        {
            if (currentLevel > 0)
                _bonusText.text = $"+{currentBonus:F1} {_stat}";
            else
                _bonusText.text = "Not Upgraded";
        }

        // Update cost text and button state
        if (isMaxLevel)
        {
            if (_costText != null)
                _costText.text = "MAX";

            if (_purchaseButton != null)
            {
                _purchaseButton.interactable = false;
                _purchaseButton.GetComponent<Image>().color = _maxLevelColor;
            }

            if (_maxLevelIndicator != null)
                _maxLevelIndicator.SetActive(true);
        }
        else
        {
            if (_costText != null)
                _costText.text = $"{cost} Gems";

            bool canAfford = MetaUpgradeManager.instance.CanAffordUpgrade(_stat);

            if (_purchaseButton != null)
            {
                _purchaseButton.interactable = canAfford;
                _purchaseButton.GetComponent<Image>().color = canAfford ? _affordableColor : _notAffordableColor;
            }

            if (_maxLevelIndicator != null)
                _maxLevelIndicator.SetActive(false);
        }
    }

    private void OnPurchaseClicked()
    {
        if (MetaUpgradeManager.instance != null)
        {
            bool success = MetaUpgradeManager.instance.PurchaseUpgrade(_stat);
            
            if (success)
            {
                // Play purchase sound/animation here if desired
                Debug.Log($"Successfully purchased {_stat} upgrade!");
            }
        }
    }

    private void OnCurrencyChanged(int newAmount)
    {
        UpdateDisplay();
    }
}
