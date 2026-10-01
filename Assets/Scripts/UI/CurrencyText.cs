using UnityEngine;
using TMPro;


[RequireComponent(typeof(TextMeshProUGUI))]
public class CurrencyText : MonoBehaviour
{
    private TextMeshProUGUI _currencyText;

    public void UppdateText(int currency)
    {
        if (_currencyText == null)
            _currencyText = GetComponent<TextMeshProUGUI>();

        _currencyText.text = currency.ToString();
    }
}
