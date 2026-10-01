using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Unlock Rule", menuName = "Scriptable Objects/Character Unlock Rule", order = 1)]
public class CharacterUnlockRuleSO : ScriptableObject
{
    [Header("Unlock Conditions (Any ONE condition can unlock)")]
    public bool useWaveCompletion;
    public int requiredWaves = 15;
    
    public bool useBossKills;
    public int requiredBossKills = 5;
    
    public bool useEndlessWave;
    public int requiredEndlessWave = 20;
    
    public bool useTotalEnemyKills;
    public int requiredEnemyKills = 100;
    
    [Header("Multiple Conditions (ALL must be met)")]
    public bool requireMultipleConditions = false;
    
    // Track if this rule has been triggered
    [NonSerialized] public bool hasBeenUnlocked = false;
}
