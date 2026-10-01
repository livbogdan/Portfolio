using UnityEngine;
using UnityEngine.Pool;

public class DamageTextManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] DamageText _damageTextPrefab;

    [Header("Pool")]
    private ObjectPool<DamageText> _damageTextPool;

    private void Awake()
    {
        Enemy._onDamageTaken += EnemyHitCallback;
        PlayerHealth.onAttackDodged += PlayerDodgedCallback;
    }


    private void OnDestroy()
    {
        Enemy._onDamageTaken -= EnemyHitCallback;
        PlayerHealth.onAttackDodged -= PlayerDodgedCallback;
    }
    void Start()
    {
        _damageTextPool = new ObjectPool<DamageText>(
            CreateFunction,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy);
    }

    private DamageText CreateFunction()
    {
        return Instantiate(_damageTextPrefab, transform);
    }

    private void ActionOnGet(DamageText damageText)
    {
        damageText.gameObject.SetActive(true);
    }

    private void ActionOnRelease(DamageText damageText)
    {
        if(damageText != null)
            damageText.gameObject.SetActive(false);
    }

    private void ActionOnDestroy(DamageText damageText)
    {
        Destroy(damageText.gameObject);
    }

    private void EnemyHitCallback(int damage, Vector3 enemyPos, bool isCriticalHit)
    {
        DamageText damageTextInstance = _damageTextPool.Get();
        Vector3 spawnPos = enemyPos + Vector3.up * 1.5f;
        damageTextInstance.transform.position = spawnPos;
        damageTextInstance.Animate(damage.ToString(), isCriticalHit);
        LeanTween.delayedCall(1f, () => _damageTextPool.Release(damageTextInstance));
    }

    private void PlayerDodgedCallback(Vector2 playerPosition)
    {
        DamageText damageTextInstance = _damageTextPool.Get();
        Vector3 spawnPos = playerPosition + Vector2.up * 1.5f;
        damageTextInstance.transform.position = spawnPos;
        damageTextInstance.Animate("DODGED", false);
        LeanTween.delayedCall(1f, () => _damageTextPool.Release(damageTextInstance));

    }

}