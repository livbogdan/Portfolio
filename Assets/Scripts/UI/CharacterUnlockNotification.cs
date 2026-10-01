using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUnlockNotification : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _notificationPanel;
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private Image _characterImage;
    [SerializeField] private Button _claimButton;
    [SerializeField] private CanvasGroup _canvasGroup;
    
    [Header("Animation Settings")]
    [SerializeField] private float _showDuration = 3f;
    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.5f;
    
    private void Awake()
    {
        // Subscribe to unlock events
        CharacterUnlockManager.OnCharacterUnlocked += ShowUnlockNotification;
        
        // Hide notification by default
        if (_notificationPanel != null)
            _notificationPanel.SetActive(false);
            
        // Setup claim button
        if (_claimButton != null)
            _claimButton.onClick.AddListener(HideNotification);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        CharacterUnlockManager.OnCharacterUnlocked -= ShowUnlockNotification;
    }
    
    private void ShowUnlockNotification(string characterName, int rarity)
    {
        // Find character data by name
        CharterDataSO[] allCharacters = ResourceManager.Character;
        CharterDataSO unlockedCharacter = null;
        
        foreach (var character in allCharacters)
        {
            if (character.CharterName == characterName)
            {
                unlockedCharacter = character;
                break;
            }
        }
        
        if (unlockedCharacter == null)
        {
            Debug.LogWarning($"Could not find character data for: {characterName}");
            return;
        }
        
        // Update UI elements
        string rarityText = GetRarityText(rarity);
        if (_characterNameText != null)
            _characterNameText.text = $"New {rarityText} Character Unlocked!\n{unlockedCharacter.CharterName}";
            
        if (_characterImage != null && unlockedCharacter.CharterSprite != null)
            _characterImage.sprite = unlockedCharacter.CharterSprite;
        
        // Show notification with animation
        if (_notificationPanel != null)
        {
            _notificationPanel.SetActive(true);
            
            // Fade in animation
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                LeanTween.alphaCanvas(_canvasGroup, 1f, _fadeInDuration);
            }
            
            // Auto-hide after duration
            LeanTween.delayedCall(_showDuration, HideNotification);
        }
    }
    
    private void HideNotification()
    {
        if (_notificationPanel != null && _notificationPanel.activeInHierarchy)
        {
            if (_canvasGroup != null)
            {
                // Fade out animation
                LeanTween.alphaCanvas(_canvasGroup, 0f, _fadeOutDuration)
                    .setOnComplete(() => _notificationPanel.SetActive(false));
            }
            else
            {
                _notificationPanel.SetActive(false);
            }
        }
    }
    
    private string GetRarityText(int rarity)
    {
        switch (rarity)
        {
            case 0: return "Starter";
            case 1: return "Basic";
            case 2: return "Advanced";
            case 3: return "Legendary";
            case 4: return "Unique";
            default: return "Special";
        }
    }
}
