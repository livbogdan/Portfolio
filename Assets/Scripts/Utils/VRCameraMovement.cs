using UnityEngine;

public class VRCameraMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _vrCamera;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 _followOffset = new Vector3(0f, 0f, 1.2f);
    [SerializeField, Range(0f, 20f)] private float _smoothSpeed = 8f;

    [Header("Movement Limits")]
    [SerializeField] private float _maxHorizontalOffset = 0.35f;
    [SerializeField] private float _maxVerticalOffset = 0.25f;
    [SerializeField] private float _minDepth = 0.4f;
    [SerializeField] private float _maxDepth = 2.5f;

    [Header("Look At")]
    [SerializeField] private bool _faceCamera = true;

    private Vector3 _currentOffset;

    private void Start()
    {
        if (_vrCamera == null)
        {
            _vrCamera = Camera.main;
        }

        if (_vrCamera == null)
        {
            Debug.LogWarning("No camera assigned to VRCameraMovement.");
            enabled = false;
            return;
        }

        _currentOffset = _followOffset;
    }

    private void LateUpdate()
    {
        if (_vrCamera == null)
        {
            return;
        }

        Vector3 targetPosition = _vrCamera.transform.position + GetClampedOffset();

        transform.position = Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);

        if (_faceCamera)
        {
            Vector3 lookDirection = transform.position - _vrCamera.transform.position;
            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _smoothSpeed * Time.deltaTime);
            }
        }
    }

    private Vector3 GetClampedOffset()
    {
        Vector3 cameraForward = _vrCamera.transform.forward;
        Vector3 cameraRight = _vrCamera.transform.right;
        Vector3 cameraUp = _vrCamera.transform.up;

        Vector3 desiredOffset = (cameraForward * _currentOffset.z) + (cameraRight * _currentOffset.x) + (cameraUp * _currentOffset.y);

        desiredOffset.x = Mathf.Clamp(desiredOffset.x, -_maxHorizontalOffset, _maxHorizontalOffset);
        desiredOffset.y = Mathf.Clamp(desiredOffset.y, -_maxVerticalOffset, _maxVerticalOffset);
        desiredOffset.z = Mathf.Clamp(desiredOffset.z, _minDepth, _maxDepth);

        return desiredOffset;
    }
}
