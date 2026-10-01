using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object Data", menuName = "Scriptable Objects/New Object", order = 0)]
public class ObjectDataSO : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public int RecyclePrice { get; private set; }


    [field: Range(0, 3)]
    [field: SerializeField] public int Rarity { get; private set; }

    [Tooltip("Make a StatData array and add all the stats here")]
    [SerializeField] private StatData[] _stats;

    public Dictionary<Stats, float> BaseStats
    {
        get
        {
            Dictionary<Stats, float> baseStats = new Dictionary<Stats, float>();

            foreach (StatData stat in _stats)
            {
                baseStats.Add(stat._stats, stat._value);
            }

            return baseStats;
        }
        private set
        {}
    }
}

[System.Serializable]
public struct StatData
{
    public Stats _stats;
    public float _value;
}
