using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Data", menuName = "Scriptable Objects/Tutorial Data")]
public class TutorialDataSO : ScriptableObject
{
    [field: SerializeField] public TutorialData[] Tutorials { get; private set; }
}

[System.Serializable]
public struct TutorialData
{
    public string _title;

    // New: multi-step tutorials (each step has its own image + description)
    public TutorialStep[] _steps;
}

[System.Serializable]
public struct TutorialStep
{
    [TextArea] public string _description;
    public Sprite _image;
}