using Sirenix.OdinInspector;
using UnityEngine;

public enum StatType
{
    Level,
    Hp,
    HpRegen,
    Mp,
    MpRegen,
    AttackDamage,
    AttackRange,
    AttackSpeed,
    AbilityPower,
    Armor,
    MagicResist,
    MoveSpeed,
    CriticalAmount,
    CriticalDamage
}

public enum ModType
{
    Flat,
    PercentMul,
    PercentAdd
}

// ModifierType : 어떤 곳에서 능력치가 제공되고 있나...
public enum ModifierType
{
    Growth, // 레벨업 성장
    Item,
    Buff,
    Debuff,
    Passive
}

public readonly struct StatModifier
{
    public readonly string ID;
    public readonly StatType Stat;
    public readonly ModType Mod;
    public readonly float Value;
    public readonly ModifierType Type;

    public StatModifier(StatType stat, ModType mode, float value, ModifierType type)
    {
        ID = null;
        Stat = stat;
        Mod = mode;
        Value = value;
        Type = type;
    }

    public StatModifier(string id, StatType stat, ModType mode, float value, ModifierType type)
    {
        ID = id;
        Stat = stat;
        Mod = mode;
        Value = value;
        Type = type;
    }
}

public interface IStat
{
    float Get(StatType type);

    // 최종 계산된 스탯 가져오기
    float Level { get; }
    float Hp { get; }
    float HpRegen { get; }
    float Mp { get; }
    float MpRegen { get; }
    float AttackDamage { get; }
    float AttackRange { get; }
    float AttackSpeed { get; }
    float AbilityPower { get; }
    float Armor { get; }
    float MagicResist { get; }
    float MoveSpeed { get; }
}
