using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Elements")]
    public WeaponPosition[] _weaponPositions;

    public bool TryAddWeapon(WeaponDataSO weaponData, int weaponLevel)
    {

        for (int i = 0; i < _weaponPositions.Length; i++)
        {
            if (_weaponPositions[i].Weapon != null)
                continue;

            _weaponPositions[i].AssignWeapon(weaponData.Prefab, weaponLevel);
            return true;
        }
        return false;
    }

    public Weapon[] GetWeaponList()
    {
        List<Weapon> weapons = new List<Weapon>();

        foreach (WeaponPosition weaponPosition in _weaponPositions)
        {
            if (weaponPosition.Weapon == null)
                weapons.Add(null);
            else
                weapons.Add(weaponPosition.Weapon);
        }
        
        return weapons.ToArray();
    }

    public void ClearAllWeapons()
    {
        for (int i = 0; i < _weaponPositions.Length; i++)
        {
            if (_weaponPositions[i].Weapon != null)
                _weaponPositions[i].RecycleWeapon();
        }
    }

    public void RecycleWeapon(int weaponIndex)
    {
        for (int i = 0; i < _weaponPositions.Length; i++)
        {
            if (i != weaponIndex)
                continue;

            int recyclePrice = _weaponPositions[i].Weapon.GetRecyclePrice();
            CurrencyManager.instance.AddCurrency(recyclePrice);

            _weaponPositions[i].RecycleWeapon();

            return;
        }

    }
}