using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

/// <summary>
/// Controls camera behavior for VR/2D gameplay. Handles target following with boundary constraints,
/// orthographic projection for mixed reality, and dynamic camera size adjustments based on screen plane.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _screenPlane;
    [SerializeField] private Camera _planeCamera;

    [Header("Camera Follow Settings")]
    [SerializeField] private Vector2 _minMaxXY;

    [Header("Orthographic Camera Settings")]
    [SerializeField] private bool _isXR;
    [SerializeField] private float _baseOrthoSize = 5f;
    [SerializeField] private int _questRenderFps = 30;

    private Camera _camera;
    private Coroutine _renderLoopCoroutine;

    private void Awake()
    {
        _camera = _planeCamera;
        if (_camera == null)
        {
            Debug.LogError("No Camera component found on " + gameObject.name + ". Please add a Camera component.");
            return;
        }
        
        if (_isXR)
        {
            _camera.orthographic = true;
            UpdateCameraSize();
        }

        if (ShouldThrottlePlaneCamera())
        {
            _camera.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (ShouldThrottlePlaneCamera() && _renderLoopCoroutine == null)
        {
            _renderLoopCoroutine = StartCoroutine(RenderPlaneCameraLoop());
        }
    }

    private void OnDisable()
    {
        if (_renderLoopCoroutine != null)
        {
            StopCoroutine(_renderLoopCoroutine);
            _renderLoopCoroutine = null;
        }
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            Debug.LogWarning("Target is null");
            return;
        }

        Vector3 targetPosition = _target.position;
        targetPosition.z = 0;

        if(!GameManager.Instance.UseInfiniteMap)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, -_minMaxXY.x, _minMaxXY.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, -_minMaxXY.y, _minMaxXY.y);
        }

        transform.position = targetPosition;
    }

    private void UpdateCameraSize()
    {
        if (_screenPlane != null && _camera != null)
        {
            float planeAspect = _screenPlane.localScale.x / _screenPlane.localScale.y;
            _camera.orthographicSize = _baseOrthoSize * (1f / planeAspect);
        }
    }

    private bool ShouldThrottlePlaneCamera()
    {
        return _camera != null
            && _camera.targetTexture != null
            && Application.platform == RuntimePlatform.Android
            && XRSettings.isDeviceActive;
    }

    private IEnumerator RenderPlaneCameraLoop()
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1f / Mathf.Max(1, _questRenderFps));

        while (true)
        {
            if (_camera != null)
            {
                _camera.Render();
            }

            yield return wait;
        }
    }

    // Public method to toggle between orthographic and perspective modes
    public void SetOrthographicMode(bool orthographic)
    {
        _isXR = orthographic;
        if (_camera != null)
        {
            _camera.orthographic = orthographic;
            if (orthographic)
            {
                UpdateCameraSize();
            }
        }
    }
    
    // Public method to update screen plane reference at runtime
    public void SetScreenPlane(Transform screenPlane)
    {
        _screenPlane = screenPlane;
        if (_isXR)
        {
            UpdateCameraSize();
        }
    }
}
