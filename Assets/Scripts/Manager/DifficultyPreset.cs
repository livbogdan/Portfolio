using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty Preset", menuName = "Scriptable Objects/Difficulty Preset", order = 0)]
public class DifficultyPreset : ScriptableObject
{
    [Header("Basic Settings")]
    public string _difficultyName;
    public float _baseWaveDuration = 30f;
    public float _bossWaveDuration = 60f;
    public int _bossInterval = 5;
    
    [Header("Difficulty Scaling")]
    public float _difficultyScalePerWave = 0.1f;
    public float _spawnRateIncreasePerWave = 0.05f;
    public float _healthScalePerWave = 0.15f;
    public float _damageScalePerWave = 0.1f;
    public float _baseSpawnRate = 2f;
    
    [Header("Spawn Settings")]
    public float _minOffset = 5f;
    public float _maxOffset = 15f;
}
