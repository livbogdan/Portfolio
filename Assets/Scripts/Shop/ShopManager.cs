using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour, IGameStateListner
{
    [Header("Elements")]
    [SerializeField] private Transform _containerParents;
    [SerializeField] private ShopItemContainer _shopItemContainerParent;
    [SerializeField] private int _containerToSpawn = 6;

    [Header("Player Components")]
    [SerializeField] private PlayerWeapon _playerWeapons;
    [SerializeField] private PlayerObject _playerObjects;

    [Header("Reroll")]
    [SerializeField] private Button _rerollButton;
    [SerializeField] private int _rerollPrice;
    [SerializeField] private TextMeshProUGUI _rerollPriceText;

    public static Action _onItemPurchased;

    private void Awake()
    {
        ShopItemContainer.OnPurchase += ItemPurchaseCallback;
        CurrencyManager.OnCurrencyChanged += CurrencyUpdatedCallback;
    }

    private void OnDestroy()
    {
        ShopItemContainer.OnPurchase -= ItemPurchaseCallback;
        CurrencyManager.OnCurrencyChanged -= CurrencyUpdatedCallback;
    }


    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.SHOP)
        {
            Configure();
            UppdateRerollVisual();
        }
    }

    private void Configure()
    {
        List<GameObject> toDestroy = new List<GameObject>();

        for (int i = 0; i < _containerParents.childCount; i++)
        {
            ShopItemContainer container = _containerParents.GetChild(i).GetComponent<ShopItemContainer>();

            if (!container.IsLocked)
                toDestroy.Add(container.gameObject);
        }

        while (toDestroy.Count > 0)
        {
            Transform t = toDestroy[0].transform;
            t.SetParent(null);
            Destroy(t.gameObject);
            toDestroy.RemoveAt(0);
        }

        int containerToSpawn = _containerToSpawn - _containerParents.childCount;
        int weaponConteinerCount = Random.Range(Mathf.Min(2, containerToSpawn), containerToSpawn);
        int objectContainerCount = containerToSpawn - weaponConteinerCount;

        for (int i = 0; i < weaponConteinerCount; i++)
        {
            ShopItemContainer weaponContainerInstance = Instantiate(_shopItemContainerParent, _containerParents);
            WeaponDataSO randomWeaponData = ResourceManager.GetRandomWeaponData();
            weaponContainerInstance.Configure(randomWeaponData, Random.Range(0, 2));
        }

        for (int i = 0; i < objectContainerCount; i++)
        {
            ShopItemContainer objectContainerInstance = Instantiate(_shopItemContainerParent, _containerParents);
            ObjectDataSO randomObjectData = ResourceManager.GetRandomObjectData();

            objectContainerInstance.Configure(randomObjectData);
        }

    }

    public void Reroll()
    {
        Configure();
        CurrencyManager.instance.UseCurrency(_rerollPrice);
    }

    private void UppdateRerollVisual()
    {
        _rerollPriceText.text = _rerollPrice.ToString();
        _rerollButton.interactable = CurrencyManager.instance.HasEnoughtCurrency(_rerollPrice);
    }

    public void CurrencyUpdatedCallback(int newAmount)
    {
        UppdateRerollVisual();
    }
    private void ItemPurchaseCallback(ShopItemContainer container, int weaponLevel)
    {
        if (container.WeaponData != null)
            TryPurchaseWeapon(container, weaponLevel);
        else
            PurchaseObjectData(container);
    }

    private void PurchaseObjectData(ShopItemContainer container)
    {
        _playerObjects.AddObject(container.ObjectData);

        CurrencyManager.instance.UseCurrency(container.ObjectData.Price);

        Destroy(container.gameObject);

        _onItemPurchased?.Invoke();
    }

    private void TryPurchaseWeapon(ShopItemContainer container, int weaponLevel)
    {
        if (_playerWeapons.TryAddWeapon(container.WeaponData, weaponLevel))
        {
            int price = WeaponStatsCalculator.GetPurchasePrice(container.WeaponData, weaponLevel);
            CurrencyManager.instance.UseCurrency(price);

            Destroy(container.gameObject);
        }

        _onItemPurchased?.Invoke();
    }
}
