using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using NaughtyAttributes;

public class WaveTransitionManager : MonoBehaviour, IGameStateListner
{

    public static WaveTransitionManager instance;

    [Header("Player References")]
    [SerializeField] private PlayerObject _playerObjects;

    
    [Header("References")]
    [SerializeField] private PlayerStatsManager _playerStatsManager;
    [SerializeField] private UpgradeContainer[] _upgradeContainers;
    [SerializeField] private GameObject _upgradeContainersParent;

    [Header("Chest References")]
    [SerializeField] private ChestObjectContainer _chestContainerPrefab;
    [SerializeField] private Transform _chestContainerParent;

    [Header("Shop References")]
    [SerializeField] private Button _shopButton;

    private int _chestCollected;
    private CharterDataSO _currentCharterData;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Chest._onCollected += ChestSelectedCallback;
        CharacterSelectionManager._onCharacterSelected += OnCharacterSelected;
    }

    private void OnDestroy()
    {
        Chest._onCollected -= ChestSelectedCallback;
        CharacterSelectionManager._onCharacterSelected -= OnCharacterSelected;
    }

    private void OnCharacterSelected(CharterDataSO charterData)
    {
        _currentCharterData = charterData;
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.WAVETRANSITION:
                TryOpenChest();
                break;
            case GameState.SELECTMODE:
                _chestCollected = 0;
                break;
        }
    }

    private void TryOpenChest()
    {
        foreach (Transform child in _chestContainerParent)
        {
            Destroy(child.gameObject);
        }

        // Disable shop button until player makes a choice
        if (_shopButton != null)
            _shopButton.interactable = false;

        if (_chestCollected > 0)
            ShowObject();
        else
            ConfigureUpgradeContainerButtons();
    }

    private void ShowObject()
    {
        _chestCollected--;

        _upgradeContainersParent.SetActive(false);

        ObjectDataSO[] objectDatas = ResourceManager.Objects;
        ObjectDataSO randomObjectData = objectDatas[Random.Range(0, objectDatas.Length)];


        ChestObjectContainer chestObjectContainer = Instantiate(_chestContainerPrefab, _chestContainerParent.transform);
        chestObjectContainer.Configure(randomObjectData);
        chestObjectContainer.TakeButton.onClick.AddListener(() => TakeButtonCallback(randomObjectData));
        chestObjectContainer.RecycleButton.onClick.AddListener(() => RecycleButtonCallback(randomObjectData));

    }

    private void TakeButtonCallback(ObjectDataSO objectDataToTake)
    {
        _playerObjects.AddObject(objectDataToTake);
        EnableShopButton();
        TryOpenChest();
    }

    private void RecycleButtonCallback(ObjectDataSO objectDataToRecycle)
    {
        CurrencyManager.instance.AddCurrency(objectDataToRecycle.RecyclePrice);
        EnableShopButton();
        TryOpenChest();
    }

    [Button]
    private void ConfigureUpgradeContainerButtons()
    {
        _upgradeContainersParent.SetActive(true);

        List<Stats> statsPool = GetUpgradeableStats();
        // Shuffle so choices are unique
        List<Stats> shuffled = statsPool.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < _upgradeContainers.Length; i++)
        {
            Stats stat = i < shuffled.Count
                ? shuffled[i]
                : shuffled[Random.Range(0, shuffled.Count)];

            Sprite upgradeSprite = ResourceManager.GetStatsIcon(stat);
            string statName = Enums.FormatStateName(stat);
            string buttonString;
            Action action = GetActionToPerform(stat, out buttonString);

            _upgradeContainers[i].Configure(upgradeSprite, statName, buttonString);

            Button btn = _upgradeContainers[i].GetButton();
            btn.interactable = true;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => action.Invoke());
            btn.onClick.AddListener(() => BonusSelectedCallback());
        }
    }

    private List<Stats> GetUpgradeableStats()
    {
        if (_currentCharterData != null && _currentCharterData.UpgradeConfig != null)
        {
            List<Stats> enabled = _currentCharterData.UpgradeConfig.GetEnabledStats();
            if (enabled.Count > 0)
                return enabled;
        }
        // Fallback: all stats
        return Enum.GetValues(typeof(Stats)).Cast<Stats>().ToList();
    }

    private void BonusSelectedCallback()
    {
        EnableShopButton();
        GameManager.Instance.WaveCompletedCallback();
    }

    private void EnableShopButton()
    {
        if (_shopButton != null)
            _shopButton.interactable = true;
    }

    private Action GetActionToPerform(Stats stat, out string buttonString)
    {
        buttonString = "";
        float value = 0;

        // Check if this is a boss wave for bonus rewards
        bool isBossWave = IsBossWave();
        float bossMultiplier = isBossWave ? 2f : 1f;

        value = Random.Range(1, 10);
        buttonString = "+" + value.ToString() + "%";

        switch (stat)
        {
            case Stats.Attack:
                value = Random.Range(1, 5) * bossMultiplier;
                buttonString = "+" + value.ToString("F2");
                break;
            case Stats.AttackSpeed:
                value = Random.Range(1f, 5f) * bossMultiplier;
                buttonString = "+" + value.ToString("F2") + "%";
                break;
            case Stats.CriticalChance:
                value = Random.Range(1f, 5f) * bossMultiplier;
                buttonString = "+" + value.ToString("F2") + "%";
                break;
            case Stats.CriticalPercent:
                value = Random.Range(0.01f, 2) * bossMultiplier;
                buttonString = "+" + value.ToString("F2") + "x";
                break;
            case Stats.MoveSpeed:
                value = Random.Range(1, 5) * bossMultiplier;
                buttonString = "+" + value.ToString("F2") + "%";
                break;
            case Stats.MaxHealth:
                value = Random.Range(1, 5) * bossMultiplier;
                buttonString = "+" + value;
                break;
            case Stats.Range:
                value = Random.Range(1, 10) * bossMultiplier;
                buttonString = "+" + value.ToString();
                break;
            case Stats.HealthRecoverySpeed:
                value = Random.Range(1, 5) * bossMultiplier;
                buttonString = "+" + value.ToString() + "%";
                break;
            case Stats.Armor:
                value = Random.Range(1, 5) * bossMultiplier;
                buttonString = "+" + value.ToString() + "%";
                break;
            case Stats.Luck:
                value = Random.Range(1, 5) * bossMultiplier;
                buttonString = "+" + value.ToString() + "%";
                break;
            case Stats.Dodge:
                value = Random.Range(1, 5) * bossMultiplier;
                buttonString = "+" + value.ToString() + "%";
                break;
            case Stats.LifeSteal:
                value = Random.Range(1, 5) * bossMultiplier;
                buttonString = "+" + value.ToString() + "%";
                break;

            default:
                return () => Debug.Log("Unknown");
        }

        return () => _playerStatsManager.AddPlayerStat(stat, value);
    }

    private bool IsBossWave()
    {
        // Check if EndlessWaveManager exists and if it's a boss wave
        EndlessWaveManager endlessManager = FindFirstObjectByType<EndlessWaveManager>();
        return endlessManager != null && endlessManager.IsBossWave();
    }

    private void ChestSelectedCallback()
    {
        _chestCollected++;
    }

    public bool HasCollectedChest() => _chestCollected > 0;

}
