using UnityEngine;
using System.Collections.Generic;
using System;

public class InfiniteMap : MonoBehaviour, IGameStateListner
{
    [Serializable]
    public class WaveMapTheme
    {
        public int _startWave;
        public GameObject _mapFragmentPrefab;
        public string _themeName;
    }

    [SerializeField] private WaveMapTheme[] _waveThemes;
    [SerializeField] private int _mapSize = 10;
    
    private List<GameObject> _currentMapFragments = new List<GameObject>();
    private int _lastWaveForMapChange = 0;
    private int _currentThemeIndex = 0;

    void Start() 
    {
        // Initialize with first theme
        if (_waveThemes.Length > 0)
        {
            GenerateMap();
        }
        else
        {
            Debug.LogWarning("No wave themes assigned to InfiniteMap!");
        }
    }

    private void GenerateMap()
    {
        ClearCurrentMap();
        
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                GenerateMapFragments(x, y);
            }
        }
        
        Debug.Log($"Generated map with theme: {GetCurrentTheme()._themeName}");
    }

    private void GenerateMapFragments(int x, int y)
    {
        Vector3 spawnPosition = new Vector3(x, y) * _mapSize;
        GameObject fragment = Instantiate(GetCurrentTheme()._mapFragmentPrefab, spawnPosition, Quaternion.identity, transform);
        _currentMapFragments.Add(fragment);
    }
    
    private void ClearCurrentMap()
    {
        foreach (GameObject fragment in _currentMapFragments)
        {
            if (fragment != null)
                DestroyImmediate(fragment);
        }
        _currentMapFragments.Clear();
    }
    
    private WaveMapTheme GetCurrentTheme()
    {
        if (_waveThemes.Length == 0) return null;
        return _waveThemes[_currentThemeIndex];
    }
    
    private void CheckForMapThemeChange(int currentWave)
    {
        // Find the appropriate theme for current wave
        int newThemeIndex = 0;
        for (int i = _waveThemes.Length - 1; i >= 0; i--)
        {
            if (currentWave >= _waveThemes[i]._startWave)
            {
                newThemeIndex = i;
                break;
            }
        }
        
        // Change map if theme changed
        if (newThemeIndex != _currentThemeIndex)
        {
            _currentThemeIndex = newThemeIndex;
            GenerateMap();
            Debug.Log($"Map theme changed to: {GetCurrentTheme()._themeName} at wave {currentWave}");
        }
    }
    
    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.GAME && EndlessWaveManager.instance != null)
        {
            int currentWave = EndlessWaveManager.instance.GetCurrentWave();
            CheckForMapThemeChange(currentWave);
        }
    }
    
    // Call this method when a wave is completed (called from EndlessWaveManager)
    public void OnWaveCompleted(int waveNumber)
    {
        CheckForMapThemeChange(waveNumber);
    }
}