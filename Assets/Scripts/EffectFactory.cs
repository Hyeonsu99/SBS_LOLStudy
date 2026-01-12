using NUnit.Framework.Constraints;
using UnityEngine;

public enum EffectType
{
    AttackBuff,
    SpeedBuff,
    ArmorBuff,
    AttackSpeedBuff,
    AbilityPowerBuff,
    // 버프 타입 쭉 추가

    SlowDebuff,
    ArmorDebuff
    // 디버프 타입 쭉 추가
}

public static class EffectFactory
{
    public static Effect Create(GameObject target, EffectType type, float duration, float value)
    {
        UnitStat stat = target.GetComponent<UnitStat>();
        if (stat == null)
        {
            Debug.LogError("UnitStat 컴포넌트 참조 실패!!");
            return null;
        }

        Effect effect = null;

        switch (type)
        {
            case EffectType.AttackBuff:
                break;
            case EffectType.SpeedBuff:
                var speedBuff = target.AddComponent<SpeedBuff>();
                speedBuff.Initialize(stat, duration, value);
                effect = speedBuff;
                break;
        }

        return effect;
    }
}
