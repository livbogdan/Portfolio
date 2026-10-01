using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemContainer : MonoBehaviour
{
    [SerializeField] private Image _container;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;

    public int Index                    { get; private set; }
    public Weapon Weapon                { get; private set; }
    public ObjectDataSO ObjectDataSO    { get; private set; }


    public void Configure(Color containerColor, Sprite sprite)
    {
        _container.color = containerColor;
        _icon.sprite = sprite;
    }

    public void Configure(Weapon weapon, int index, Action clickedCallback)
    {
        Weapon = weapon;
        Index = index;

        Color color = ColorHolder.GetColor(weapon.Level);
        Sprite icon = weapon.WeaponData.WeaponSprite;

        Configure(color, icon);

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => clickedCallback?.Invoke());
    }
    public void Configure(ObjectDataSO objectDataSO, Action clickedCallback)
    {
        ObjectDataSO = objectDataSO;

        Color color = ColorHolder.GetColor(objectDataSO.Rarity);
        Sprite icon = objectDataSO.Icon;

        Configure(color, icon);

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => clickedCallback?.Invoke());
    }
}
