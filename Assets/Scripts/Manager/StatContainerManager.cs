using System.Collections.Generic;
using UnityEngine;

public class StatContainerManager : MonoBehaviour
{
    public static StatContainerManager instance;
    [SerializeField] private StatContainer _statContainerPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void GenerateContainer(Dictionary<Stats, float> statsDictionary, Transform parent)
    {
        List<StatContainer> statContainers = new List<StatContainer>();

        foreach (KeyValuePair<Stats, float> kvp in statsDictionary)
        {
            StatContainer containerInstance = Instantiate(_statContainerPrefab, parent);
            statContainers.Add(containerInstance);

            Sprite statIcon = ResourceManager.GetStatsIcon(kvp.Key);
            string statName = Enums.FormatStateName(kvp.Key);
            float statValue = kvp.Value;

            containerInstance.Configure(statIcon, statName, statValue);
        }

        LeanTween.delayedCall(Time.deltaTime * 2, () => ResizeText(statContainers));
    }

    private void ResizeText(List<StatContainer> statContainers)
    {
        float minFontSize = 5000;

        for (int i = 0; i < statContainers.Count; i++)
        {
            StatContainer container = statContainers[i];
            float fontSize = container.GetFontSize();

            if (fontSize < minFontSize)
                minFontSize = fontSize;
        }

        for (int i = 0; i < statContainers.Count; i++)
        {
  
            statContainers[i].SetFontSize(minFontSize);
        }
    }

    public static void GenerateStatsContainer(Dictionary<Stats, float> statsDictionary, Transform parent)
    {
        parent.Clear();
        instance.GenerateContainer(statsDictionary, parent);
    }
}
