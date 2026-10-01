using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.UI;
using NaughtyAttributes;

public class WeaponSelectionManager : MonoBehaviour, IGameStateListner
{
    [Header("References")]
    [SerializeField] private Transform _containerParent;
    [SerializeField] private WeaponSelectionContainer[] _containerPrefab;
    [SerializeField] private PlayerWeapon _playerWeapons;

    [Header("Data")]
    [SerializeField] private WeaponDataSO[] allAvailableWeapons; // Assign all 110 weapons here
    [SerializeField] private WeaponDataSO[] _starterWeapon; // Will be populated with 3 random weapons

    [Header("UI")]
    [SerializeField] private Button _startButton;
    private WeaponDataSO _selectedWeapon;
    private int _initialWeaponLevel;

    public void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GAME:

                if (_selectedWeapon == null)
                    return;

                _playerWeapons.TryAddWeapon(_selectedWeapon, _initialWeaponLevel);
                _selectedWeapon = null;
                _initialWeaponLevel = 0;

                break;

            case GameState.WEAPONSELECTION:
                _playerWeapons.ClearAllWeapons();
                SetRandomStarterWeapons(); // Set 3 random weapons before configuring containers
                ConfigureWeaponContainerButtons();
                DisableStartButton();
                break;

        }
    }

    private void SetRandomStarterWeapons()
    {
        if (allAvailableWeapons == null || allAvailableWeapons.Length == 0)
        {
            Debug.LogError("No weapons available in allAvailableWeapons array!");
            return;
        }

        // Ensure we don't try to select more weapons than available
        int weaponsToSelect = Mathf.Min(3, allAvailableWeapons.Length);

        // Use LINQ to randomly shuffle and take 3 unique weapons
        _starterWeapon = allAvailableWeapons.OrderBy(x => Random.value).Take(weaponsToSelect).ToArray();

        Debug.Log($"Selected {_starterWeapon.Length} random starter weapons");
    }

    
    public void TryStartGame()
    {
        if (CanStartGame())
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogWarning("Please select a weapon before starting the game!");
        }
    }

    [Button]
    private void ConfigureWeaponContainerButtons()
    {
        // Clear our parent container
        _containerParent.Clear();
        // Generate weapon containers
        for (int i = 0; i < _starterWeapon.Length; i++)
        {
            GenerateWeaponContainer();
        }
    }


    private void GenerateWeaponContainer()
    {
        WeaponSelectionContainer containerInstance = Instantiate(_containerPrefab[Random.Range(0, _containerPrefab.Length)], _containerParent);
        WeaponDataSO weaponData = _starterWeapon[Random.Range(0, _starterWeapon.Length)];

        int level = Random.Range(0, 4);

        containerInstance.Configure(weaponData, level);
        containerInstance.GetButton().onClick.RemoveAllListeners();
        containerInstance.GetButton().onClick.AddListener(() => WeaponSelectedCallback(weaponData, containerInstance, level));
    }

    private void WeaponSelectedCallback(WeaponDataSO weaponData, WeaponSelectionContainer containerInstance, int level)
    {
        _selectedWeapon = weaponData;
        _initialWeaponLevel = level;

        EnableStartButton();

        foreach (WeaponSelectionContainer container in _containerParent.GetComponentsInChildren<WeaponSelectionContainer>())
        {
            if (container == containerInstance)
                container.Select();
            else
                container.Deselect();
        }
    }
    
    
    private void DisableStartButton()
    {
        if (_startButton != null)
        {
            _startButton.interactable = false;
        }
    }

    private void EnableStartButton()
    {
        if (_startButton != null)
        {
            _startButton.interactable = true;
        }
    }

    public bool CanStartGame()
    {
        return _selectedWeapon != null;
    }
}
