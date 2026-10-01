using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Button _sfxButton;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Color _onColor;
    [SerializeField] private Color _offColor;

    private bool _sfxState;
    private bool _musicState;

    [Header("Actions")]
    public static Action<bool> onSFXStateChanged;
    public static Action<bool> onMusicStateChanged;


    private void Awake()
    {
        _sfxButton.onClick.RemoveAllListeners();
        _sfxButton.onClick.AddListener(SFXButtonCallback);

        _musicButton.onClick.RemoveAllListeners();
        _musicButton.onClick.AddListener(MusickButtonCallback);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadData();
        onSFXStateChanged?.Invoke(_sfxState);
        onMusicStateChanged?.Invoke(_musicState);
    }

    private void SFXButtonCallback()
    {
        _sfxState = !_sfxState;
        UpdateSFXVisual();
        onSFXStateChanged?.Invoke(_sfxState);

    }

    private void UpdateSFXVisual()
    {
        if (_sfxState)
        {
            _sfxButton.image.color = _onColor;
            _sfxButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else
        {
            _sfxButton.image.color = _offColor;
            _sfxButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }
    private void MusickButtonCallback()
    {
        _musicState = !_musicState;
        UpdateMusicVisual();

        onMusicStateChanged?.Invoke(_musicState);
    }

    private void UpdateMusicVisual()
    {
        if (_musicState)
        {
            _musicButton.image.color = _onColor;
            _musicButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else
        {
            _musicButton.image.color = _offColor;
            _musicButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }

    private void LoadData()
    {
        // Use same keys as AudioManager for consistency

        UpdateSFXVisual();
        UpdateMusicVisual();
    }

}
