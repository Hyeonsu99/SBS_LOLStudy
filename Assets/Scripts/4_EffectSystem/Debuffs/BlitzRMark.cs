using UnityEngine;

public class BlitzRMark : Effect
{
    private float _damageAmount;
    private UnitStat _ownerStat;

    public void Initialize(UnitStat stat, float duration, EffectType type, float value = 0, string customID = null)
    {
        _damageAmount = value;
        _ownerStat = GetComponent<UnitStat>();

        base.Initialize(stat, duration, type, customID);
    }

    protected override void Apply()
    {
        
    }

    protected override void Remove()
    {
        if (IsExpired && _targetStat != null)
        {
            var info = new DamageInfo
            {
                Attacker = _ownerStat.gameObject,
                Target = _targetStat.gameObject,
                RawDamage = _damageAmount,
                Type = DamageType.Magic,
                isCritical = false,        
                BonusDamage = 0,               
            };


            float finalDamage = DamageCalculater.CalculateFinalDamage(_ownerStat, _targetStat, info);

            _targetStat.TakeDamage(finalDamage, null);
        }
    }


}
