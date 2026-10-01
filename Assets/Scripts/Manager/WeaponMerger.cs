using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMerger : MonoBehaviour
{
    public static WeaponMerger instance;

    [SerializeField] private PlayerWeapon _playerWeapon;

    private List<Weapon> _weaponsToMerge = new List<Weapon>();

    public static Action<Weapon> _onMerge;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public bool CanMerge(Weapon weapon)
    {
        if (weapon.Level >= 3)
            return false;

        _weaponsToMerge.Clear();
        _weaponsToMerge.Add(weapon);

        Weapon[] weapons = _playerWeapon.GetWeaponList();

        foreach (Weapon playerWeapon in weapons)
        {
            if (playerWeapon == null)
                continue;

            if (playerWeapon == weapon)
                continue;

            if (playerWeapon.WeaponData.WeaponName != weapon.WeaponData.WeaponName)
                continue;

            if (playerWeapon.Level != weapon.Level)
                continue;

            _weaponsToMerge.Add(playerWeapon);
            return true;
        }
        return false;
    }

    public void Merge()
    {
        if (_weaponsToMerge.Count < 2)
        {
            return;
        }

        DestroyImmediate(_weaponsToMerge[1].gameObject);

        _weaponsToMerge[0].Upgrade();

        Weapon weapon = _weaponsToMerge[0];
        _weaponsToMerge.Clear();

        _onMerge?.Invoke(weapon);
    }
}
