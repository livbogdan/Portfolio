using UnityEngine;
using System.Reflection;

/// <summary>
/// Integration helper for the cheat system to work with existing game systems
/// This script patches into existing systems without modifying their core functionality
/// </summary>
public class CheatSystemIntegration : MonoBehaviour
{
    private static CheatSystemIntegration _instance;
    public static CheatSystemIntegration Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Hook into existing game events
            HookIntoGameSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HookIntoGameSystems()
    {
        // Subscribe to enemy spawn events to apply one-hit kill
        Enemy._onPassAway += OnEnemyDeath;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            Enemy._onPassAway -= OnEnemyDeath;
            _instance = null;
        }
    }

    private void Update()
    {
        // Apply continuous cheat effects
        ApplyActiveCheatEffects();
    }

    private void ApplyActiveCheatEffects()
    {
        if (CheatManager.Instance == null) return;

        // Apply one-hit kill to newly spawned enemies
        if (CheatManager.Instance.OneHitKillActive)
        {
            ApplyOneHitKillToAllEnemies();
        }

        // Ensure infinite health stays active
        if (CheatManager.Instance.InfiniteHealthActive)
        {
            EnsureInfiniteHealth();
        }
    }

    private void ApplyOneHitKillToAllEnemies()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        
        foreach (Enemy enemy in enemies)
        {
            // Use reflection to set enemy health to 1
            FieldInfo healthField = typeof(Enemy).GetField("_health", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (healthField != null)
            {
                float currentHealth = (float)healthField.GetValue(enemy);
                if (currentHealth > 1f) // Only reduce if health is greater than 1
                {
                    healthField.SetValue(enemy, 1f);
                }
            }
        }
    }

    private void EnsureInfiniteHealth()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            // Get current and max health using reflection
            FieldInfo healthField = typeof(PlayerHealth).GetField("_health", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo maxHealthField = typeof(PlayerHealth).GetField("_maxHealth", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (healthField != null && maxHealthField != null)
            {
                float currentHealth = (float)healthField.GetValue(playerHealth);
                float maxHealth = (float)maxHealthField.GetValue(playerHealth);
                
                // If health is not at max, restore it
                if (currentHealth < maxHealth)
                {
                    healthField.SetValue(playerHealth, maxHealth);
                    
                    // Trigger UI update
                    MethodInfo updateUIMethod = typeof(PlayerHealth).GetMethod("UpdateUI", 
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    updateUIMethod?.Invoke(playerHealth, null);
                }
            }
        }
    }

    private void OnEnemyDeath(Vector3 enemyPosition)
    {
        // This can be used to trigger additional cheat effects when enemies die
        // For example, extra currency drops when cheats are active
        if (CheatManager.Instance != null && CheatManager.Instance.CheatsEnabled)
        {
            // Could add bonus drops here
        }
    }

    /// <summary>
    /// Force kill all enemies with proper cleanup
    /// </summary>
    public void ForceKillAllEnemies()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                // Set health to 0 then call PassAway to ensure proper cleanup
                FieldInfo healthField = typeof(Enemy).GetField("_health", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (healthField != null)
                {
                    healthField.SetValue(enemy, 0f);
                    enemy.PassAway();
                }
            }
        }
    }

    /// <summary>
    /// Modify drop rates at runtime
    /// </summary>
    public void SetDropRates(float chestDropRate, float cashDropRate = -1f, float gemDropRate = -1f)
    {
        DropManager dropManager = FindFirstObjectByType<DropManager>();
        if (dropManager != null)
        {
            // Set chest drop rate
            FieldInfo chestField = typeof(DropManager).GetField("_chestDropChance", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            chestField?.SetValue(dropManager, Mathf.Clamp(chestDropRate, 0f, 100f));

            // Set cash drop rate if specified
            if (cashDropRate >= 0)
            {
                FieldInfo cashField = typeof(DropManager).GetField("_cashDropChance", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                cashField?.SetValue(dropManager, Mathf.Clamp(cashDropRate, 0f, 100f));
            }

            // Set gem drop rate if specified
            if (gemDropRate >= 0)
            {
                FieldInfo gemField = typeof(DropManager).GetField("_gemDropChance", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                gemField?.SetValue(dropManager, Mathf.Clamp(gemDropRate, 0f, 100f));
            }
        }
    }

    /// <summary>
    /// Get current drop rates for UI display
    /// </summary>
    public (float chest, float cash, float gem) GetCurrentDropRates()
    {
        DropManager dropManager = FindFirstObjectByType<DropManager>();
        if (dropManager == null) return (0f, 0f, 0f);

        FieldInfo chestField = typeof(DropManager).GetField("_chestDropChance", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo cashField = typeof(DropManager).GetField("_cashDropChance", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo gemField = typeof(DropManager).GetField("_gemDropChance", 
            BindingFlags.NonPublic | BindingFlags.Instance);

        float chestRate = chestField != null ? (float)chestField.GetValue(dropManager) : 0f;
        float cashRate = cashField != null ? (float)cashField.GetValue(dropManager) : 0f;
        float gemRate = gemField != null ? (float)gemField.GetValue(dropManager) : 0f;

        return (chestRate, cashRate, gemRate);
    }

    /// <summary>
    /// Advanced player stat manipulation
    /// </summary>
    public void SetPlayerStat(Stats stat, float value)
    {
        PlayerStatsManager statsManager = FindFirstObjectByType<PlayerStatsManager>();
        if (statsManager != null)
        {
            // First, get current value
            float currentValue = statsManager.GetStatsValue(stat);
            
            // Calculate difference and add it
            float difference = value - currentValue;
            statsManager.AddPlayerStat(stat, difference);
        }
    }

    /// <summary>
    /// Get current player stat value
    /// </summary>
    public float GetPlayerStat(Stats stat)
    {
        PlayerStatsManager statsManager = FindFirstObjectByType<PlayerStatsManager>();
        return statsManager?.GetStatsValue(stat) ?? 0f;
    }

    /// <summary>
    /// Force complete current wave with proper cleanup
    /// </summary>
    public void ForceCompleteWave()
    {
        // Kill all enemies first
        ForceKillAllEnemies();

        // Try regular wave manager first
        WaveManager waveManager = WaveManager.instance;
        if (waveManager != null)
        {
            MethodInfo transitionMethod = typeof(WaveManager).GetMethod("StartWaveTransition", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            transitionMethod?.Invoke(waveManager, null);
            return;
        }

        // Try endless wave manager
        EndlessWaveManager endlessManager = FindFirstObjectByType<EndlessWaveManager>();
        if (endlessManager != null)
        {
            MethodInfo transitionMethod = typeof(EndlessWaveManager).GetMethod("StartWaveTransition", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            transitionMethod?.Invoke(endlessManager, null);
        }
    }

    /// <summary>
    /// Spawn a boss enemy at a random position around the player
    /// </summary>
    public void SpawnBossEnemy()
    {
        // This would need access to boss prefabs - implementation depends on how bosses are stored
        Debug.Log("Boss spawning requires access to boss prefabs in WaveManager");
        
        // For now, we can force trigger a boss wave
        WaveManager waveManager = WaveManager.instance;
        if (waveManager != null)
        {
            // Try to find and spawn a boss from the current wave configuration
            // This is a simplified approach - real implementation would need boss prefab references
        }
    }
}
