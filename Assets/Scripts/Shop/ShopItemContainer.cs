using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemContainer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("UI")]
    [SerializeField] private Image _upgradeIcon;
    [SerializeField] private TextMeshProUGUI _upgradeNameText;
    [SerializeField] private TextMeshProUGUI _priceText;


    [Header("Stats")]
    [SerializeField] private Transform _statContainerParent;

    [Header("Color")]
    [SerializeField] private Image[] _levelDependedImage;

    [Header("Lock Elements")]
    [SerializeField] private Image _lockButton;
    [SerializeField] private Sprite _lockSprite, _unlockSprite;
    public bool IsLocked { get; private set; }

    [Header("Purchase")]
    public WeaponDataSO WeaponData { get; private set; }
    public ObjectDataSO ObjectData { get; private set; }
    private int _weaponLevel;


    [Header("Actions")]
    public static Action<ShopItemContainer, int> OnPurchase;


    [SerializeField] private Button _purchaseButton;

    private void Awake()
    {
        CurrencyManager.OnCurrencyChanged += CurrencyUpdatedCallback;
    }
    private void OnDestroy()
    {
        CurrencyManager.OnCurrencyChanged -= CurrencyUpdatedCallback;
    }


    public void Configure(WeaponDataSO weaponData, int level)
    {
        _weaponLevel = level;
        WeaponData = weaponData;

        _upgradeIcon.sprite = weaponData.WeaponSprite;
        _upgradeNameText.text = weaponData.WeaponName + $" (Lv.{level + 1})";

        int weaponPrice = WeaponStatsCalculator.GetPurchasePrice(weaponData, level);

        _priceText.text = weaponPrice.ToString();

        Color imageColor = ColorHolder.GetColor(level);
        _upgradeNameText.color = imageColor;

        foreach (Image image in _levelDependedImage)
        {
            image.color = imageColor;
        }

        Dictionary<Stats, float> calculatedStats = WeaponStatsCalculator.GetStats(weaponData, level);

        ConfigureStatsContainer(calculatedStats);

        _purchaseButton.onClick.AddListener(Purchase);
        _purchaseButton.interactable = CurrencyManager.instance.HasEnoughtCurrency(weaponPrice);
    }


    public void Configure(ObjectDataSO objectData)
    {

        ObjectData = objectData;

        _upgradeIcon.sprite = objectData.Icon;
        _upgradeNameText.text = objectData.Name;
        _priceText.text = objectData.Price.ToString();


        Color imageColor = ColorHolder.GetColor(objectData.Rarity);
        _upgradeNameText.color = imageColor;

        foreach (Image image in _levelDependedImage)
        {
            image.color = imageColor;
        }

        ConfigureStatsContainer(objectData.BaseStats);

        _purchaseButton.onClick.AddListener(Purchase);
        _purchaseButton.interactable = CurrencyManager.instance.HasEnoughtCurrency(objectData.Price);
    }

    private void ConfigureStatsContainer(Dictionary<Stats, float> stats)
    {
        _statContainerParent.Clear();
        StatContainerManager.GenerateStatsContainer(stats, _statContainerParent);
    }

    public void LockButtonCallback()
    {
        IsLocked = !IsLocked;
        UpdateLockVisual();
    }
    private void UpdateLockVisual()
    {
        _lockButton.sprite = IsLocked ? _lockSprite : _unlockSprite;
    }
    private void Purchase()
    {
        OnPurchase?.Invoke(this, _weaponLevel);
    }
    private void CurrencyUpdatedCallback(int newAmount)
    {
        int itemPrice;

        if (WeaponData != null)
            itemPrice = WeaponStatsCalculator.GetPurchasePrice(WeaponData, _weaponLevel);
        else
            itemPrice = ObjectData.Price;

        _purchaseButton.interactable = CurrencyManager.instance.HasEnoughtCurrency(itemPrice);
    }

}
