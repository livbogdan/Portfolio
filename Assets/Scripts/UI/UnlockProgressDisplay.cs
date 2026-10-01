using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UnlockProgressDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private Transform _rulesParent;
    [SerializeField] private GameObject _ruleDisplayPrefab;
    
    private CharacterUnlockManager _unlockManager;
    private List<GameObject> _ruleDisplays = new List<GameObject>();
    
    private void Start()
    {
        _unlockManager = FindFirstObjectByType<CharacterUnlockManager>();
        if (_unlockManager == null)
        {
            Debug.LogWarning("CharacterUnlockManager not found!");
            return;
        }
        
        UpdateProgressDisplay();
        
        // Update display every few seconds
        InvokeRepeating(nameof(UpdateProgressDisplay), 1f, 2f);
    }
    
    private void UpdateProgressDisplay()
    {
        if (_unlockManager == null) return;
        
        // Update main progress text
        if (_progressText != null)
        {
            string progressInfo = $"Unlock Progress:\n" +
                                $"Waves Completed: {_unlockManager.GetWavesCompleted()}\n" +
                                $"Bosses Killed: {_unlockManager.GetBossesKilled()}\n" +
                                $"Highest Endless Wave: {_unlockManager.GetHighestEndlessWave()}\n" +
                                $"Total Enemy Kills: {_unlockManager.GetTotalEnemyKills()}";
            
            _progressText.text = progressInfo;
        }
        
        // Update individual rule displays if using prefab system
        UpdateRuleDisplays();
    }
    
    private void UpdateRuleDisplays()
    {
        if (_rulesParent == null || _ruleDisplayPrefab == null) return;
        
        // Clear existing displays
        foreach (GameObject display in _ruleDisplays)
        {
            if (display != null) Destroy(display);
        }
        _ruleDisplays.Clear();
        
        // Get unlock rules from manager
        var rules = _unlockManager.GetUnlockRules();
        
        foreach (var rule in rules)
        {
            GameObject ruleDisplay = Instantiate(_ruleDisplayPrefab, _rulesParent);
            _ruleDisplays.Add(ruleDisplay);
            
            TextMeshProUGUI ruleText = ruleDisplay.GetComponent<TextMeshProUGUI>();
            if (ruleText != null)
            {
                string status = rule.hasBeenUnlocked ? "✓ UNLOCKED" : GetRuleProgressText(rule);
                string character = rule.targetCharacter != null ? rule.targetCharacter.CharterName : $"Any Rarity {rule.fallbackRarity}";
                string rarity = rule.targetCharacter != null ? $"Rarity {rule.targetCharacter.Rarity}" : $"Rarity {rule.fallbackRarity}";
                
                ruleText.text = $"{character} ({rarity})\n{status}";
                ruleText.color = rule.hasBeenUnlocked ? Color.green : Color.white;
            }
        }
    }
    
    private string GetRuleProgressText(CharacterUnlockRule rule)
    {
        List<string> progressParts = new List<string>();
        
        if (rule.useWaveCompletion)
        {
            int current = _unlockManager.GetWavesCompleted();
            int required = rule.requiredWaves;
            string status = current >= required ? "✓" : $"{current}/{required}";
            progressParts.Add($"Waves: {status}");
        }
        
        if (rule.useBossKills)
        {
            int current = _unlockManager.GetBossesKilled();
            int required = rule.requiredBossKills;
            string status = current >= required ? "✓" : $"{current}/{required}";
            progressParts.Add($"Bosses: {status}");
        }
        
        if (rule.useEndlessWave)
        {
            int current = _unlockManager.GetHighestEndlessWave();
            int required = rule.requiredEndlessWave;
            string status = current >= required ? "✓" : $"{current}/{required}";
            progressParts.Add($"Endless: {status}");
        }
        
        if (rule.useTotalEnemyKills)
        {
            int current = _unlockManager.GetTotalEnemyKills();
            int required = rule.requiredEnemyKills;
            string status = current >= required ? "✓" : $"{current}/{required}";
            progressParts.Add($"Kills: {status}");
        }
        
        string connector = rule.requireMultipleConditions ? " & " : " OR ";
        return string.Join(connector, progressParts);
    }
}
