using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterButton : MonoBehaviour
{

    [SerializeField] private Image _characterImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _outlineImage;
    [SerializeField] private GameObject _lockObject;
    [SerializeField] private ColorPaletteSO _colorPalette;
    private UIManager _uiManager;

    public Button Button
    {
        get
        {
            return GetComponent<Button>();
        }
        private set
        {

        }
    }

    public void Configure(Sprite characterIcon, bool isUnlocked, int rarity = 0)
    {
        _characterImage.sprite = characterIcon;
        
        // Apply rarity colors
        if (_colorPalette != null)
        {
            Color rarityColor = _colorPalette.GetRarityColor(rarity);
            Color outlineColor = _colorPalette.GetRarityOutlineColor(rarity);
            
            if (_backgroundImage != null)
                _backgroundImage.color = rarityColor;
                
            if (_outlineImage != null)
                _outlineImage.color = outlineColor;
        }

        if (isUnlocked)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }

    public void Lock()
    {
        _lockObject.SetActive(true);
        _characterImage.color = Color.gray;
        
    }

    public void Unlock()
    {
        _lockObject.SetActive(false);
        _characterImage.color = Color.white;

    }
}
