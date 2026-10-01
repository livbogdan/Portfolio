public enum GameState
{
    MENU,
    GAME,
    WAVETRANSITION,
    SHOP,
    WEAPONSELECTION,
    GAMEOVER,
    STAGECOMPLETE,
    GAMEPLAY,
    CHARACHTERSELECTION,
    UPGRADES,
    IAPSHOP,
    TUTORIAL,
    SETTINGS,
    SELECTMODE,
    PAUSE,
    REWARDS,
}

public enum MusicCategory
{
    None,
    Menu,
    Game,
    Shop,
    GameOver,
    StageComplete
}

public enum Stats
{

    Attack,
    AttackSpeed,
    CriticalChance,
    CriticalPercent,
    MoveSpeed,
    MaxHealth,
    Range,
    HealthRecoverySpeed,
    Armor,
    Luck,
    Dodge,
    LifeSteal

}

public static class Enums
{
    public static string FormatStateName(Stats playerStats)
    {
        string formatted = "";
        string unformattedString = playerStats.ToString();

        if(unformattedString.Length <=0)
            return "Unvalued name";

        formatted += unformattedString[0];

        for (int i = 1; i < unformattedString.Length; i++)
        {
            if (char.IsUpper(unformattedString[i]))
                formatted += " ";
            
            formatted += unformattedString[i];
        }

        return formatted;
    }
}