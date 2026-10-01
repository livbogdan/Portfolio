using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [field: SerializeField] public bool UseInfiniteMap { get; private set; }

    public GameState CurrentGameState { get; private set; } = GameState.MENU;

    public static Action _onGamePaused;
    public static Action _onGameResumed;
    
    // Unlock progression events
    public static Action<int> OnWaveCompleted;
    public static Action OnBossKilled;
    public static Action<string> OnEnemyKilled;
    
    // Achievement tracking events
    public static Action OnCriticalHit;
    public static Action<float> OnLifeSteal;
    public static Action<float> OnDamageDealt;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // GameManager stays only in scene 1 - no DontDestroyOnLoad
            // This ensures UI is properly reconnected when scene reloads
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When gameplay scene (index 1) is loaded, set to MENU state
        if (scene.buildIndex == 1)
        {
            SetGameState(GameState.MENU);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {       
        // Only set menu state if we're already in gameplay scene
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SetGameState(GameState.MENU);
        }
    }

    private void Update()
    {
        // Handle pause input using new Input System
        // Meta Quest controller Y button or keyboard P
        var gamepad = Gamepad.current;
        if (gamepad != null && gamepad.yButton.wasPressedThisFrame)
        {
            HandlePauseToggle();
        }
        
    }

    private void HandlePauseToggle()
    {
        if (CurrentGameState == GameState.GAME)
        {
            ShowPause();
        }
        else if (CurrentGameState == GameState.PAUSE)
        {
            ResumeButtonCallback();
        }
    }

    #region GameStates

    public void StartGame() 
    {
        // Only check if weapon is selected when starting wave 1
        int currentWave = GetCurrentWaveNumber();
        if (currentWave <= 1)
        {
            WeaponSelectionManager weaponManager = FindFirstObjectByType<WeaponSelectionManager>();
            if (weaponManager != null && !weaponManager.CanStartGame())
            {
                return;
            }
        }
        
        SetGameState(GameState.GAME);
    }
    
    private void SetWaveMode(WaveMode mode)
    {
        if (WaveManager.instance != null)
        {
            WaveManager.instance.SetWaveMode(mode);
            Debug.Log($"Wave mode set to: {mode}");
        }
        else
        {
            Debug.LogError("WaveManager instance not found!");
        }
    }

    public void SetStructuredMode() => SetWaveMode(WaveMode.Structured);

    public void SetEndlessMode() => SetWaveMode(WaveMode.Endless);
    
    private int GetCurrentWaveNumber()
    {
        // Get current wave number from the appropriate wave manager
        if (WaveManager.instance != null)
            return WaveManager.instance.GetCurrentWaveNumber();
        else if (EndlessWaveManager.instance != null)
            return EndlessWaveManager.instance.GetCurrentWaveNumber();
        
        // Default to wave 1 if no wave manager is found
        return 1;
    }
    public void StartWeaponSelection() => SetGameState(GameState.WEAPONSELECTION);
    public void StartShop() => SetGameState(GameState.SHOP);
    public void ShowUpgrades() => SetGameState(GameState.UPGRADES);
    public void ShowCharacterSelection() => SetGameState(GameState.CHARACHTERSELECTION);
    public void ShowMenu() => SetGameState(GameState.MENU);
    public void ShowIAPShop() => SetGameState(GameState.IAPSHOP);
    public void ShowTutorial() => SetGameState(GameState.TUTORIAL);
    public void ShowSetting() => SetGameState(GameState.SETTINGS);
    public void ShowRewards() => SetGameState(GameState.REWARDS);
    public void ShowSelectMode()
    {
        DestroyAllDroppables();
        SetGameState(GameState.SELECTMODE);
    }
    public void ShowPause()
    {
        PauseButtonCallback();  
        SetGameState(GameState.PAUSE);
    } 

    #endregion

    public void SetGameState(GameState state)
    {
        GameState previousState = CurrentGameState;
        CurrentGameState = state;
        
        // Convert gold to gems when game is over
        if (state == GameState.GAMEOVER)
        {
            ConvertGoldToGemsOnGameOver();
        }

        // Clear any leftover droppables when entering the menu from gameplay.
        if (state == GameState.MENU && previousState != GameState.MENU)
        {
            DestroyAllDroppables();
        }
        
        IEnumerable<IGameStateListner> gameStateListners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameStateListner>();

        foreach (IGameStateListner gameStateListner in gameStateListners)
            gameStateListner.GameStateChangedCallback(state);
    }
    
    private void ConvertGoldToGemsOnGameOver()
    {
        if (CurrencyManager.instance != null)
        {
            int gemsEarned = CurrencyManager.instance.ConvertGoldToGemsOnRunEnd();
            Debug.Log($"Game Over: Converted gold to {gemsEarned} gems");
        }
    }

    public void ManageGameOver()
    {        
        SetGameState(GameState.MENU);
    }
    public void WaveCompletedCallback()
    {
        // Get current wave number for achievements
        int currentWave = 1; // Default
        if (WaveManager.instance != null)
            currentWave = WaveManager.instance.GetCurrentWaveNumber();
        else if (EndlessWaveManager.instance != null)
            currentWave = EndlessWaveManager.instance.GetCurrentWaveNumber();
            
        // Trigger unlock progression event
        OnWaveCompleted?.Invoke(currentWave);
        
        // Track quest reward progress for game wins
        RewardManager.Instance?.UpdateChallengeProgress("weekly_win_3_games", 1);
        RewardManager.Instance?.UpdateChallengeProgress("monthly_win_30_games", 1);
        
        if (Player.instance.HasLeveledUp() || WaveTransitionManager.instance.HasCollectedChest())
        {
            SetGameState(GameState.WAVETRANSITION);
        }
        else
        {
            SetGameState(GameState.SHOP);
        }
    }

    public void PauseButtonCallback()
    {
        _onGamePaused?.Invoke();
        Time.timeScale = 0;
        SetGameState(GameState.PAUSE);
    }

    public void ResumeButtonCallback()
    {
        _onGameResumed?.Invoke();
        Time.timeScale = 1;
        SetGameState(GameState.GAME);
    }

    public void RestartButtonCallback()
    {
        ResumeButtonCallback();
        
        // Reset wave manager state for a fresh run
        if (WaveManager.instance != null)
            WaveManager.instance.ResetWaveState();
        
        // Reset player health
        if (Player.instance != null)
        {
            PlayerHealth playerHealth = Player.instance.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.ResetHealth();
        }
        
        // Reset run currency (gold) for the new run
        if (CurrencyManager.instance != null)
            CurrencyManager.instance.ResetRunCurrency();
        
        // Destroy all coins/gems/cash left on the scene
        DestroyAllDroppables();
        
        ShowSelectMode();
    }
    
    private void DestroyAllDroppables()
    {
        DroppableCurrency[] droppables = FindObjectsByType<DroppableCurrency>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (DroppableCurrency droppable in droppables)
            Destroy(droppable.gameObject);
    }

    /// <summary>
    /// Reloads the gameplay scene (Scene 1) - useful for full game restart
    /// </summary>
    public void ReloadGameplayScene()
    {
        ResumeButtonCallback(); // Ensure time scale is reset
        SceneManager.LoadScene(1);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        // Reset singleton when GameManager is destroyed (scene unloads)
        // This allows a new GameManager to be created when scene 1 reloads
        if (Instance == this)
        {
            Instance = null;
            Debug.Log("[GameManager] Instance reset - GameManager destroyed with scene");
        }
    }
}

public interface IGameStateListner
{
    void GameStateChangedCallback(GameState gameState);
}