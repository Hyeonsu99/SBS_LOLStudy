using UnityEngine;

public class AttackSpeedBuff : Effect
{
    private float _attackSpeedBonus;
    private ModType _mod;

    public void Initialize(UnitStat stat, float duration, ModType mod, float attackSpeedBouns, EffectType type)
    {
        _attackSpeedBonus = attackSpeedBouns;
        _mod = mod;
        base.Initialize(stat, duration, type);
    }

    protected override void Apply()
    {
        _modifier = new StatModifier(EffectID, StatType.AttackSpeed, _mod, _attackSpeedBonus, ModifierType.Buff);
        _targetStat.AddModifier(_modifier);
    }

    protected override void Remove()
    {
        if (_targetStat != null)
        {
            _targetStat.RemoveModifier(_modifier);
        }
    }
}
