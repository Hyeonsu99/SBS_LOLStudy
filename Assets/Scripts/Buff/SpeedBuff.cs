using JetBrains.Annotations;
using UnityEngine;

public class SpeedBuff : Effect
{
    private float speedBonus;
    private ModType modType;

    public void Initialize(UnitStat target , float duration, ModType mod, float speedBonus)
    {
        this.speedBonus = speedBonus;
        EffectType = StringValue.SpeedBuff;
        modType = mod;
        base.Initialize(target, duration);
    }

    protected override void Apply()
    {
        // 곱연산 증가
        modifier = new StatModifier(EffectID, StatType.MoveSpeed, modType, speedBonus);
        targetStat.AddModifier(modifier);
        Debug.Log($"{EffectID}_이동속도 버프 적용 + {Duration}초 지속");
    }

    protected override void Remove()
    {
        if(targetStat != null)
        {
            targetStat.RemoveModifier(modifier);
        }
        
        Debug.Log($"{EffectID}__이동속도 버프 종료");
    }
}
