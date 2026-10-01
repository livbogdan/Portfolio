using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private float _shakeMagnitude = 0.1f;
    [SerializeField] private float _shakeDuration = 0.2f;

    private void Awake() => PlayerHealth.onTakeDamage += Shake;

    private void OnDestroy() => PlayerHealth.onTakeDamage -= Shake;

    private void Shake()
    {
        Vector2 direction = Random.onUnitSphere.With(z: 0).normalized;

        transform.localPosition = Vector3.zero;

        LeanTween.cancel(gameObject);
        LeanTween.moveLocal(gameObject, direction * _shakeMagnitude, _shakeDuration)
            .setEase(LeanTweenType.easeShake);
    }
}
