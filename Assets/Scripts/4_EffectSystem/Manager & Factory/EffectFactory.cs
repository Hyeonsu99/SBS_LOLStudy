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
    BlitzRMark,

    // CC기 타입
    Root,
    Silence,
    Stun,
    Airborne
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

            // --- 디버프 ---
            case EffectType.SlowDebuff:
                return CreateEffect<SlowDebuff>(target, stat, type, mod, duration, value);
            case EffectType.JhinMark:
                return CreateEffect<JhinMarkDebuff>(target, stat, type, mod, duration, value);
            case EffectType.BlitzRMark:
                return CreateEffect<BlitzRMark>(target, stat, type, mod, duration, value);

            // --- CC기 (통합 관리) ---
            case EffectType.Root:
            case EffectType.Stun:
            case EffectType.Silence:
                // CCEffect 하나로 3가지를 모두 처리 (Type으로 구분됨)
                return CreateEffect<CCDebuff>(target, stat, type, mod, duration, value);

            // --- 에어본 (특수 CC) ---
            case EffectType.Airborne:
                return CreateEffect<AirborneDebuff>(target, stat, type, mod, duration, value);
            default:
                return null;        
        }
    }

    private static T CreateEffect<T>(GameObject target, UnitStat stat, EffectType type, ModType mod, float duration, float value) where T : Effect
    {
        T effectComponent = target.AddComponent<T>();

        if (effectComponent is AttackBuff attack) attack.Initialize(stat, duration, mod, value, type);
        else if (effectComponent is SpeedBuff speedBuff) speedBuff.Initialize(stat, duration, mod, value, type);
        else if (effectComponent is AttackSpeedBuff attackSpeed) attackSpeed.Initialize(stat, duration, mod, value, type);
        else if (effectComponent is SlowDebuff slow) slow.Initialize(stat, duration, value, type); // Slow는 value 필요
        else if (effectComponent is JhinMarkDebuff jhin) jhin.Initialize(stat, duration, type);
        else if (effectComponent is BlitzRMark blitz) blitz.Initialize(stat, duration, type, value);

        // 2. [신규] 단순 CC기 (Stun, Silence, Root)
        else if (effectComponent is CCDebuff cc)
        {
            // CC기는 ModType이나 Value가 필요 없음
            cc.Initialize(stat, duration, type);
        }
        else if (effectComponent is RootDebuff root)
        {
            root.Initialize(stat, duration, type);
        }

        // 3. [신규] 에어본 (Value를 높이로 사용)
        else if (effectComponent is AirborneDebuff airborne)
        {
            // value 파라미터를 height로 전달
            airborne.Initialize(stat, duration, value, type);
        }

        return effectComponent;
    }
}
