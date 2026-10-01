using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject _privacyPanel;
    [SerializeField] private GameObject _playPanel;
    [SerializeField] private GameObject _selectModePanel;
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _tutorialPanel;
    private const string PRIVACY_READ_KEY = "PrivacyPolicyRead";

    private void Start()
    {
        CheckPrivacyAndCanPlay();
        PlayPanel();
    }

    private void CheckPrivacyAndCanPlay()
    {
        bool hasReadPrivacy = ES3.Load(PRIVACY_READ_KEY, false);

        if (!hasReadPrivacy)
        {
            _privacyPanel.SetActive(true);
        }
        else
        {
            _privacyPanel.SetActive(false);
        }
    }

    public void MarkPrivacyAsRead()
    {
        ES3.Save(PRIVACY_READ_KEY, true);
        _privacyPanel.SetActive(false);
    }
    public void LoadGameplayScene(int sceneIndex)
    {

        SceneManager.LoadScene(sceneIndex);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }

    public void SelectionMode()
    {
        _selectModePanel.SetActive(true);
        _mainMenuPanel.SetActive(false);
        _tutorialPanel.SetActive(false);
        _playPanel.SetActive(false);
    }

    public void MainMenuPanel()
    {
        _mainMenuPanel.SetActive(true);
        _selectModePanel.SetActive(false);
        _tutorialPanel.SetActive(false);
        _playPanel.SetActive(false);
    }

    public void TutorialPanel()
    {
        _tutorialPanel.SetActive(true);
        _selectModePanel.SetActive(false);
        _mainMenuPanel.SetActive(false);
        _playPanel.SetActive(false);


    }
    public void PlayPanel()
    {
        _playPanel.SetActive(true);
        _selectModePanel.SetActive(false);
        _mainMenuPanel.SetActive(false);
        _tutorialPanel.SetActive(false);
    }
}
