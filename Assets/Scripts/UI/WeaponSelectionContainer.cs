using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class WeaponSelectionContainer : MonoBehaviour
{
    
    [Header("UI")]
    [SerializeField] private Image _upgradeIcon;
    [SerializeField] private TextMeshProUGUI _upgradeNameText;

    
    [Header("Stats")]
    [SerializeField] private Transform _statContainerParent;

    [Header("Color")]
    [SerializeField] private Image[] _levelDependedImage;


    [field: SerializeField] public Button Button { get; private set; }

    public void Configure(WeaponDataSO weaponData, int level)
    {

        _upgradeIcon.sprite = weaponData.WeaponSprite;
        _upgradeNameText.text = weaponData.WeaponName + $" (Lv.{level + 1})";

        Color imageColor = ColorHolder.GetColor(level);
        _upgradeNameText.color = imageColor;

        foreach (Image image in _levelDependedImage)
        {
            image.color = imageColor;
        }
        
        Dictionary<Stats, float> calculatedStats = WeaponStatsCalculator.GetStats(weaponData, level);
        ConfigureStatsContainer(calculatedStats);
    }

    private void ConfigureStatsContainer(Dictionary<Stats, float> calculatedStats)
    {
        StatContainerManager.GenerateStatsContainer(calculatedStats, _statContainerParent);
    }

    public Button GetButton()
    {
        return Button;
    }

    public void Select()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.2f).setEase(LeanTweenType.easeOutBack);
    }

    public void Deselect()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one, 0.2f);
    }

}
