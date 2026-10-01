using TMPro;
using UnityEngine;

public class WaveManagerUI : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] public TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _segmentText;

    [Header("Boss Wave Styling")]
    [SerializeField] private Color _normalWaveColor = Color.white;
    [SerializeField] private Color _bossWaveColor = Color.red;

    public void UpdateWaveText(string waveString) => _waveText.text = waveString;
    public void UpdateTimerText(string timerString) => _timerText.text = timerString;
    public void UpdateSegmentText(string segmentString) => _segmentText.text = segmentString;



    public void UpdateWaveText(string waveString, bool isBossWave)
    {
        _waveText.text = waveString;
        _waveText.color = isBossWave ? _bossWaveColor : _normalWaveColor;

        if (isBossWave)
        {
            _waveText.fontSize = 30f;
        }
    }

    public void UpdateTimerText(string timerString, bool isBossWave)
    {
        _timerText.text = timerString;
        _timerText.color = isBossWave ? _bossWaveColor : _normalWaveColor;
    }

    public void ShowWavesCompleted()
    {
        _waveText.text = "All Waves Completed!!!";
        _timerText.text = "";
    }

    public void ShowEndlessMode()
    {
        _waveText.text = "Endless Mode - Survive as long as you can!";
        _timerText.text = "";
    }

    public void ShowBossWave(int currentWave, int totalWaves)
    {
        _waveText.text = "Defeat the boss!";
        _waveText.color = _bossWaveColor;
        _waveText.fontSize = 30f;
        _timerText.text = "";
    }

    public void UpdateWaveTextForBoss(string waveText)
    {
        _waveText.text = waveText;
        _waveText.color = _bossWaveColor;
        _waveText.fontSize = 30f;
        _timerText.text = "";
    }

    public void HideTimer()
    {
        _timerText.text = "";
    }

    public void ResetWaveTextFormatting()
    {
        _waveText.color = _normalWaveColor;
        _waveText.fontSize = 30f;
    }
}
