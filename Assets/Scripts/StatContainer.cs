using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

// 챔피언의 1레벨 초기 스탯 보관용
public class StatEntity : IStat
{
    private readonly StatData _data;
    public StatEntity(StatData data) => _data = data;
    public float Get(StatType type) => _data.GetBase(type);

    public float Level => Get(StatType.Level);
    public float Hp => Get(StatType.Hp);
    public float HpRegen => Get(StatType.HpRegen);
    public float Mp => Get(StatType.Mp);
    public float MpRegen => Get(StatType.MpRegen);
    public float AttackDamage => Get(StatType.AttackDamage);
    public float AttackRange => Get(StatType.AttackRange);
    public float AttackSpeed => Get(StatType.AttackSpeed);
    public float AbilityPower => Get(StatType.AbilityPower);
    public float Armor => Get(StatType.Armor);
    public float MagicResist => Get(StatType.MagicResist);
    public float MoveSpeed => Get(StatType.MoveSpeed);
}

public class StatDecorator : IStat
{
    private readonly IStat _wrapped;
    private readonly StatModifier _mod;

    public StatDecorator(IStat wrapped, StatModifier mod)
    {
        _wrapped = wrapped;
        _mod = mod;
    }
    public float Get(StatType type)
    {
        float value = _wrapped.Get(type);

        if(_mod.Stat == type)
        {
            return _mod.Mode == ModType.Add ? value + _mod.Value : value * _mod.Value;
        }

        return value;
    }

    public float Level => Get(StatType.Level);
    public float Hp => Get(StatType.Hp);
    public float HpRegen => Get(StatType.HpRegen);
    public float Mp => Get(StatType.Mp);
    public float MpRegen => Get(StatType.MpRegen);
    public float AttackDamage => Get(StatType.AttackDamage);
    public float AttackRange => Get(StatType.AttackRange);
    public float AttackSpeed => Get(StatType.AttackSpeed);
    public float AbilityPower => Get(StatType.AbilityPower);
    public float Armor => Get(StatType.Armor);
    public float MagicResist => Get(StatType.MagicResist);
    public float MoveSpeed => Get(StatType.MoveSpeed);
}

public class LevelStatDecorator : IStat
{
    private readonly IStat _wrapped;
    private readonly StatData _data;

    public LevelStatDecorator(IStat wrapped, StatData data)
    {
        _wrapped = wrapped;
        _data = data;
    }

    public float Get(StatType type)
    {
        var baseValue = _wrapped.Get(type);

        if(type == StatType.Level) return baseValue;

        float n = _wrapped.Get(StatType.Level);
        float growth = _data.GetGrowth(type);

        if(growth <= 0) return baseValue;

        // 공속, 방어력, 마법 저항력 수치 조정

        return baseValue + growth * (n - 1) * (0.7025f + 0.0175f * (n - 1));
    }

    public float Level => Get(StatType.Level);
    public float Hp => Get(StatType.Hp);
    public float HpRegen => Get(StatType.HpRegen);
    public float Mp => Get(StatType.Mp);
    public float MpRegen => Get(StatType.MpRegen);
    public float AttackDamage => Get(StatType.AttackDamage);
    public float AttackRange => Get(StatType.AttackRange);
    public float AttackSpeed => Get(StatType.AttackSpeed);
    public float AbilityPower => Get(StatType.AbilityPower);
    public float Armor => Get(StatType.Armor);
    public float MagicResist => Get(StatType.MagicResist);
    public float MoveSpeed => Get(StatType.MoveSpeed);
}


