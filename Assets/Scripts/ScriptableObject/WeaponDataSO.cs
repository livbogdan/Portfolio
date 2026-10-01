using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon Data", menuName = "Scriptable Objects/Weapon Data", order = 0)]
public class WeaponDataSO : ScriptableObject
{
    
    [field: SerializeField] public Weapon Prefab { get; private set; }
    [field: SerializeField] public string WeaponName { get; private set; }
    [field: SerializeField] public string WeaponDescription { get; private set; }
    [field: SerializeField] public Sprite WeaponSprite { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public int RecyclePrice { get; private set; }
    [field: SerializeField] public AudioClip AttackSound { get; private set; }
    [field: SerializeField] public AnimatorOverrideController AnimatorOverrideController { get; private set; }


    
    [SerializeField] private float _attack;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _criticalChance;
    [SerializeField] private float _criticalPercent;
    [SerializeField] private float _Range;
    [SerializeField] private float _lifeSteal;

    public Dictionary<Stats, float> BaseStats
    {
        get
        {
            return new Dictionary<Stats, float>
            {
                { Stats.Attack,                 _attack },
                { Stats.AttackSpeed,            _attackSpeed },
                { Stats.CriticalChance,         _criticalChance },
                { Stats.CriticalPercent,        _criticalPercent },
                { Stats.Range,                  _Range },
                { Stats.LifeSteal,              _lifeSteal },
            };
        }
        private set
        {

        }
    }
    
    public float GetStatsValue(Stats stats)
    {
        foreach (KeyValuePair<Stats, float> kvp in BaseStats)
        {
            if (kvp.Key == stats)
                return kvp.Value;
        }

        Debug.LogError("Stats not found: " + stats);

        return 0;
    }
}
