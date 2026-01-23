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
            case EffectType.AttackSpeedBuff:
                return CreateEffect<AttackSpeedBuff>(target, stat, type, mod, duration, value);
            case EffectType.JhinMark:
                return CreateEffect<JhinMarkDebuff>(target, stat, type, mod, duration, value);
            case EffectType.Root:
                return CreateEffect<RootDebuff>(target, stat, type, mod, duration, value);
            case EffectType.SlowDebuff:
                return CreateEffect<SlowDebuff>(target, stat, type, mod, duration, value);
            default:
                return null;        
        }
    }

    private static T CreateEffect<T>(GameObject target, UnitStat stat, EffectType type, ModType mod, float duration, float value) where T : Effect
    {
        T effectComponent = target.AddComponent<T>();

        if (effectComponent is AttackBuff attack) attack.Initialize(stat, duration, mod, value, type);
        if (effectComponent is SpeedBuff speedBuff) speedBuff.Initialize(stat, duration, mod, value, type);
        if (effectComponent is JhinMarkDebuff jhinMark) jhinMark.Initialize(stat, duration, type);
        if (effectComponent is RootDebuff rootDebuff) rootDebuff.Initialize(stat, duration, type);
        if (effectComponent is SlowDebuff slowDebuff) slowDebuff.Initialize(stat, duration, value, type);
        if (effectComponent is AttackSpeedBuff attackSpeedbuff) attackSpeedbuff.Initialize(stat, duration, mod, value, type);

        return effectComponent;
    }
}
