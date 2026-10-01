using UnityEngine;

[CreateAssetMenu(fileName = "Color Palette", menuName = "Scriptable Objects/Color Palette", order = 0)]
public class ColorPaletteSO : ScriptableObject
{
   [field: SerializeField] public Color[] LevelColor { get; private set; }
   [field: SerializeField] public Color[] LevelOutColors { get; private set; }
   
   [Header("Rarity Colors")]
   [field: SerializeField] public Color[] RarityColors { get; private set; } = new Color[5];
   [field: SerializeField] public Color[] RarityOutlineColors { get; private set; } = new Color[5];
   
   public Color GetRarityColor(int rarity)
   {
       if (RarityColors != null && rarity >= 0 && rarity < RarityColors.Length)
           return RarityColors[rarity];
       return Color.white;
   }
   
   public Color GetRarityOutlineColor(int rarity)
   {
       if (RarityOutlineColors != null && rarity >= 0 && rarity < RarityOutlineColors.Length)
           return RarityOutlineColors[rarity];
       return Color.black;
   }
}
