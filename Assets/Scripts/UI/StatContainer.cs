using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatContainer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Image statIcon;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private Color _statValueColor = Color.white;

    public void Configure(Sprite icon, string statName, float statValue, bool useColor = false)
    {
        statIcon.sprite = icon;
        statText.text = statName;

        if (useColor)
            ColorizeStatValue(statValue);
        else
        {
            statValueText.color = _statValueColor;
            statValueText.text = statValue.ToString("F2");
        }

    }

    private void ColorizeStatValue(float statValue)
    {
        float sign = Mathf.Sign(statValue);

        if (statValue == 0)
            sign = 0;

        float absStatValue = Mathf.Abs(statValue);

        Color statValueColor = _statValueColor;

        if (sign > 0)
            statValueColor = Color.green;
        else if (sign < 0)
            statValueColor = Color.red;

        statValueText.color = statValueColor;
        statValueText.text = absStatValue.ToString("F2");
    }

    public float GetFontSize()
    {
        return statText.fontSize;
    }

    internal void SetFontSize(float fontSize)
    {
        statText.fontSizeMax = fontSize;
        statValueText.fontSizeMax = fontSize;
    }
}
