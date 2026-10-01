using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator _animator;
    [SerializeField] TextMeshPro _textMeshPro;
    [SerializeField] Color _damageColor = Color.white;
    [SerializeField] Color _criticalHitColor = Color.red;

    public void Animate(string damage, bool isCriticalHit)
    {
        _textMeshPro.text = damage.ToString();
        _textMeshPro.color = isCriticalHit ? _criticalHitColor : _damageColor;

        _animator.Play("animate");

    }
}
