using UnityEngine;
using NaughtyAttributes;

public class ShopManagerUI : MonoBehaviour
{

    [SerializeField] private GameObject _playerStatsPanel;
    [SerializeField] private GameObject _inventoryStatsPanel;

    [Header("Item Info Panel")]
    [SerializeField]private RectTransform _itemInfoSlidePanel;
    [SerializeField]private Vector2 itemInfoOpenPosition;
    [SerializeField] private Vector2 itemInfoHidePosition;



    public void TogglePlayerStats()
    {
        _playerStatsPanel.SetActive(!_playerStatsPanel.activeSelf);
    }
    public void ToggleInventoryStats()
    {
        _inventoryStatsPanel.SetActive(!_inventoryStatsPanel.activeSelf);
    }

    [Button]
    public void OpenItemInfoStats()
    {
        _itemInfoSlidePanel.gameObject.SetActive(true);

        _itemInfoSlidePanel.LeanCancel();
        _itemInfoSlidePanel.LeanMove((Vector3)itemInfoOpenPosition, 0.3f)
            .setEase(LeanTweenType.easeInCubic);
    }

    [Button]
    public void HideItemInfoStats()
    {
        //_itemInfoSlidePanel.SetActive(!_itemInfoSlidePanel.activeSelf);

        _itemInfoSlidePanel.LeanCancel();
        _itemInfoSlidePanel.LeanMove((Vector3)itemInfoHidePosition, 0.3f)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(() => _itemInfoSlidePanel.gameObject.SetActive(false));

    }

}
