using UnityEngine;

public class FollowVRCamera : MonoBehaviour
{
    [SerializeField] private float verticalOffset = 0f; // Offset from camera eye height
    [SerializeField] private string cameraTag = "MainCamera"; // Tag to find the camera
    [SerializeField] private RectTransform excludedChild; // Child UI to ignore
    [SerializeField] private float smoothSpeed = 5f; // Smoothing speed (higher = faster)
    private Transform cameraTransform;
    private Vector3 initialPosition;
    private Vector3 excludedChildWorldPos;

    void Start()
    {
        // Find camera by tag across all scenes
        GameObject cameraObj = GameObject.FindWithTag(cameraTag);
        if (cameraObj != null)
        {
            cameraTransform = cameraObj.transform;
        }
        else
        {
            Debug.LogError($"Camera with tag '{cameraTag}' not found!");
        }
        
        initialPosition = transform.position;
        
        if (excludedChild != null)
        {
            excludedChildWorldPos = excludedChild.position;
        }
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            Vector3 targetPosition = transform.position;
            targetPosition.y = cameraTransform.position.y + verticalOffset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            if (excludedChild != null)
            {
                excludedChild.position = excludedChildWorldPos;
            }
        }
    }
}
