using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Populates the left list with tutorial buttons and shows the selected tutorial
/// (image + description) on the right. Supports Next/Prev navigation.
/// </summary>
public class TutorialUIController : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] private TutorialDataSO _data;

    [Header("List (Left)")]
    [SerializeField] private RectTransform _contentParent;      // ScrollView/Viewport/Content
    [SerializeField] private Button _buttonTemplate;             // Disabled template child under Content

    [Header("Details (Right)")]
    [SerializeField] private Image _detailImage;
    [SerializeField] private TMP_Text _detailTitle;
    [SerializeField] private TMP_Text _detailDescription;
    [SerializeField] private GameObject _detailPanel;            // Optional: panel to toggle on select

    [Header("Navigation")]
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TMP_Text _pageIndicator;            // e.g. "12 / 465" (optional)

    private readonly List<Button> _spawnedButtons = new();
    private int _currentTutorialIndex = -1;
    private int _currentStepIndex = -1;

    private void Awake()
    {
        if (_buttonTemplate != null)
            _buttonTemplate.gameObject.SetActive(false); // keep template hidden

        // Wire navigation
        if (_prevButton) _prevButton.onClick.AddListener(ShowPrevious);
        if (_nextButton) _nextButton.onClick.AddListener(ShowNext);
    }

    private void OnEnable()
    {
        BuildList();
        // Auto-select first item if any
        if (_data != null && _data.Tutorials != null && _data.Tutorials.Length > 0)
        {
            ShowTutorial(0);
        }
        else
        {
            ClearDetails();
        }
    }

    /// <summary>
    /// Creates one button per tutorial entry under Content.
    /// </summary>
    public void BuildList()
    {
        // Cleanup existing spawned buttons (keep template)
        foreach (var button in _spawnedButtons)
        {
            if (button) Destroy(button.gameObject);
        }
        _spawnedButtons.Clear();

        if (_data == null || _data.Tutorials == null || _data.Tutorials.Length == 0)
            return;

        for (int i = 0; i < _data.Tutorials.Length; i++)
        {
            var tutorial = _data.Tutorials[i];

            var button = Instantiate(_buttonTemplate, _contentParent);
            button.gameObject.name = $"Button_{i:00}_{tutorial._title}";
            button.gameObject.SetActive(true);

            // Set text label if present (TextMeshPro preferred)
            var text = button.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = tutorial._title;
            else
            {
                var legacy = button.GetComponentInChildren<Text>();
                if (legacy != null) legacy.text = tutorial._title;
            }

            int captured = i; // capture for lambda
            button.onClick.AddListener(() => ShowTutorial(captured));
            _spawnedButtons.Add(button);
        }
    }

    public void ShowPrevious()
    {
        if (!HasData() || _currentTutorialIndex < 0) return;

        var steps = GetSteps(_data.Tutorials[_currentTutorialIndex]);
        if (steps.Length == 0) return;

        int newStep = Mathf.Clamp(_currentStepIndex - 1, 0, steps.Length - 1);
        ShowStep(newStep);
    }

    public void ShowNext()
    {
        if (!HasData() || _currentTutorialIndex < 0) return;

        var steps = GetSteps(_data.Tutorials[_currentTutorialIndex]);
        if (steps.Length == 0) return;

        int newStep = Mathf.Clamp(_currentStepIndex + 1, 0, steps.Length - 1);
        ShowStep(newStep);
    }

    /// <summary>
    /// Displays selected tutorial in the right panel.
    /// </summary>
    public void ShowTutorial(int index)
    {
        if (!HasData())
        {
            ClearDetails();
            return;
        }
        index = Mathf.Clamp(index, 0, _data.Tutorials.Length - 1);
        _currentTutorialIndex = index;
        _currentStepIndex = 0; // reset to first step when selecting a tutorial

        var tutorialData = _data.Tutorials[index];
        if (_detailTitle) _detailTitle.text = tutorialData._title;

        if (_detailPanel) _detailPanel.SetActive(true);
        ShowCurrentStep();
    }

    private void ClearDetails()
    {
        _currentTutorialIndex = -1;
        _currentStepIndex = -1;
        if (_detailTitle) _detailTitle.text = string.Empty;
        if (_detailDescription) _detailDescription.text = string.Empty;
        if (_detailImage)
        {
            _detailImage.sprite = null;
            _detailImage.enabled = false;
        }
        if (_pageIndicator) _pageIndicator.text = string.Empty;
        if (_detailPanel) _detailPanel.SetActive(false);

        if (_prevButton) _prevButton.interactable = false;
        if (_nextButton) _nextButton.interactable = false;
    }

    private bool HasData()
    {
        return _data != null && _data.Tutorials != null && _data.Tutorials.Length > 0;
    }

    private TutorialStep[] GetSteps(TutorialData tutorialData)
    {
        // Use multi-step data only
        return tutorialData._steps ?? System.Array.Empty<TutorialStep>();
    }

    private void ShowCurrentStep()
    {
        if (!HasData() || _currentTutorialIndex < 0) { ClearDetails(); return; }
        var tutorialData = _data.Tutorials[_currentTutorialIndex];
        var steps = GetSteps(tutorialData);
        if (steps == null || steps.Length == 0) { ClearDetails(); return; }

        _currentStepIndex = Mathf.Clamp(_currentStepIndex, 0, steps.Length - 1);
        var step = steps[_currentStepIndex];

        if (_detailDescription) _detailDescription.text = step._description ?? string.Empty;
        if (_detailImage)
        {
            _detailImage.sprite = step._image;
            _detailImage.enabled = step._image != null;
        }

        // Update nav states (within tutorial steps)
        if (_prevButton) _prevButton.interactable = _currentStepIndex > 0;
        if (_nextButton) _nextButton.interactable = _currentStepIndex < steps.Length - 1;
        if (_pageIndicator) _pageIndicator.text = $"{_currentStepIndex + 1} / {steps.Length}";
    }

    private void ShowStep(int stepIndex)
    {
        _currentStepIndex = stepIndex;
        ShowCurrentStep();
    }
}
