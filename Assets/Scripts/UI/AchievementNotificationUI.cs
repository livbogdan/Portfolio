using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AchievementNotificationUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI achievementNameText;
    [SerializeField] private TextMeshProUGUI achievementDescriptionText;
    [SerializeField] private Image achievementIcon;
    [SerializeField] private Animator notificationAnimator;
    
    [Header("Settings")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private AudioClip achievementSound;
    
    private AudioSource audioSource;
    private Coroutine hideCoroutine;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }
    
    private void OnEnable()
    {
        AchievementManager.OnAchievementUnlocked += ShowAchievementNotification;
    }
    
    private void OnDisable()
    {
        AchievementManager.OnAchievementUnlocked -= ShowAchievementNotification;
    }
    
    private void ShowAchievementNotification(string achievementId, string achievementName)
    {
        // Get achievement details from definitions
        var definitions = FindFirstObjectByType<AchievementDefinitions>();
        if (definitions != null)
        {
            var achievement = definitions.GetAchievement(achievementId);
            if (achievement != null)
            {
                DisplayNotification(achievement.name, achievement.description);
            }
            else
            {
                DisplayNotification(achievementName, "Achievement Unlocked!");
            }
        }
        else
        {
            DisplayNotification(achievementName, "Achievement Unlocked!");
        }
    }
    
    private void DisplayNotification(string name, string description)
    {
        // Update UI elements
        if (achievementNameText != null)
            achievementNameText.text = name;
            
        if (achievementDescriptionText != null)
            achievementDescriptionText.text = description;
        
        // Show notification panel
        if (notificationPanel != null)
            notificationPanel.SetActive(true);
        
        // Play animation if available
        if (notificationAnimator != null)
            notificationAnimator.SetTrigger("Show");
        
        // Play sound effect
        if (audioSource != null && achievementSound != null)
            audioSource.PlayOneShot(achievementSound);
        
        // Hide after duration
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }
    
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        
        // Play hide animation if available
        if (notificationAnimator != null)
            notificationAnimator.SetTrigger("Hide");
        
        // Wait for animation to complete
        yield return new WaitForSeconds(0.5f);
        
        // Hide notification panel
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }
    
    // Public method to manually hide notification
    public void HideNotification()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
            
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }
}
