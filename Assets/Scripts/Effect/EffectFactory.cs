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

    // 스탯 관련 디버프
    SlowDebuff,
    ArmorDebuff,
    
    // 특정 챔피언의 디버프
    JhinMark,

    // CC기 타입
    Root
}

public static class EffectFactory
{
    public static Effect Create(GameObject target, EffectType type, ModType mod, float duration, float value)
    {
        if (!target.TryGetComponent<UnitStat>(out var stat))
        {
            Debug.LogError("UnitStat 컴포넌트 참조 실패!!");
            return null;
        }

        switch (type)
        {
            case EffectType.AttackBuff:
                return CreateEffect<AttackBuff>(target, stat, type, mod, duration, value);
            case EffectType.SpeedBuff:
                return CreateEffect<SpeedBuff>(target, stat, type, mod, duration, value);
            case EffectType.JhinMark:
                break;
            case EffectType.Root:
                break;
            default:
                return null;        
        }

        return null;
    }

    private static T CreateEffect<T>(GameObject target, UnitStat stat, EffectType type, ModType mod, float duration, float value) where T : Effect
    {
        T effectComponent = target.AddComponent<T>();

        if (effectComponent is AttackBuff attack) attack.Initialize(stat, duration, mod, value, type);
        if (effectComponent is SpeedBuff speedBuff) speedBuff.Initialize(stat, duration, mod, value, type);

        return effectComponent;
    }
}
