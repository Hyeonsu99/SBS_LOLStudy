using UnityEngine;

public interface IAttackProvider
{
    void DecorateDamage(ref DamageInfo info, UnitStat attacker, UnitStat target);
}
