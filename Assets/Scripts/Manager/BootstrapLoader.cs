using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Place this script in Scene 0 (Bootstrap scene).
/// It ensures all DontDestroyOnLoad managers are initialized,
/// then automatically loads the Gameplay scene (Scene 1).
/// 
/// Scene 0 persists with all its managers via DontDestroyOnLoad.
/// Scene 1 loads additively on top of Scene 0.
/// GameManager in Scene 1 resets when Scene 1 reloads (no DontDestroyOnLoad).
/// </summary>
public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private int _gameplaySceneIndex = 1;
    [SerializeField] private string _gameplaySceneName = "GameplayScene"; // Alternative: use scene name
    [SerializeField] private bool _useSceneName = false;
    [SerializeField] private float _maxWaitTime = 2f; // Fallback timeout if data never loads

    private bool _dataReady = false;
    private bool _sceneLoaded = false; // Prevent multiple loads

    private void Start()
    {

        StartCoroutine(LoadGameplaySceneAfterManagerInit());
    }


    private IEnumerator LoadGameplaySceneAfterManagerInit()
    {
        // Wait for ES3SaveManager to load data, with a timeout fallback
        float elapsedTime = 0f;
        while (!_dataReady && elapsedTime < _maxWaitTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        
        if (!_dataReady)
        {
            Debug.LogWarning("[BootstrapLoader] Data load timeout after " + _maxWaitTime + " seconds, proceeding anyway");
        }
        
        // All managers should now be ready with data loaded
        LoadGameplayScene();
    }

    private void LoadGameplayScene()
    {
        if (_sceneLoaded)
        {
            Debug.LogWarning("[BootstrapLoader] Scene already loaded, ignoring duplicate load request");
            return;
        }

        _sceneLoaded = true;
        Debug.Log("[BootstrapLoader] Loading gameplay scene additively - all managers initialized");
        
        // Load Scene 1 additively so Scene 0 stays with all its DontDestroyOnLoad managers
        // GameManager in Scene 1 has NO DontDestroyOnLoad, so it resets when Scene 1 reloads
        if (_useSceneName)
        {
            SceneManager.LoadScene(_gameplaySceneName, LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.LoadScene(_gameplaySceneIndex, LoadSceneMode.Additive);
        }
        
        // Wait one frame for Scene 1 to initialize, then set it as active
        StartCoroutine(SetGameplaySceneActive());
    }

    private IEnumerator SetGameplaySceneActive()
    {
        // Wait for Scene 1 to fully load and initialize
        yield return null;
        
        // Set Scene 1 as the active scene for gameplay
        // Scene 0 stays loaded in background with all its DontDestroyOnLoad managers
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(_gameplaySceneIndex));
        Debug.Log("[BootstrapLoader] Scene 1 is now active | Scene 0 managers persist in background");
    }

}
