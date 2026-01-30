using UnityEngine;
using System;
using System.Collections.Generic;

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
    public float CriticalAmount => Get(StatType.CriticalAmount);
    public float CriticalDamage => Get(StatType.CriticalDamage);
    public float AttackRange => Get(StatType.AttackRange);
    public float AttackSpeed => Get(StatType.AttackSpeed);
    public float AbilityPower => Get(StatType.AbilityPower);
    public float Armor => Get(StatType.Armor);
    public float MagicResist => Get(StatType.MagicResist);
    public float MoveSpeed => Get(StatType.MoveSpeed);
    public float AbilityHaste => Get(StatType.AbilityHaste);
}

public class StatDecorator : IStat
{
    private readonly IStat _wrapped;
    private readonly StatModifier _mod;
    private readonly IStat _baseEntity;

    public StatDecorator(IStat wrapped, StatModifier mod, IStat baseEntity)
    {
        _wrapped = wrapped;
        _mod = mod;
        _baseEntity = baseEntity;
    }
    
    public float Get(StatType type)
    {
        float value = _wrapped.Get(type);

        if(_mod.Stat != type)
        {
            return value;
        }

        return _mod.Mod switch
        {
            ModType.Flat => value + _mod.Value,
            ModType.PercentAdd => HandlePercentAdd(type, value),
            ModType.PercentMul => value * (1 + _mod.Value),
            _ => value
        };
    }

    private float HandlePercentAdd(StatType type, float currentVal)
    {
        if (type == StatType.AttackSpeed)
        {
            return currentVal + (_baseEntity.Get(type) * _mod.Value);
        }

        return currentVal * (1 + _mod.Value);
    }

    public float Level => Get(StatType.Level);
    public float Hp => Get(StatType.Hp);
    public float HpRegen => Get(StatType.HpRegen);
    public float Mp => Get(StatType.Mp);
    public float MpRegen => Get(StatType.MpRegen);
    public float AttackDamage => Get(StatType.AttackDamage);
    public float CriticalAmount => Get(StatType.CriticalAmount);
    public float CriticalDamage => Get(StatType.CriticalDamage);
    public float AttackRange => Get(StatType.AttackRange);
    public float AttackSpeed => Get(StatType.AttackSpeed);
    public float AbilityPower => Get(StatType.AbilityPower);
    public float Armor => Get(StatType.Armor);
    public float MagicResist => Get(StatType.MagicResist);
    public float MoveSpeed => Get(StatType.MoveSpeed);
    public float AblityHaste => Get(StatType.AbilityHaste);
    public float AbilityHaste => Get(StatType.AbilityHaste);
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

        float n = Mathf.Clamp(_wrapped.Get(StatType.Level), 1f, 18f);
        float growth = _data.GetGrowth(type);

        if(growth <= 0) return baseValue;

        if(type == StatType.AttackSpeed)
        {
            return baseValue * (1 + (growth * (n - 1f)));
        }

        float factor = (n - 1) * (0.7025f + 0.0175f * (n - 1));
        return baseValue + (growth * factor);
    }

    public float Level => Get(StatType.Level);
    public float Hp => Get(StatType.Hp);
    public float HpRegen => Get(StatType.HpRegen);
    public float Mp => Get(StatType.Mp);
    public float MpRegen => Get(StatType.MpRegen);
    public float AttackDamage => Get(StatType.AttackDamage);
    public float CriticalAmount => Get(StatType.CriticalAmount);
    public float CriticalDamage => Get(StatType.CriticalDamage);
    public float AttackRange => Get(StatType.AttackRange);
    public float AttackSpeed => Get(StatType.AttackSpeed);
    public float AbilityPower => Get(StatType.AbilityPower);
    public float Armor => Get(StatType.Armor);
    public float MagicResist => Get(StatType.MagicResist);
    public float MoveSpeed => Get(StatType.MoveSpeed);
    public float AbilityHaste => Get(StatType.AbilityHaste);
}

public class TransformStatDecorator : IStat
{
    private readonly IStat _wrapped;
    private readonly IStat _baseStat;
    private readonly List<IStatTransformer> _transformers;

    public TransformStatDecorator(IStat wrapped, IStat baseStat, List<IStatTransformer> transformers)
    {
        _wrapped = wrapped;
        _baseStat = baseStat;
        _transformers = transformers;
    }   

    public float Get(StatType type)
    {
        float value = _wrapped.Get(type);

        foreach(var transfomer in _transformers)
        {
            value = transfomer.Transform(type, value, _baseStat);
        }

        return value;
    }

    public float Level => Get(StatType.Level);
    public float Hp => Get(StatType.Hp);
    public float HpRegen => Get(StatType.HpRegen);
    public float Mp => Get(StatType.Mp);
    public float MpRegen => Get(StatType.MpRegen);
    public float AttackDamage => Get(StatType.AttackDamage);
    public float CriticalAmount => Get(StatType.CriticalAmount);
    public float CriticalDamage => Get(StatType.CriticalDamage);
    public float AttackRange => Get(StatType.AttackRange);
    public float AttackSpeed => Get(StatType.AttackSpeed);
    public float AbilityPower => Get(StatType.AbilityPower);
    public float Armor => Get(StatType.Armor);
    public float MagicResist => Get(StatType.MagicResist);
    public float MoveSpeed => Get(StatType.MoveSpeed);
    public float AbilityHaste => Get(StatType.AbilityHaste);
}


