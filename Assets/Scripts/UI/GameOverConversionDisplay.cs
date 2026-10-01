using TMPro;
using UnityEngine;

/// <summary>
/// Optional component to display gold-to-gem conversion on the game over screen
/// Add this to your game over UI panel
/// </summary>
public class GameOverConversionDisplay : MonoBehaviour, IGameStateListner
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _conversionText;
    [SerializeField] private GameObject _conversionPanel;
    [SerializeField] private float _displayDuration = 3f;

    [Header("Text Format")]
    [SerializeField] private string _conversionTextFormat = "Converted {0} Gold → {1} Gems!";

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.GAMEOVER)
        {
            DisplayConversion();
        }
    }

    /// <summary>
    /// Display the gold-to-gem conversion that happened
    /// Call this when the game over screen appears
    /// </summary>
    public void DisplayConversion()
    {
        if (CurrencyManager.instance == null)
            return;

        // Get the actual converted amounts from CurrencyManager (already converted by GameManager)
        int goldAmount = CurrencyManager.instance.LastConvertedGold;
        int gemReward = CurrencyManager.instance.LastConvertedGems;

        if (gemReward > 0)
        {
            // Show conversion info
            if (_conversionText != null)
            {
                _conversionText.text = string.Format(_conversionTextFormat, goldAmount, gemReward);
            }

            if (_conversionPanel != null)
            {
                _conversionPanel.SetActive(true);
            }

            // Optionally hide after delay
            if (_displayDuration > 0)
            {
                Invoke(nameof(HideConversionPanel), _displayDuration);
            }
        }
        else
        {
            // No conversion to show
            if (_conversionPanel != null)
            {
                _conversionPanel.SetActive(false);
            }
        }
    }

    private void HideConversionPanel()
    {
        if (_conversionPanel != null)
        {
            _conversionPanel.SetActive(false);
        }
    }
}
