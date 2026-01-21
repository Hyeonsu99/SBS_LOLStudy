using UnityEngine;

public class SlowDebuff : Effect
{
    private float _slowPercent;

    public void Initialize(UnitStat target, float duration, float slowPercent, EffectType type)
    {
        _slowPercent = slowPercent;

        base.Initialize(target, duration, type);
    }

    protected override void Apply()
    {
        modifier = new StatModifier(EffectID, StatType.MoveSpeed, ModType.PercentAdd, -_slowPercent, ModifierType.Debuff);
        targetStat.AddModifier(modifier);
    }

    protected override void Remove()
    {
        if(targetStat != null) targetStat.RemoveModifier(modifier);
    }
}
