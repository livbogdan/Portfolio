using UnityEngine;

[CreateAssetMenu(fileName = "Stat Icon", menuName = "Scriptable Objects/Stats Icon")]
public class StatIconDataSO : ScriptableObject
{
    [field: SerializeField] public StatIconData[] StatIcons { get; private set; }
}

[System.Serializable]
public struct StatIconData
{
    public Stats _stat;
    public Sprite _icon;
}