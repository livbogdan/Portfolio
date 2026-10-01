using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListner
{

    [Header("Panels")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _selectModePanel;
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _weaponSelectionPanel;
    [SerializeField] private GameObject _waveTransitionPanel;
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private GameObject _iapShopPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _stageCompletePanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _restartConfimPanel;
    [SerializeField] private GameObject _characterSelectionPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _upgradesPanel;
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private GameObject _privacyPanel;
    [SerializeField] private GameObject _rewardsPanel;

    [HorizontalLine(5, EColor.White)]
    [SerializeField] private GameObject _gameplayObject;
    [SerializeField] private GameObject _player;

    private List<GameObject> _panels = new List<GameObject>();

    private const string PRIVACY_READ_KEY = "PrivacyPolicyRead";

    private void Awake()
    {
        _panels.AddRange(new GameObject[]
        {
            _menuPanel,
            _selectModePanel,
            _characterSelectionPanel,
            _upgradesPanel,
            _gamePanel,
            _waveTransitionPanel,
            _shopPanel,
            _gameOverPanel,
            _stageCompletePanel,
            _weaponSelectionPanel,
            _gameplayObject,
            _player,
            _iapShopPanel,
            _tutorialPanel,
            _settingsPanel,
            _pausePanel,
            _rewardsPanel,
        });

        CheckPrivacyPanel();
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        GameObject panel = gameState switch
        {
            GameState.MENU => _menuPanel,
            GameState.SELECTMODE => _selectModePanel,
            GameState.CHARACHTERSELECTION => _characterSelectionPanel,
            GameState.UPGRADES => _upgradesPanel,
            GameState.GAME => _gamePanel,
            GameState.WAVETRANSITION => _waveTransitionPanel,
            GameState.SHOP => _shopPanel,
            GameState.WEAPONSELECTION => _weaponSelectionPanel,
            GameState.GAMEOVER => _gameOverPanel,
            GameState.STAGECOMPLETE => _stageCompletePanel,
            GameState.IAPSHOP => _iapShopPanel,
            GameState.TUTORIAL => _tutorialPanel,
            GameState.SETTINGS => _settingsPanel,
            GameState.PAUSE => _pausePanel,
            GameState.REWARDS => _rewardsPanel,
            _ => null
        };

        if (panel != null)
            ShowPanel(panel);

        bool isGameActive = gameState == GameState.GAME;
        _gameplayObject.SetActive(isGameActive);
        _player.SetActive(isGameActive);
        AudioManager._instance?.UpdateAudioBasedOnGamePanel(isGameActive);
    }

    private void ShowPanel(GameObject panel)
    {
        foreach (GameObject panelPanel in _panels)
            panelPanel.SetActive(panelPanel == panel);
    }

    private void CheckPrivacyPanel()
    {
        if (_privacyPanel == null) return;
        
        bool hasReadPrivacy = ES3.Load(PRIVACY_READ_KEY, false);
        _privacyPanel.SetActive(!hasReadPrivacy);
    }

    public void ClosePrivacyPanelPermanently()
    {
        ES3.Save(PRIVACY_READ_KEY, true);
        if (_privacyPanel != null)
            Destroy(_privacyPanel);
    }

    public void ToggleUIPanel(GameObject panel)
    {
        if (panel == null) return;
        
        bool isActive = panel.activeSelf;
        ShowPanel(isActive ? null : panel);
    }
}
