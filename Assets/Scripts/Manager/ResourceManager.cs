using UnityEngine;

public static class ResourceManager
{
    const string statIconsPath = "Data/Stat Icon";
    const string objectDataPath = "Data/Objects Data/";
    const string weaponDataPath = "Data/Weapon Data/";
    const string characterDataPath = "Data/Characters Data/";

    private static StatIconData[] _statIcons;

    public static Sprite GetStatsIcon(Stats stat)
    {
        if (_statIcons == null)
        {
            StatIconDataSO data = Resources.Load<StatIconDataSO>(statIconsPath);
            _statIcons = data.StatIcons;
        }

        foreach (StatIconData statIcon in _statIcons)
        {
            if (stat == statIcon._stat)
                return statIcon._icon;
        }

        Debug.LogError("Stat Icon not found for stat: " + stat);

        return null;
    }

    private static ObjectDataSO[] _objectDatas;
    public static ObjectDataSO[] Objects
    {
        get
        {
            if (_objectDatas == null)

                _objectDatas = Resources.LoadAll<ObjectDataSO>(objectDataPath);

            return _objectDatas;

        }

        private set { }
    }

    public static ObjectDataSO GetRandomObjectData()
    {
        return Objects[Random.Range(0, Objects.Length)];
    }


    private static WeaponDataSO[] _weaoponDatas;
    public static WeaponDataSO[] Weapons
    {
        get
        {
            if (_weaoponDatas == null)

                _weaoponDatas = Resources.LoadAll<WeaponDataSO>(weaponDataPath);

            return _weaoponDatas;

        }

        private set { }
    }

    public static WeaponDataSO GetRandomWeaponData()
    {
        return Weapons[Random.Range(0, Weapons.Length)];
    }

    private static CharterDataSO[] _characterDatas;
    public static CharterDataSO[] Character
    {
        get
        {
            if (_characterDatas == null)

                _characterDatas = Resources.LoadAll<CharterDataSO>(characterDataPath);

            return _characterDatas;

        }

        private set { }
    }
}
