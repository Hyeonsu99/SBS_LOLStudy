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
        // 곱연산 증가
        _modifier = new StatModifier(EffectID, StatType.MoveSpeed, modType, speedBonus, ModifierType.Buff);
        _targetStat.AddModifier(_modifier);
        Debug.Log($"{EffectID}_이동속도 버프 적용 + {Duration}초 지속");
    }

    protected override void Remove()
    {
        if(_targetStat != null)
        {
            _targetStat.RemoveModifier(_modifier);
        }
        
        Debug.Log($"{EffectID}__이동속도 버프 종료");
        Debug.Log($"현재 이동속도 : {_targetStat.Current.Get(StatType.MoveSpeed)}");
    }
}
