using System.Collections.Generic;
using UnityEngine;

public static class WeaponStatsCalculator
{
    public static Dictionary<Stats, float> GetStats(WeaponDataSO weaponData, int level)
    {
        float multiplier = 1 + (float)level / 3;
        Dictionary<Stats, float> calculatedStats = new Dictionary<Stats, float>();

        foreach (KeyValuePair<Stats, float> kvp in weaponData.BaseStats)
        {
            if (weaponData.Prefab.GetType() != typeof(RangeWeapon) && kvp.Key == Stats.Range)
                calculatedStats.Add(kvp.Key, kvp.Value);
            else
                calculatedStats.Add(kvp.Key, kvp.Value * multiplier);
        }
        return calculatedStats;

    }

    public static int GetPurchasePrice(WeaponDataSO weaponData, int level)
    {
        float multiplier = 1 + (float)level / 3;
        return (int) (weaponData.Price * multiplier);
    }
    public static int GetRecyclePrice(WeaponDataSO weaponData, int level)
    {
        float multiplier = 1 + (float)level / 3;
        return (int) (weaponData.RecyclePrice * multiplier);
    }
}
