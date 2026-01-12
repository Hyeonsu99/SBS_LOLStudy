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
    Add,
    Mul
}

public readonly struct StatModifier
{
    public readonly string ID;
    public readonly StatType Stat;
    public readonly ModType Mode;
    public readonly float Value;

    public StatModifier(StatType stat, ModType mode, float value)
    {
        ID = null;
        Stat = stat;
        Mode = mode;
        Value = value;
    }

    public StatModifier(string id, StatType stat, ModType mode, float value)
    {
        ID = id;
        Stat = stat;
        Mode = mode;
        Value = value;
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
