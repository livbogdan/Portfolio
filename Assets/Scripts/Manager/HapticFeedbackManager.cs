using UnityEngine;
using UnityEngine.XR;

public class HapticFeedbackManager : MonoBehaviour
{   
    public static HapticFeedbackManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void TriggerDamageHaptic(float intensity = 0.5f, float duration = 0.2f)
    {
        Debug.Log($"[HapticFeedback] Triggering damage haptic - Intensity: {intensity}, Duration: {duration}");
        SendHapticToBothControllers(intensity, duration);
    }
    
    private void SendHapticToBothControllers(float intensity, float duration)
    {
        var leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        var rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        
        Debug.Log($"[HapticFeedback] Left Controller Valid: {leftController.isValid}, Right Controller Valid: {rightController.isValid}");
        
        if (leftController.isValid)
        {
            leftController.SendHapticImpulse(0, intensity, duration);
            Debug.Log("[HapticFeedback] Sent haptic to left controller");
        }
        else
        {
            Debug.LogWarning("[HapticFeedback] Left controller not valid");
        }
            
        if (rightController.isValid)
        {
            rightController.SendHapticImpulse(0, intensity, duration);
            Debug.Log("[HapticFeedback] Sent haptic to right controller");
        }
        else
        {
            Debug.LogWarning("[HapticFeedback] Right controller not valid");
        }
    }
}
