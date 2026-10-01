using UnityEngine;
using System;
/// <summary>
/// Manages data persistence across application lifecycle events.
/// Ensures all data is properly saved when the application is paused, quit, or loses focus.
/// This is critical for VR applications where users may remove the headset or close the app unexpectedly.
/// 
/// NOTE: This class now delegates to ES3SaveManager for optimized save operations.
/// ES3SaveManager caches all data in memory and only writes to disk on GameOver/App Quit.
/// </summary>
public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[DataPersistence] DataPersistenceManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("[DataPersistence] OnApplicationQuit - Saving all data");
        SaveAllData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // On mobile/VR platforms, save when app goes to background
        if (pauseStatus)
        {
            Debug.Log("[DataPersistence] OnApplicationPause - Saving all data");
            SaveAllData();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Save when losing focus (extra safety net)
        if (!hasFocus)
        {
            Debug.Log("[DataPersistence] OnApplicationFocus (lost) - Saving all data");
            SaveAllData();
        }
    }

    /// <summary>
    /// Saves all game data and forces PlayerPrefs to write to disk.
    /// This ensures data persistence across app sessions.
    /// </summary>
    public void SaveAllData()
    {
        try
        {
            // Delegate to ES3SaveManager if available (optimized path)
            if (ES3SaveManager.Instance != null)
            {
                ES3SaveManager.Instance.SaveToDisk();
                Debug.Log("[DataPersistence] Delegated save to ES3SaveManager");
                return;
            }
            
            // Fallback to direct ES3 operations
            ES3.StoreCachedFile();
            
            Debug.Log("[DataPersistence] All data saved successfully (fallback)");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataPersistence] Error saving data: {e.Message}");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Debug.Log("[DataPersistence] OnDestroy - Final save");
            SaveAllData();
        }
    }
}
