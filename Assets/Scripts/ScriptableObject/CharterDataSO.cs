using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Charter Data", menuName = "Scriptable Objects/Charter Data", order = 0)]
public class CharterDataSO : ScriptableObject
{
    [field: SerializeField] public string CharterName { get; private set; }
    [field: SerializeField] public string CharterDescription { get; private set; }
    [field: SerializeField] public int CharterPrice { get; private set; }
    [field: SerializeField] public bool ProgressionOnlyUnlock { get; private set; }
    [field: SerializeField] public CharacterUnlockRuleSO UnlockRule { get; private set; }
    [field: SerializeField] public CharacterUpgradeConfigSO UpgradeConfig { get; private set; }
    [field: SerializeField] public Sprite CharterSprite { get; private set; }
    [field: SerializeField] public MeshFilter CharacterMesh { get; private set; }
    [field: SerializeField] public MeshRenderer CharacterRenderer { get; private set; } 
    [field: SerializeField] public Material CharacterMaterial { get; private set; }

    [Header("Audio")]
    [field: SerializeField] public AudioClip TakeDamageClip { get; private set; }
    [field: SerializeField] public AudioClip DodgeClip { get; private set; }

    [field: Range(0, 4)]
    [field: SerializeField] public int Rarity { get; private set; }

    [HorizontalLine]
    [SerializeField] private float _attack;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _criticalChance;
    [SerializeField] private float _criticalPercent;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _Range;
    [SerializeField] private float _healthRecovery;
    [SerializeField] private float _armor;
    [SerializeField] private float _luck;
    [SerializeField] private float _dodge;
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
                { Stats.MaxHealth,              _maxHealth },
                { Stats.MoveSpeed,              _moveSpeed },
                { Stats.Range,                  _Range },
                { Stats.HealthRecoverySpeed,    _healthRecovery },
                { Stats.Armor,                  _armor },
                { Stats.Luck,                   _luck },
                { Stats.Dodge,                  _dodge },
                { Stats.LifeSteal,              _lifeSteal },
            };
        }
        private set
        {

        }
    }

    public Dictionary<Stats, float> NonNeutralStats
    {
        get
        {
            Dictionary<Stats, float> nonNeutralStats = new Dictionary<Stats, float>();

            foreach (KeyValuePair<Stats, float> kvp in BaseStats)
                if (kvp.Value != 0)
                    nonNeutralStats.Add(kvp.Key, kvp.Value);

            return nonNeutralStats;
        }
        private set {}
    }

}
