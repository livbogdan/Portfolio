using System;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameStateListner
{
    public static AudioManager _instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource[] _sfxAudioSources;
    [SerializeField] private AudioSource[] _ambientAudioSources;
    
    private int _sfxIndex = 0;
    private int _ambientIndex = 0;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 1f;

    [Header("Audio Controls")]
    public bool IsSFXOn { get; private set; } = true;
    public bool IsMusicOn { get; private set; } = true;
    public bool IsAmbientOn { get; private set; } = true;

    [Header("Game State Music Clips")]
    [SerializeField] private AudioClip _menuMusicClip;
    [SerializeField] private AudioClip _gameMusicClip;
    [SerializeField] private AudioClip _shopMusicClip;
    [SerializeField] private AudioClip _gameOverMusicClip;
    [SerializeField] private AudioClip _stageCompleteMusicClip;

    private GameState _currentGameState = GameState.MENU;
    private MusicCategory _currentMusicCategory = MusicCategory.Menu;

    [Header("Player Audio Clips")]
    [SerializeField] private AudioClip _playerMoveClip;
    [SerializeField] private AudioClip _playerIdleClip;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
            
        UpdateAllVolumes();
        SettingManager.onMusicStateChanged += UpdateMusicState;
        SettingManager.onSFXStateChanged += UpdateSFXState;
    }

    private void OnDestroy()
    {
        SettingManager.onMusicStateChanged -= UpdateMusicState;
        SettingManager.onSFXStateChanged -= UpdateSFXState;
    }

    private void UpdateMusicState(bool musicState)
    {
        IsMusicOn = musicState;
        UpdateMusicVolume();

        if (!IsMusicOn)
        {
            _musicAudioSource?.Stop();
        }
        else
        {
            ApplyMusicForCurrentState();
        }
    }

    private void UpdateSFXState(bool SFXState)
    {
        IsSFXOn = SFXState;
        if (!IsSFXOn)
        {
            foreach (var sfx in _sfxAudioSources)
                if (sfx != null && sfx.isPlaying) sfx.Stop();
        }
    }

    public void PlayPlayerSFX(AudioClip clip)
    {
        if (!IsSFXOn || _sfxAudioSources == null || _sfxAudioSources.Length == 0) return;

        var sfxSource = GetAvailableSFXSource();
        if (sfxSource != null)
        {
            sfxSource.clip = clip;
            sfxSource.Play();
        }
    }

    public AudioClip GetPlayerMoveClip() => _playerMoveClip;
    public AudioClip GetPlayerIdleClip() => _playerIdleClip;

    private AudioSource GetAvailableSFXSource()
    {
        if (_sfxAudioSources == null || _sfxAudioSources.Length == 0) return null;

        for (int i = 0; i < _sfxAudioSources.Length; i++)
        {
            if (_sfxAudioSources[i] != null && !_sfxAudioSources[i].isPlaying)
                return _sfxAudioSources[i];
        }

        var source = _sfxAudioSources[_sfxIndex];
        _sfxIndex = (_sfxIndex + 1) % _sfxAudioSources.Length;
        return source;
    }

    private AudioSource GetAvailableAmbientSource()
    {
        if (_ambientAudioSources == null || _ambientAudioSources.Length == 0) return null;

        for (int i = 0; i < _ambientAudioSources.Length; i++)
        {
            if (_ambientAudioSources[i] != null && !_ambientAudioSources[i].isPlaying)
                return _ambientAudioSources[i];
        }

        var source = _ambientAudioSources[_ambientIndex];
        _ambientIndex = (_ambientIndex + 1) % _ambientAudioSources.Length;
        return source;
    }

    // Volume Control Methods
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateMusicVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateSFXVolume();
    }

    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        UpdateAmbientVolume();
    }

    private void UpdateAllVolumes()
    {
        UpdateMusicVolume();
        UpdateSFXVolume();
        UpdateAmbientVolume();
    }

    private void UpdateMusicVolume()
    {
        if (_musicAudioSource != null)
            _musicAudioSource.volume = masterVolume * musicVolume * (IsMusicOn ? 1f : 0f);
    }

    private void UpdateSFXVolume()
    {
        if (_sfxAudioSources != null)
        {
            foreach (var sfx in _sfxAudioSources)
                if (sfx != null) sfx.volume = masterVolume * sfxVolume * (IsSFXOn ? 1f : 0f);
        }
    }

    private void UpdateAmbientVolume()
    {
        if (_ambientAudioSources != null)
        {
            foreach (var ambient in _ambientAudioSources)
                if (ambient != null) ambient.volume = masterVolume * ambientVolume * (IsAmbientOn ? 1f : 0f);
        }
    }

    // Audio Control Methods
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (_musicAudioSource == null) return;

        if (clip == null)
        {
            _musicAudioSource.Stop();
            return;
        }

        if (!IsMusicOn)
        {
            _musicAudioSource.clip = clip;
            _musicAudioSource.loop = loop;
            return;
        }

        bool needsTrackChange = _musicAudioSource.clip != clip;
        if (needsTrackChange)
        {
            _musicAudioSource.clip = clip;
            _musicAudioSource.loop = loop;
            _musicAudioSource.Play();
            return;
        }

        if (!_musicAudioSource.isPlaying)
        {
            _musicAudioSource.Play();
        }
    }

    private void ApplyMusicForCurrentState(bool loop = true)
    {
        if (_musicAudioSource == null) return;

        AudioClip clipToPlay = GetMusicClipForGameState(_currentGameState);

        if (clipToPlay == null)
        {
            _musicAudioSource.Stop();
            return;
        }

        PlayMusic(clipToPlay, loop);
    }

    private AudioClip GetMusicClipForGameState(GameState gameState)
    {
        return GetMusicCategory(gameState) switch
        {
            MusicCategory.Menu => _menuMusicClip,
            MusicCategory.Game => _gameMusicClip,
            MusicCategory.Shop => _shopMusicClip,
            MusicCategory.GameOver => _gameOverMusicClip,
            MusicCategory.StageComplete => _stageCompleteMusicClip,
            _ => null
        };
    }

    private MusicCategory GetMusicCategory(GameState gameState)
    {
        return gameState switch
        {
            GameState.MENU => MusicCategory.Menu,
            GameState.CHARACHTERSELECTION => MusicCategory.Menu,
            GameState.UPGRADES => MusicCategory.Menu,
            GameState.IAPSHOP => MusicCategory.Menu,
            GameState.REWARDS => MusicCategory.Menu,
            GameState.TUTORIAL => MusicCategory.Menu,
            GameState.SELECTMODE => MusicCategory.Menu,
            GameState.GAME => MusicCategory.Game,
            GameState.WAVETRANSITION => MusicCategory.Shop,
            GameState.SHOP => MusicCategory.Shop,
            GameState.STAGECOMPLETE => MusicCategory.StageComplete,
            GameState.WEAPONSELECTION => MusicCategory.Menu,
            GameState.GAMEOVER => MusicCategory.GameOver,
            _ => MusicCategory.None
        };
    }

    public void PlayAmbient(AudioClip clip, bool loop = true)
    {
        if (_ambientAudioSources != null && IsAmbientOn)
        {
            var ambientSource = GetAvailableAmbientSource();
            if (ambientSource != null)
            {
                ambientSource.clip = clip;
                ambientSource.loop = loop;
                ambientSource.Play();
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (_sfxAudioSources != null && IsSFXOn)
        {
            var sfxSource = GetAvailableSFXSource();
            if (sfxSource != null) sfxSource.PlayOneShot(clip);
        }
    }

    public void StopMusic() => _musicAudioSource?.Stop();
    public void StopAmbient()
    {
        if (_ambientAudioSources != null)
            foreach (var ambient in _ambientAudioSources) ambient?.Stop();
    }
    public void StopAllSFX()
    {
        if (_sfxAudioSources != null)
            foreach (var sfx in _sfxAudioSources) sfx?.Stop();
    }

    public void ToggleMusic()
    {
        IsMusicOn = !IsMusicOn;
        UpdateMusicVolume();

        if (!IsMusicOn)
        {
            _musicAudioSource?.Stop();
        }
        else
        {
            ApplyMusicForCurrentState();
        }

    }

    public void ToggleSFX()
    {
        IsSFXOn = !IsSFXOn;
        UpdateSFXVolume();

    }

    public void ToggleAmbient()
    {
        IsAmbientOn = !IsAmbientOn;
        UpdateAmbientVolume();

    }


    public void UpdateAudioBasedOnGamePanel(bool isGamePanelActive)
    {
        if (isGamePanelActive)
        {
            StopAmbient();
            ApplyMusicForCurrentState();
        }
        else
        {
            if (IsAmbientOn && _ambientAudioSources != null)
            {
                foreach (var ambient in _ambientAudioSources)
                {
                    if (ambient != null && !ambient.isPlaying && ambient.clip != null)
                    {
                        ambient.Play();
                    }
                }
            }
        }
    }

    #region IGameStateListner Implementation

    public void GameStateChangedCallback(GameState gameState)
    {
        _currentGameState = gameState;
        MusicCategory nextCategory = GetMusicCategory(gameState);

        if (_currentMusicCategory != nextCategory)
        {
            _currentMusicCategory = nextCategory;
            ApplyMusicForCurrentState();
        }
        else if (!_musicAudioSource.isPlaying && IsMusicOn)
        {
            ApplyMusicForCurrentState();
        }

        bool isInGame = gameState == GameState.GAME || gameState == GameState.WAVETRANSITION;
        UpdateAudioBasedOnGamePanel(isInGame);
    }

    #endregion
}
