using UnityEngine;

public static class StatModifierFactory
{
    // +,- 용 Modifier
    public static StatModifier Add(string id, StatType stat, float value)
        => new StatModifier(id, stat, ModType.Add, value);

    public static StatModifier Add(StatType stat, float value)
        => new StatModifier(stat, ModType.Add, value);

    // 배율 직접 입력
    public static StatModifier Mul(string id, StatType stat, float value)  
        => new StatModifier(id, stat, ModType.Mul, value);

    public static StatModifier Mul(StatType stat, float value)
        => new StatModifier(stat, ModType.Mul, value);

    // 퍼센트 => 배율 변환
    public static StatModifier Percent(string id, StatType stat, float value)
        => new StatModifier(id, stat, ModType.Mul, (1f + value));

    public static StatModifier Percent(StatType stat, float value)
        => new StatModifier(stat, ModType.Mul, (1f + value));
}
