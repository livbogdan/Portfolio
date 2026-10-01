using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemInfo : MonoBehaviour
{
    [Header("Item Info")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _recyclePriceText;

    [Header("Item Stats")]
    [SerializeField] private Image _container;
    [SerializeField] private Transform _statsParent;

    [Header("Buttons")]
    [field: SerializeField] public Button RecycleButton { get; private set; }
    [SerializeField] private Button _mergeButton;

    public void Configure(Weapon weapon)
    {
        Configure(
            weapon.WeaponData.WeaponSprite,
            weapon.WeaponData.WeaponName + " (Level " + (weapon.Level + 1) + ")",
            ColorHolder.GetColor(weapon.Level),
            WeaponStatsCalculator.GetRecyclePrice(weapon.WeaponData, weapon.Level),
            WeaponStatsCalculator.GetStats(weapon.WeaponData, weapon.Level)
        );

        _mergeButton.gameObject.SetActive(true);

        _mergeButton.interactable = WeaponMerger.instance.CanMerge(weapon);

        _mergeButton.onClick.RemoveAllListeners();
        _mergeButton.onClick.AddListener(WeaponMerger.instance.Merge);

    }

    public void Configure(ObjectDataSO objectDataSO)
    {
        Configure(
            objectDataSO.Icon,
            objectDataSO.Name,
            ColorHolder.GetColor(objectDataSO.Rarity),
            objectDataSO.RecyclePrice,
            objectDataSO.BaseStats
        );

        _mergeButton.gameObject.SetActive(false);
    }

    private void Configure(
        Sprite icon,
        string itemName,
        Color containerColor,
        int recyclePrice,
        Dictionary<Stats, float> stats)
    {
        _icon.sprite = icon;
        _itemNameText.text = itemName;
        _itemNameText.color = containerColor;

        _recyclePriceText.text = recyclePrice.ToString();

        _container.color = containerColor;

        StatContainerManager.GenerateStatsContainer(stats, _statsParent);
    }
}
