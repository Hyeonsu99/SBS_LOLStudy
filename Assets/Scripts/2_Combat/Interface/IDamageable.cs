using UnityEngine;

// 데미지를 입을 수 있는 모든 존재
public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(float damage, GameObject attacker);
    UnitStat Stat { get; }
}
