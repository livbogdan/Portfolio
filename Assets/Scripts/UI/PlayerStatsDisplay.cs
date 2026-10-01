using System;
using UnityEngine;

public class PlayerStatsDisplay : MonoBehaviour, IPlayerStatsDependency
{
    [SerializeField] private Transform _statContainerParent;

    public void UpdateStats(PlayerStatsManager playerStatsManager)
    {
        int index = 0;

        foreach (Stats stats in Enum.GetValues(typeof(Stats)))
        {
             if (index >= _statContainerParent.childCount)
            {
                //Debug.LogWarning($"Not enough StatContainer children for stat: {stats}. Expected at least {index + 1} children, but only found {_statContainerParent.childCount}");
                break;
            }

            StatContainer statContainer = _statContainerParent.GetChild(index).GetComponent<StatContainer>();
            statContainer.gameObject.SetActive(true);

            if (statContainer == null)
            {
                //Debug.LogError($"Child at index {index} doesn't have StatContainer component!");
                index++;
                continue;
            }

            statContainer.gameObject.SetActive(true);

            Sprite statIcon = ResourceManager.GetStatsIcon(stats);

            float statValue = playerStatsManager.GetStatsValue(stats);

            statContainer.Configure(statIcon, Enums.FormatStateName(stats), statValue, true);

            index++;
        }

        for (int i = index; i < _statContainerParent.childCount; i++)
        {
            _statContainerParent.GetChild(i).gameObject.SetActive(false);
        }
    }
}
