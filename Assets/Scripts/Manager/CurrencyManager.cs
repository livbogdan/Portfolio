using System;
using UnityEngine;
using NaughtyAttributes;

public class CurrencyManager : MonoBehaviour, IGameStateListner
{
    public static CurrencyManager instance;

    private const string gemCurrency = "GemCurrency";

    [field: SerializeField] public int Currency { get; private set; }
    [field: SerializeField] public int GemCurrency { get; private set; }

    [Header("Actions")]
    public static Action<int> OnCurrencyChanged;
    
    // Store last conversion info for GameOverConversionDisplay
    public int LastConvertedGold { get; private set; }
    public int LastConvertedGems { get; private set; }

    [SerializeField] private int _debugCurrency;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        Gem._onCollected += GemCollectedCallback;
        Cash._onCollected += CashCollectedCallback;
    }



    private void OnDestroy()
    {
        Gem._onCollected -= GemCollectedCallback;
        Cash._onCollected -= CashCollectedCallback;
    }
    

    [Button("Add Currency")]
    private void AddCurrencyDebug() => AddCurrency(_debugCurrency);
    
    [Button("Add Gem Currency")]
    private void AddGemCurrencyDebug() => AddGemCurrency(_debugCurrency);
    
    public void AddCurrency(int recyclePrice)
    {
        Currency += recyclePrice;
        UpdateVisual();
    }

    public void AddGemCurrency(int amount, bool save = true)
    {
        GemCurrency += amount;
        UpdateVisual();
    }
    void UpdateVisual()
    {
        UppdateText();
        OnCurrencyChanged?.Invoke(Currency);
    }

    private void UppdateText()
    {
        CurrencyText[] currencyTexts = FindObjectsByType<CurrencyText>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (CurrencyText currencyText in currencyTexts)
            currencyText.UppdateText(Currency);

        GemCurrencyText[] gemCurrencyTexts = FindObjectsByType<GemCurrencyText>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (GemCurrencyText gemCurrencyText in gemCurrencyTexts)
            gemCurrencyText.UppdateText(GemCurrency);
    }

    public bool HasEnoughtCurrency(int price) => Currency >= price;
    public bool HasEnoughtGemCurrency(int price) => GemCurrency >= price;

    public void UseCurrency(int price) => AddCurrency(-price);
    public void UseGemCurrency(int price) => AddGemCurrency(-price);
    
    /// <summary>
    /// Converts leftover gold to gems at the end of a run. 
    /// Conversion rate: 100 gold = 1 gem (adjustable)
    /// </summary>
    public int ConvertGoldToGemsOnRunEnd()
    {
        const float GOLD_TO_GEM_RATIO = 100f; // 100 gold = 1 gem
        
        // Store values for display before conversion
        LastConvertedGold = Currency;
        LastConvertedGems = Mathf.FloorToInt(Currency / GOLD_TO_GEM_RATIO);
        
        if (LastConvertedGems > 0)
        {
            AddGemCurrency(LastConvertedGems);
            Debug.Log($"Run ended. Converted {LastConvertedGold} gold to {LastConvertedGems} gems");
        }
        
        return LastConvertedGems;
    }
    
    /// <summary>
    /// Resets the run currency (gold) to 0. Called at the start of a new run.
    /// </summary>
    public void ResetRunCurrency()
    {
        Currency = 0;
        UpdateVisual();
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.SELECTMODE)
            ResetRunCurrency();
    }

    private void GemCollectedCallback(Gem gem) => AddGemCurrency(1);
    private void CashCollectedCallback(Cash cash) => AddCurrency(1);

}
