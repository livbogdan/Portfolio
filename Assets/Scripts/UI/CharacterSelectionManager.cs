using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _characterButtonsParent;
    [SerializeField] private CharacterButton _characterButtonPrefab;
    [SerializeField] private Image _characterImage;
    [SerializeField] private CharacterInfoPanel _characterInfo;
    private CharterDataSO[] _charterDataSOs;
    private List<bool> _unlockedStates = new List<bool>();
    private int _selectedCharacterIndex;
    private int _lastSelectedCharacterIndex;
    private const string _unlockedStateskey = "unlockedStates";
    private const string _lastSelectedCharacterIndexKey = "lastSelectedCharacterIndex";

    public static Action<CharterDataSO> _onCharacterSelected;
    
    private bool _isInitializing = true; // Prevent auto-transition during startup
    
    // Method to unlock character by progression (called from CharacterUnlockManager)
    public void UnlockCharacterByProgression(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < _unlockedStates.Count)
        {
            _unlockedStates[characterIndex] = true;
            _lastSelectedCharacterIndex = characterIndex; // Auto-select the newly unlocked character

            
            // Update the UI button if it exists
            if (characterIndex < _characterButtonsParent.childCount)
            {
                CharacterButton button = _characterButtonsParent.GetChild(characterIndex).GetComponent<CharacterButton>();
                if (button != null)
                {
                    button.Unlock();
                }
            }
            
            // Refresh character info display for the newly unlocked character
            if (_charterDataSOs != null && characterIndex < _charterDataSOs.Length)
            {
                CharacterSelectedCallback(characterIndex);
            }
        }
    }

    
    private void ReloadUnlockStates()
    {

        // Refresh UI
        Initialize();
        CharacterSelectedCallback(_lastSelectedCharacterIndex);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterInfo.BuyButton.onClick.RemoveAllListeners();
        _characterInfo.BuyButton.onClick.AddListener(BuyButtonCallback);

        LoadData();
    }

    private void Initialize()
    {
        // Clear existing buttons first
        for (int i = _characterButtonsParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_characterButtonsParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < _charterDataSOs.Length; i++)
        {
            CreateCharacterButton(i);
        }
    }

    private void CreateCharacterButton(int index )
    {
        CharterDataSO charterDataSO = _charterDataSOs[index];

        CharacterButton charterButtonInstance = Instantiate(_characterButtonPrefab, _characterButtonsParent);
        charterButtonInstance.Configure(charterDataSO.CharterSprite, _unlockedStates[index], charterDataSO.Rarity);

        charterButtonInstance.Button.onClick.RemoveAllListeners();
        charterButtonInstance.Button.onClick.AddListener(() => CharacterSelectedCallback(index));
    }

    private void CharacterSelectedCallback(int index)
    {
        if (_charterDataSOs == null || index >= _charterDataSOs.Length)
        {
            Debug.LogError($"CharterDataSOs is null or index {index} is out of bounds");
            return;
        }

        _selectedCharacterIndex = index;

        CharterDataSO charterDataSO = _charterDataSOs[index];

        if (_unlockedStates[index])
        {
            _lastSelectedCharacterIndex = index;
            _characterInfo.BuyButton.interactable = false;

            _onCharacterSelected?.Invoke(charterDataSO);
            
            // Set current character in MetaUpgradeManager (but don't open upgrades panel yet)
            if (MetaUpgradeManager.instance != null)
            {
                MetaUpgradeManager.instance.SetCurrentCharacter(charterDataSO);
            }
            
            // Don't automatically transition to upgrades - let player choose via button
        }
        else
        {
            // Check if character can only be unlocked through progression
            if (charterDataSO.ProgressionOnlyUnlock)
            {
                _characterInfo.BuyButton.interactable = false;
            }
            else
            {
                _characterInfo.BuyButton.interactable =
                    CurrencyManager.instance.HasEnoughtGemCurrency(charterDataSO.CharterPrice);
            }
        }

        _characterImage.sprite = charterDataSO.CharterSprite;
        _characterInfo.Configure(charterDataSO, _unlockedStates[index]);
    }
    private void BuyButtonCallback()
    {
        if (_charterDataSOs == null || _selectedCharacterIndex >= _charterDataSOs.Length)
        {
            Debug.LogError($"CharterDataSOs is null or selectedCharacterIndex {_selectedCharacterIndex} is out of bounds");
            return;
        }

        CharterDataSO selectedCharacter = _charterDataSOs[_selectedCharacterIndex];
        
        // Check if character can only be unlocked via progression
        if (selectedCharacter.ProgressionOnlyUnlock)
        {
            Debug.LogWarning($"Cannot buy {selectedCharacter.CharterName} - this character can only be unlocked through progression!");
            return;
        }
        
        // Check if already unlocked
        if (_unlockedStates[_selectedCharacterIndex])
        {
            Debug.LogWarning($"{selectedCharacter.CharterName} is already unlocked!");
            return;
        }

        int price = selectedCharacter.CharterPrice;
        
        // Verify currency before purchase
        if (!CurrencyManager.instance.HasEnoughtGemCurrency(price))
        {
            Debug.LogWarning($"Not enough gems to unlock {selectedCharacter.CharterName}. Need {price} gems.");
            return;
        }
        
        // Deduct currency
        CurrencyManager.instance.UseGemCurrency(price);
        Debug.Log($"Purchased {selectedCharacter.CharterName} for {price} gems");

        // Unlock the character
        _unlockedStates[_selectedCharacterIndex] = true;
        _lastSelectedCharacterIndex = _selectedCharacterIndex;

        // Update UI button
        _characterButtonsParent.GetChild(_selectedCharacterIndex).GetComponent<CharacterButton>().Unlock();

        // Refresh character display
        CharacterSelectedCallback(_selectedCharacterIndex);

        
        Debug.Log($"Character {selectedCharacter.CharterName} unlocked and saved!");
    }



    private void LoadData()
    {
        _charterDataSOs = ResourceManager.Character;

        if (_charterDataSOs == null || _charterDataSOs.Length == 0)
        {
            Debug.LogError("Failed to load character data from ResourceManager.Character");
            return;
        }

        // Sort characters by rarity (0-4) then by price (lowest to highest)
        _charterDataSOs = _charterDataSOs
            .OrderBy(c => c.Rarity)
            .ThenBy(c => c.CharterPrice)
            .ToArray();

        for (int i = 0; i < _charterDataSOs.Length; i++)
            _unlockedStates.Add(i == 0);


        Initialize();
        
        CharacterSelectedCallback(_lastSelectedCharacterIndex);
        
        // Initialization complete - allow automatic transitions now
        _isInitializing = false;
    }

}
