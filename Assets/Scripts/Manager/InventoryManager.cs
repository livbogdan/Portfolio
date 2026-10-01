using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameStateListner
{
    [Header("Player Components")]
    [SerializeField] private PlayerWeapon _playerWeapon;
    [SerializeField] private PlayerObject _playerObject;

    [Header("Inventory")]
    [SerializeField] private Transform _inventoryItemsParent;
    [SerializeField] private Transform _pauseInventoryItemsParent;
    [SerializeField] private InventoryItemContainer _inventoryItemContainer;
    [SerializeField] private ShopManagerUI _shopManagerUI;
    [SerializeField] private InventoryItemInfo _itemInfo;

    private void Awake()
    {
        ShopManager._onItemPurchased += ItemPurchasedCallback;
        WeaponMerger._onMerge += WeaponMergeCallback;

        GameManager._onGamePaused += Configure;
    }
    private void OnDestroy()
    {
        ShopManager._onItemPurchased -= ItemPurchasedCallback;
        WeaponMerger._onMerge -= WeaponMergeCallback;
        GameManager._onGamePaused -= Configure;
    }


    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.SHOP)
            Configure();
    }

    private void Configure()
    {
        _inventoryItemsParent.Clear();
        _pauseInventoryItemsParent.Clear();

        Weapon[] weapons = _playerWeapon.GetWeaponList();

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
                continue;

            InventoryItemContainer container = Instantiate(_inventoryItemContainer, _inventoryItemsParent);
            container.Configure(weapons[i], i, () => ShowItemInfo(container));

            InventoryItemContainer pauseContainer = Instantiate(_inventoryItemContainer, _pauseInventoryItemsParent);
            pauseContainer.Configure(weapons[i], i, () => ShowItemInfo(container));
        }

        ObjectDataSO[] objectsData = _playerObject.PlayerObjects.ToArray();

        for (int i = 0; i < objectsData.Length; i++)
        {
            InventoryItemContainer container = Instantiate(_inventoryItemContainer, _inventoryItemsParent);
            container.Configure(objectsData[i], () => ShowItemInfo(container));

            InventoryItemContainer pauseContainer = Instantiate(_inventoryItemContainer, _pauseInventoryItemsParent);
            pauseContainer.Configure(objectsData[i], () => ShowItemInfo(container));
        }
    }

    private void ShowItemInfo(InventoryItemContainer container)
    {
        if (container.Weapon != null)
            ShowWeaponInfo(container.Weapon, container.Index);
        else
            ShowObjectInfo(container.ObjectDataSO);

    }

    private void ShowWeaponInfo(Weapon weapon, int index)
    {
        _itemInfo.Configure(weapon);

        _itemInfo.RecycleButton.onClick.RemoveAllListeners();
        _itemInfo.RecycleButton.onClick.AddListener(() => RecycleWeapon(index));

        _shopManagerUI.OpenItemInfoStats();
    }

    private void ShowObjectInfo(ObjectDataSO objectDataSO)
    {
        _itemInfo.Configure(objectDataSO);

        _itemInfo.RecycleButton.onClick.RemoveAllListeners();
        _itemInfo.RecycleButton.onClick.AddListener(() => RecycleObject(objectDataSO));

        _shopManagerUI.OpenItemInfoStats();
    }

    private void RecycleObject(ObjectDataSO objectDataToRecycle)
    {
        _playerObject.RecycleObject(objectDataToRecycle);
        Configure();
        _shopManagerUI.HideItemInfoStats();
    }

    private void RecycleWeapon(int index)
    {
        _playerWeapon.RecycleWeapon(index);
        Configure();
        _shopManagerUI.HideItemInfoStats();
    }

    private void ItemPurchasedCallback() => Configure();
    private void WeaponMergeCallback(Weapon weapon)
    {
        Configure();
        _itemInfo.Configure(weapon);
    }
}
