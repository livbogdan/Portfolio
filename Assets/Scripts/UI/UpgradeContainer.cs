using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeContainer : MonoBehaviour
{
    [SerializeField] private Image _upgradeIcon;
    [SerializeField] private TextMeshProUGUI _upgradeNameText;
    [SerializeField] private TextMeshProUGUI _upgradeValueText;
    
    [field: SerializeField] public Button _button { get; private set; }
    
    public void Configure(Sprite icon, string name, string value)
    {
        _upgradeIcon.sprite = icon;
        _upgradeNameText.text = name;
        _upgradeValueText.text = value;
    }

    public Button GetButton()
    {
        return _button;
    }
}
