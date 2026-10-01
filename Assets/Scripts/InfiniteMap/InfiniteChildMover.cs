using UnityEngine;

public class InfiniteChildMover : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _mapFragmentsSize;
    [SerializeField] private float _distanceThreshold = 1.5f;

    void Update()
    {
        UpdateFragmentsPositions();
    }

    private void UpdateFragmentsPositions()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform fragment = transform.GetChild(i);

            Vector3 distance = _playerTransform.position - fragment.position;
            float calculatedDistanceFragment = _mapFragmentsSize * _distanceThreshold;

            if (Mathf.Abs(distance.x) > calculatedDistanceFragment)
            {
                fragment.position += Vector3.right * calculatedDistanceFragment * 2 * Mathf.Sign(distance.x);
            }
            if (Mathf.Abs(distance.y) > calculatedDistanceFragment)
            {
                fragment.position += Vector3.up * calculatedDistanceFragment * 2 * Mathf.Sign(distance.y);
            }
        }
    }
}
