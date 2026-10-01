using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStatsManager))]
public class PlayerObject : MonoBehaviour
{
    [field: SerializeField] public List<ObjectDataSO> PlayerObjects { get; private set; }
    private PlayerStatsManager _playerStatsManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _playerStatsManager = GetComponent<PlayerStatsManager>();
    }
    void Start()
    {
        foreach (ObjectDataSO objectDataSO in PlayerObjects)
            _playerStatsManager.AddObject(objectDataSO.BaseStats);

    }

    public void AddObject(ObjectDataSO objectData)
    {
        PlayerObjects.Add(objectData);
        _playerStatsManager.AddObject(objectData.BaseStats);
    }

    public void RecycleObject(ObjectDataSO objectDataToRecycle)
    {
        PlayerObjects.Remove(objectDataToRecycle);
        CurrencyManager.instance.AddCurrency(objectDataToRecycle.RecyclePrice);
        _playerStatsManager.RemoveObjectStats(objectDataToRecycle.BaseStats);
    }
}
