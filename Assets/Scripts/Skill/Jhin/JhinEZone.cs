using System.Collections.Generic;
using UnityEngine;

public class JhinEZone : MonoBehaviour
{
    private GameObject _owner;
    private JhinEData _data;
    private int _skillLevel;

    private HashSet<UnitStat> _targetsInside = new HashSet<UnitStat>();

    public void Initialize(GameObject owner, JhinEData data, int level)
    {
        _owner = owner;
        _data = data;
        _skillLevel = level;
        Invoke(nameof(Explode), _data.ExplosionDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitIdentity targetId) && other.TryGetComponent(out UnitStat stat))
        {
            var ownerId = _owner.GetComponent<UnitIdentity>();
            if (!ownerId.IsEnemy(targetId)) return;

            stat.ApplyEffect(EffectType.SlowDebuff, ModType.PercentAdd, 10f, _data.SlowPercentage);
            _targetsInside.Add(stat);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out UnitStat stat))
        {
            if (_targetsInside.Contains(stat))
            {
                stat.RemoveEffect(EffectType.SlowDebuff);
                _targetsInside.Remove(stat);
            }
        }
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2.6f);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out UnitIdentity targetId) && col.TryGetComponent(out UnitStat targetStat))
            {
                var ownerId = _owner.GetComponent<UnitIdentity>();
                if (!ownerId.IsEnemy(targetId)) continue;

                ApplyDamage(targetStat, targetId.Type);
                targetStat.RemoveEffect(EffectType.SlowDebuff);
            }
        }
        Destroy(gameObject);
    }

    private void ApplyDamage(UnitStat targetStat, UnitType unitType)
    {
        int idx = Mathf.Clamp(_skillLevel - 1, 0, _data.BaseDamages.Length - 1);
        float baseDmg = _data.BaseDamages[idx];

        var ownerStat = _owner.GetComponent<UnitStat>();
        float ad = ownerStat.Current.Get(StatType.AttackDamage);
        float ap = ownerStat.Current.Get(StatType.AbilityPower);

        float finalDamage = baseDmg + (ad * _data.AdRatio) + (ap * _data.ApRatio);

        if (unitType == UnitType.Minion) finalDamage *= _data.MinionDamageRatio;

        var info = new DamageInfo
        {
            Attacker = _owner,
            Target = targetStat.gameObject,
            RawDamage = finalDamage,
            Type = DamageType.Magic,
            BonusDamage = 0,
            isCritical = false,
        };

        finalDamage = DamageCalculater.CalculateFinalDamage(ownerStat, targetStat, info);

        targetStat.TakeDamage(finalDamage, _owner);
    }
}
