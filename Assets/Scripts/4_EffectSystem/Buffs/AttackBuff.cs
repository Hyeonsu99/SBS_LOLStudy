using UnityEngine;

public class AttackBuff : Effect
{
    private float attackBouns;
    private ModType modType;

    public void Initialize(UnitStat target, float duration, ModType mod, float attackBouns, EffectType type)
    {
        this.attackBouns = attackBouns;
        modType = mod;
        base.Initialize(target, duration, type);
    }

    protected override void Apply()
    {
        modifier = new StatModifier(EffectID, StatType.AttackDamage, modType, attackBouns, ModifierType.Buff);
        targetStat.AddModifier(modifier);
        Debug.Log($"{EffectID}_공격력 버프 적용 + {Duration}초 지속");
    }

    protected override void Remove()
    {
        if (targetStat != null)
        {
            targetStat.RemoveModifier(modifier);
        }

        Debug.Log($"{EffectID}_공격력 버프 종료");
    }
}
