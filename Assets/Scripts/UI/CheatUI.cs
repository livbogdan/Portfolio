using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CheatUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject _cheatPanel;
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] private Button _cheatButtonPrefab;
    [SerializeField] private TMP_InputField _goldAmountInput;
    [SerializeField] private TMP_InputField _chestDropChanceInput;
    [SerializeField] private Button _toggleCheatButton;
    [SerializeField] private TextMeshProUGUI _cheatStatusText;

    [Header("UI Settings")]
    [SerializeField] private Color _enabledColor = Color.green;
    [SerializeField] private Color _disabledColor = Color.red;

    private List<Button> _cheatButtons = new List<Button>();
    private CanvasGroup _panelCanvasGroup;

    private void Start()
    {
        InitializeUI();
        SetupToggleButton();
        UpdateCheatStatus();
    }

    private void InitializeUI()
    {
        if (_cheatPanel != null)
        {
            _panelCanvasGroup = _cheatPanel.GetComponent<CanvasGroup>();
            if (_panelCanvasGroup == null)
                _panelCanvasGroup = _cheatPanel.AddComponent<CanvasGroup>();
            
            _cheatPanel.SetActive(false);
        }

        // Set default input values
        if (_goldAmountInput != null)
            _goldAmountInput.text = "100";
        
        if (_chestDropChanceInput != null)
            _chestDropChanceInput.text = "10";
    }

    private void SetupToggleButton()
    {
        if (_toggleCheatButton != null)
        {
            _toggleCheatButton.onClick.AddListener(() => {
                if (CheatManager.Instance != null)
                {
                    CheatManager.Instance.ToggleCheats();
                    UpdateCheatStatus();
                }
            });
        }
    }

    public void UpdateCheatStatus()
    {
        bool cheatsEnabled = CheatManager.Instance != null && CheatManager.Instance.CheatsEnabled;
        
        if (_cheatStatusText != null)
        {
            _cheatStatusText.text = cheatsEnabled ? "CHEATS: ON" : "CHEATS: OFF";
            _cheatStatusText.color = cheatsEnabled ? _enabledColor : _disabledColor;
        }

        // Update button interactability
        foreach (Button button in _cheatButtons)
        {
            if (button != null)
                button.interactable = cheatsEnabled;
        }

        // Update panel transparency
        if (_panelCanvasGroup != null)
        {
            _panelCanvasGroup.alpha = cheatsEnabled ? 1f : 0.5f;
        }
    }

    public void ShowCheatPanel()
    {
        if (_cheatPanel != null)
            _cheatPanel.SetActive(true);
    }

    public void HideCheatPanel()
    {
        if (_cheatPanel != null)
            _cheatPanel.SetActive(false);
    }

    public void ToggleCheatPanel()
    {
        if (_cheatPanel != null)
            _cheatPanel.SetActive(!_cheatPanel.activeSelf);
    }

    public Button CreateCheatButton(string buttonText, System.Action buttonAction)
    {
        if (_cheatButtonPrefab == null || _buttonContainer == null) return null;

        Button newButton = Instantiate(_cheatButtonPrefab, _buttonContainer);
        TextMeshProUGUI buttonTextComponent = newButton.GetComponentInChildren<TextMeshProUGUI>();
        
        if (buttonTextComponent != null)
            buttonTextComponent.text = buttonText;

        newButton.onClick.AddListener(() => {
            if (CheatManager.Instance != null && CheatManager.Instance.CheatsEnabled)
            {
                buttonAction.Invoke();
                UpdateCheatStatus(); // Update UI after cheat action
            }
        });

        _cheatButtons.Add(newButton);
        return newButton;
    }

    private void Update()
    {
        // Handle keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleCheatPanel();
        }

        if (Input.GetKeyDown(KeyCode.F2) && CheatManager.Instance != null)
        {
            CheatManager.Instance.ToggleCheats();
            UpdateCheatStatus();
        }
    }

    // Button callback methods for UI
    public void OnInfiniteHealthToggle()
    {
        CheatManager.Instance?.ToggleInfiniteHealth();
        UpdateCheatStatus();
    }

    public void OnLevelUpClick()
    {
        CheatManager.Instance?.LevelUpPlayer();
    }

    public void OnAddGoldClick()
    {
        if (CheatManager.Instance != null && _goldAmountInput != null)
        {
            if (int.TryParse(_goldAmountInput.text, out int amount))
                CheatManager.Instance.AddGold(amount);
        }
    }

    public void OnCompleteWaveClick()
    {
        CheatManager.Instance?.CompleteCurrentWave();
    }

    public void OnKillAllEnemiesClick()
    {
        CheatManager.Instance?.KillAllEnemies();
    }

    public void OnOneHitKillToggle()
    {
        CheatManager.Instance?.ToggleOneHitKill();
        UpdateCheatStatus();
    }

    public void OnSetChestDropRateClick()
    {
        if (CheatManager.Instance != null && _chestDropChanceInput != null)
        {
            if (float.TryParse(_chestDropChanceInput.text, out float rate))
                CheatManager.Instance.SetChestDropRate(rate);
        }
    }

    public void OnMaxAllStatsClick()
    {
        CheatManager.Instance?.MaxAllStats();
    }

    public void OnResetAllStatsClick()
    {
        CheatManager.Instance?.ResetAllStats();
    }

    public void OnUnlockAllCharactersClick()
    {
        CheatManager.Instance?.UnlockAllCharacters();
    }

    public void OnFullHealthRecoveryClick()
    {
        CheatManager.Instance?.FullHealthRecovery();
    }

    public void OnGodModeToggle()
    {
        CheatManager.Instance?.ToggleGodMode();
        UpdateCheatStatus();
    }

    private void OnDestroy()
    {
        // Clean up button references
        _cheatButtons.Clear();
    }
}
