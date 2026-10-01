using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Optional helper to expose a simple API for setting button title at runtime.
/// Attach to the left-side button template if you want. Not strictly required,
/// as TutorialUIController can set the TMP/Text directly, but handy for prefab reuse.
/// </summary>
[DisallowMultipleComponent]
public class TutorialButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _labelTMP;
    [SerializeField] private Text _labelLegacy;

    public void SetTitle(string title)
    {
        if (_labelTMP) _labelTMP.text = title;
        if (_labelLegacy) _labelLegacy.text = title;
    }
}
