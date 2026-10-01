using System;
using UnityEngine;
using UnityEngine.XR;

public class GripButtonUIToggle : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameObject _uiPanelToToggle;
    [SerializeField] private XRNode _controllerNode = XRNode.RightHand;
    [Tooltip("The game state during which the grip button toggle will be ignored (e.g., when the game is active)")]
    [SerializeField] private GameState[] _requiredGameState = { GameState.PAUSE };
    private InputDevice _controller;
    private bool _wasGripPressed = false;

    private void Awake()
    {
        _controller = InputDevices.GetDeviceAtXRNode(_controllerNode);
        
        if (!_controller.isValid)
            Debug.LogWarning($"Grip Button Toggle: No controller found at {_controllerNode}");
    }

    private void LateUpdate()
    {
        if (!_controller.isValid)
            _controller = InputDevices.GetDeviceAtXRNode(_controllerNode);

        bool gripPressed = false;
        _controller.TryGetFeatureValue(CommonUsages.gripButton, out gripPressed);

        // Toggle only on new press (not continuous)
        if (gripPressed && !_wasGripPressed)
        {
            HandleGripToggle();
        }

        _wasGripPressed = gripPressed;
    }

    private void HandleGripToggle()
    {
        // Ignore toggle if game is currently active
        if (GameManager.Instance != null && Array.IndexOf(_requiredGameState, GameManager.Instance.CurrentGameState) >= 0)
            return;

        if (_uiPanelToToggle != null)
        {
            // Toggle the panel on/off
            _uiPanelToToggle.SetActive(!_uiPanelToToggle.activeSelf);
        }
        else
        {
            Debug.LogWarning("Grip Button Toggle: Missing UI Panel reference");
        }
    }
}
