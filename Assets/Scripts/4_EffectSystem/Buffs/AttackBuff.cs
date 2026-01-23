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
        _modifier = new StatModifier(EffectID, StatType.AttackDamage, modType, attackBouns, ModifierType.Buff);
        _targetStat.AddModifier(_modifier);
        Debug.Log($"{EffectID}_공격력 버프 적용 + {Duration}초 지속");
    }

    protected override void Remove()
    {
        if (_targetStat != null)
        {
            _targetStat.RemoveModifier(_modifier);
        }

        Debug.Log($"{EffectID}_공격력 버프 종료");
    }
}
