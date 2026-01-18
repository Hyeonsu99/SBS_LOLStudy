using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JhinQProjectile : MonoBehaviour
{
    private GameObject _owner;
    private UnitStat _ownerStat;
    private JhinQData _data;

    private float _levelBaseDamage;
    private float _levelAdRatio;
    private float _levelApRatio;
    private int _killCount;
    private int _currentBonusCount;
    private List<GameObject> _hitHistory = new();

    public void Initialize(GameObject owner, GameObject target, float baseDamage, float adRatio, float apRatio, JhinQData data)
    {
        _owner = owner;
        _ownerStat = owner.GetComponent<UnitStat>();
        _levelBaseDamage = baseDamage;
        _levelAdRatio = adRatio;
        _levelApRatio = apRatio;
        _data = data;

        StartCoroutine(BounceRoutine(target));
    }

    private IEnumerator BounceRoutine(GameObject target)
    {
        while(target != null && _currentBonusCount < _data.MaxBounces)
        {
            _currentBonusCount++;

            Vector3 startPos = transform.position;

            float elapsed = 0f;
            float duration = 0.4f;
            while (elapsed < duration)
            {
                if (target == null) break;

                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                Vector3 currentPos = Vector3.Lerp(startPos, target.transform.position, t);
                float height = Mathf.Sin(t * Mathf.PI) * 2f; // 2f는 곡선의 높이
                transform.position = currentPos + Vector3.up * height;

                yield return null;
            }

            if (target == null) break;

            float ownerAD = _ownerStat.Current.Get(StatType.AttackDamage);
            float ownerAP = _ownerStat.Current.Get(StatType.AbilityPower);

            float currentDamage = _levelBaseDamage + (ownerAD * _levelAdRatio) + (ownerAP * _levelApRatio);

            float multiplier = 1f + (_killCount * _data.KillBouns);
            float finalDamage = currentDamage * multiplier;

            Debug.Log($"Final Damage {finalDamage}");

            if(target.TryGetComponent(out UnitStat targetStat))
            {
                //float beforeHp = targetStat.CurrentHP;

                targetStat.TakeDamage(finalDamage, _owner);

                if(targetStat.CurrentHP <= 0)
                {
                    _killCount++;
                }
            }

            _hitHistory.Add(target);

            target = FindNextTarget(target);
            if (target == null) break;
        }

        Destroy(gameObject);
    }

    private GameObject FindNextTarget(GameObject currentTarget)
    {
        Collider[] colliders = Physics.OverlapSphere(currentTarget.transform.position, _data.BounceRange);
        GameObject bestTarget = null;
        float minDistance = float.MaxValue;

        foreach (var col in colliders)
        {
            // 이미 맞은 적 제외, 살아있는 적 탐색
            if (col.gameObject == currentTarget || _hitHistory.Contains(col.gameObject)) continue;

            if (col.TryGetComponent(out UnitStat stat) && stat.CurrentHP > 0)
            {
                float dist = Vector3.Distance(currentTarget.transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestTarget = col.gameObject;
                }
            }
        }
        return bestTarget;
    }
}
