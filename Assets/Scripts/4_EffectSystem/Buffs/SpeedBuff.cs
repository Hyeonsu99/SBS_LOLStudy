using JetBrains.Annotations;
using UnityEngine;

public class SpeedBuff : Effect
{
    private float speedBonus;
    private ModType modType;

    public void Initialize(UnitStat target , float duration, ModType mod, float speedBonus, EffectType type)
    {
        this.speedBonus = speedBonus;
        modType = mod;
        base.Initialize(target, duration, type);
    }

    protected override void Apply()
    {
        // °ö¿¬»ê Áõ°¡
        _modifier = new StatModifier(EffectID, StatType.MoveSpeed, modType, speedBonus, ModifierType.Buff);
        _targetStat.AddModifier(_modifier);
    }

    protected override void Remove()
    {
        if(_targetStat != null)
        {
            _targetStat.RemoveModifier(_modifier);
        }      
    }
}
