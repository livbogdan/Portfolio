using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestObjectContainer : MonoBehaviour
{
[Header("UI")]
    [SerializeField] private Image _upgradeIcon;
    [SerializeField] private TextMeshProUGUI _upgradeNameText;

    
    [Header("Stats")]
    [SerializeField] private Transform _statContainerParent;
    [field: SerializeField] public Button TakeButton { get; private set; }
    [field: SerializeField] public Button RecycleButton { get; private set; }
    [SerializeField] private TextMeshProUGUI _recycleTextPrice;

    [Header("Color")]
    [SerializeField] private Image[] _levelDependedImage;



    public void Configure(ObjectDataSO objectData)
    {

        _upgradeIcon.sprite = objectData.Icon;
        _upgradeNameText.text = objectData.Name;
        _recycleTextPrice.text = objectData.RecyclePrice.ToString();

        Color imageColor = ColorHolder.GetColor(objectData.Rarity);
        _upgradeNameText.color = imageColor;

        foreach (Image image in _levelDependedImage)
        {
            image.color = imageColor;
        }
        
        ConfigureStatsContainer(objectData.BaseStats);
    }

    private void ConfigureStatsContainer(Dictionary<Stats, float> stats)
    {
        StatContainerManager.GenerateStatsContainer(stats, _statContainerParent);
    }

    
}
