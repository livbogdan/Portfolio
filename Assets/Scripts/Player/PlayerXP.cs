using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerXP : MonoBehaviour, IGameStateListner
{
    [Header("Player Stats")]
    [SerializeField] private int _maxXP;
    private int _currentXP;
    private int _requireXP;
    private int _level;
    private int _levelsEared;

    [Header("UI")]
    [SerializeField] private Slider _XPBar;
    [SerializeField] private TextMeshProUGUI _XPText;

    //[Header("Debug")]
    //[SerializeField] private bool _DEBUG;
    //[SerializeField] private int _xpPerKill = 1;

    private void Awake()
    {
        Enemy._onEnemyKilledWithXP += EnemyKilledWithXPCallback;
    }

    private void OnDestroy()
    {
        Enemy._onEnemyKilledWithXP -= EnemyKilledWithXPCallback;
    }

    void Start()
    {
        UpdateRequireXP();
        UpdateLevelUI();
    }

    private void UpdateRequireXP()
    {
        _requireXP = (_level + 1) * 5;
    }
    private void UpdateLevelUI()
    {
        _XPBar.value = (float) _currentXP / _requireXP;
        _XPText.text = "Level " + (_level +1);
    }

    private void EnemyKilledWithXPCallback(int xpAmount)
    {
        _currentXP += xpAmount;

        if (_currentXP >= _requireXP)
            LevelUp();
        UpdateLevelUI();
    }

    private void LevelUp()
    {
        _level++;
        _currentXP = 0;
        _levelsEared++;
        UpdateRequireXP();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.WEAPONSELECTION || gameState == GameState.GAMEOVER)
        {
            _level = 0;
            _currentXP = 0;
            _levelsEared = 0;
            UpdateRequireXP();
            UpdateLevelUI();
        }
    }

    public bool HasLeveledUp()
    {
        // DEBUG: Only skip shop on certain waves if needed
        //if (_DEBUG)
        //    return true;

        if (_levelsEared > 0)
        {
            _levelsEared--;
            return true;
        }

        return false;
    }
}
