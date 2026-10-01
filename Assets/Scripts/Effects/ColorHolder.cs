using UnityEngine;

public class ColorHolder : MonoBehaviour
{
    public static ColorHolder instance;
    [SerializeField] private ColorPaletteSO colorPaletteSO;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static Color GetColor(int level)
    {
        level = Mathf.Clamp(level, 0, instance.colorPaletteSO.LevelColor.Length);
        return instance.colorPaletteSO.LevelColor[level];
    }
    public static Color GetOutLineColor(int level)
    {
        level = Mathf.Clamp(level, 0, instance.colorPaletteSO.LevelOutColors.Length);
        return instance.colorPaletteSO.LevelOutColors[level];
    }
}
