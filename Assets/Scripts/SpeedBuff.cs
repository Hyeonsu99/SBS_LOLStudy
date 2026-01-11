using JetBrains.Annotations;
using UnityEngine;

public class SpeedBuff : Effect
{
    private float speedBonus;

    public void Initialize(PlayerStat target , float duration, float speedBonus)
    {
        this.speedBonus = speedBonus;
        EffectID = "Speed_Buff";
        base.Initialize(target, duration);
    }

    protected override void Apply()
    {
        modifier = new StatModifier(EffectID, StatType.MoveSpeed, ModType.Add, speedBonus);
        targetStat.AddModifier(modifier);
        Debug.Log($"{EffectID}_이동속도 버프 적용 + {Duration}초 지속");
    }

    protected override void Remove()
    {
        targetStat.RemoveModifier(modifier);
        Debug.Log($"{EffectID}__이동속도 버프 종료");
    }
}
