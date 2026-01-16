using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitStat : MonoBehaviour
{
    [SerializeField] private StatData StatData;
    [ShowInInspector] public float CurrentHP { get; private set; }
    [ShowInInspector] public float CurrentMP { get; private set; }


    [ShowInInspector]
    private readonly List<StatModifier> _mods = new();

    private Dictionary<string, Effect> _activeEffects = new();

    private List<string> _expiredEffects = new();

    private readonly List<IStatTransformer> _transformers = new();
    public IStat Current { get; private set; }

    [Title("Final Statistics")]
    [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    private Dictionary<StatType, float> _finalStat
    {
        get
        {
            var dict = new Dictionary<StatType, float>();
            if (Current == null) return dict;

            // 모든 스탯 타입을 순회하며 현재 값을 채웁니다.
            foreach (StatType type in Enum.GetValues(typeof(StatType)))
            {
                dict[type] = Current.Get(type);
            }
            return dict;
        }
    }

    [Title("active Effects")]
    [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    private List<string> _activeEffectsList
    {
        get
        {
            var list = new List<string>();  

            foreach(var effect in _activeEffects.Values)
            {
                if(effect != null && !effect.IsExpired)
                {
                    list.Add($"{effect.EffectType} ({effect.RemainTime:F1}s)");
                }
            }

            return list;
        }
    }

    private void Awake() => Rebuild();

    private void Start()
    {
        // 현재 체력 초기화..
        CurrentHP = Current.Get(StatType.Hp);
        CurrentMP = Current.Get(StatType.Mp);
    }

    // HP, MP 재설정
    // 데미지 처리..? 인터페이스..?
    public void RestoreHP(float amount) => CurrentHP = Mathf.Min(CurrentHP + amount, Current.Get(StatType.Hp));
    public void RestoreMP(float amount) => CurrentMP = Mathf.Min(CurrentMP + amount, Current.Get(StatType.Mp));
    public void TakeDamage(float damage) => CurrentHP = Mathf.Max(0, CurrentHP - damage);

    private void LateUpdate()
    {
        _expiredEffects.Clear();

        foreach(var kvp in _activeEffects)
        {
            if(kvp.Value == null || kvp.Value.IsExpired)
            {
                _expiredEffects.Add(kvp.Key);
            }
        }

        foreach(var effectId in _expiredEffects)
        {
            _activeEffects.Remove(effectId);
        }
    }

    public void Rebuild()
    {
        if (StatData == null) return;

        IStat baseEntity = new StatEntity(StatData);

        IStat result = baseEntity;

        // 레벨 및 성장 적용
        foreach(var mod in _mods)
        {
            if(mod.Stat == StatType.Level)
                result = new StatDecorator(result, mod, baseEntity);
        }

        result = new LevelStatDecorator(result, StatData);

        // 아이템/버프 Modifier 추출
        var statMods = _mods.FindAll(m => m.Stat != StatType.Level);

        // 계산 우선 순위 사용
        ModType[] priorityOrder =
        {
            ModType.Flat,
            ModType.PercentAdd,
            ModType.PercentMul
        };

        foreach (ModType modType in priorityOrder)
        {
            foreach (var mod in statMods.FindAll(m => m.Mod == modType))
            {
                result = new StatDecorator(result, mod, baseEntity);
            }
        }

        // 스탯 변환 적용(방어력의 50%만큼 체력이 늘어난다던지...)
        if(_transformers.Count > 0)
        {
            result = new TransformStatDecorator(result, baseEntity, _transformers);
        }

        Current = result;
    }

    public void AddModifier(StatModifier modifier)
    {
        _mods.Add(modifier);
        Rebuild();
    }

    // 
    public void RemoveModifier(StatModifier modifier)
    {
        _mods.RemoveAll(m => m.ID == modifier.ID);
        Rebuild();
    }

    public void AddTransformer(IStatTransformer transformer) { _transformers.Add(transformer); Rebuild(); }

    public void RemoveTransformer(IStatTransformer transformer) { _transformers.Remove(transformer); Rebuild(); }

    public void UpdateLevel(int targetLevel)
    {
        _mods.RemoveAll(m => m.ID == "CurrentLevel");

        int amount = targetLevel - 1;
        _mods.Add(new StatModifier("CurrentLevel", StatType.Level, ModType.Flat, amount, ModifierType.Growth));

        CurrentHP += StatData.GetGrowth(StatType.Hp);
        CurrentMP += StatData.GetGrowth(StatType.Mp);

        Rebuild();
    }

    public string ApplyEffect(EffectType type, ModType mod, float duration, float value, string customID = null)
    {
        if(!string.IsNullOrEmpty(customID) && _activeEffects.TryGetValue(customID, out Effect existingEffect))
        {
            existingEffect.Refresh(duration);
            return customID;
        }

        var effect = EffectFactory.Create(gameObject, type, mod, duration, value);

        if(effect != null)
        {
            _activeEffects[effect.EffectID] = effect;
            return effect.EffectID;
        }

        return null;
    }

    public string RefreshEffect(EffectType type, float duration, ModType mod, float value , string customID = null)
    {
        string effectType = GetEffetTypeName(type);
        var exist = FindEffectByType(effectType);

        if(exist != null)
        {
            exist.Refresh(duration);
            return effectType;
        }
        else
        {
            return ApplyEffect(type, mod, duration, value);
        }
    }

    public void RemoveEffect(string id)
    {
        if(_activeEffects.ContainsKey(id))
        {
            Destroy(_activeEffects[id]);
            _activeEffects.Remove(id);
        }
    }

    public float GetBonusStat(StatType type)
    {
        float bonus = 0f;

        foreach(var mod in _mods)
        {
            if(mod.Stat == type && mod.Type != ModifierType.Growth)
            {
                bonus += mod.Value;
            }
        }

        return bonus;
    }

    // 특정 타입의 모든 효과 리턴
    public List<Effect> GetEffectsByType(string effectType)
    {
        List<Effect> results = new List<Effect>();

        foreach(var effect in _activeEffects.Values)
        {
            if (effect != null && effect.EffectType == effectType && !effect.IsExpired)
            {
                results.Add(effect);
            }
        }

        return results;
    }

    // 특정 타입의 모든 효과 제거
    public void RemoveEffectsByType(string effectType)
    {
        List<string> toRemove = new();

        foreach(var kvp in _activeEffects)
        {
            toRemove.Add(kvp.Key);
        }

        foreach(var id in toRemove)
        {
            RemoveEffect(id);
        }
    }

    // 모든 효과 제거
    public void ClearEffect()
    {
        foreach(var effect in _activeEffects.Values)
        {
            if(effect != null)
            {
                Destroy(effect);
            }
        }

        _activeEffects.Clear();
    }

    // 특정 타입의 효과 찾기
    private Effect FindEffectByType(string effectType)
    {
        foreach(var effect in _activeEffects.Values)
        {
            if(effect != null && effect.EffectType == effectType && !effect.IsExpired)
            {
                return effect;
            }
        }

        return null;
    }    

    // 타입 이름 가져오기
    private string GetEffetTypeName(EffectType type)
    {
        return type switch
        {
            EffectType.AttackBuff => StringValue.AttackBuff,
            EffectType.SpeedBuff => StringValue.SpeedBuff,
            EffectType.ArmorBuff => StringValue.ArmorBuff,
            EffectType.AttackSpeedBuff => StringValue.AttackSpeedBuff,
            EffectType.AbilityPowerBuff => StringValue.AbilityPowerBuff,
            EffectType.SlowDebuff => StringValue.SlowDebuff,
            EffectType.ArmorDebuff => StringValue.ArmorDebuff,
            _ => ""
        };       
    }

    [BoxGroup("Level Test")]
    [Button(ButtonSizes.Medium)]
    public void SetLevel2() => UpdateLevel(2);

    [BoxGroup("Level Test")]
    [Button(ButtonSizes.Medium)]
    public void SetLevel18() => UpdateLevel(18);


    [BoxGroup("Level Test")]
    [Button(ButtonSizes.Medium)]
    public void ResetLevel() => UpdateLevel(1);

    [BoxGroup("Buff Test")]
    [Button(ButtonSizes.Medium)]
    public void TestSpeedBuff()
    {
        ApplyEffect(EffectType.SpeedBuff, ModType.PercentAdd, 3f, 0.1f);
    }

    // 깡 공격력 30 더하기
    [BoxGroup("Buff Test")]
    [Button(ButtonSizes.Medium)]
    public void TestAdBuff1()
    {
        ApplyEffect(EffectType.AttackBuff, ModType.Flat, 5f, 30);
    }

    // 총 공격력 50% 증가
    [BoxGroup("Buff Test")]
    [Button(ButtonSizes.Medium)]
    public void TestAdBuff2()
    {
        ApplyEffect(EffectType.AttackBuff, ModType.PercentMul, 5f, 0.5f);
    }

    // 공격속도 20% 증가
    [BoxGroup("Passive Test")]
    [Button(ButtonSizes.Medium)]
    public void TestASIncrease()
    {
        AddModifier(new StatModifier("TestAS", StatType.AttackSpeed, ModType.PercentMul, 0.2f, ModifierType.Passive));

        Rebuild();
    }

    [BoxGroup("체젠, 마젠 적용 테스트")]
    [Button(ButtonSizes.Medium)]
    public void TestRegen()
    {
        TakeDamage(50);
        CurrentMP -= 50f;
    }
}
