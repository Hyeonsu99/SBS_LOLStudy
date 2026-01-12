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

    private readonly List<StatModifier> _mods = new();

    private Dictionary<string, Effect> _activeEffects = new();

    private List<string> _expiredEffects = new();
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

        IStat result = new StatEntity(StatData);

        foreach(var mod in _mods)
        {
            if (mod.Stat == StatType.Level)
                result = new StatDecorator(result, mod);
        }

        result = new LevelStatDecorator(result, StatData);

        foreach (var mod in _mods)
        {
            if(mod.Stat != StatType.Level)
                result = new StatDecorator(result, mod);
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
        //foreach(var mod in _mods)
        //{
        //    if(mod.ID == modifier.ID)
        //    {
        //        _mods.Remove(mod);
        //        _mods.Add(modifier);
        //    }
        //}

        _mods.RemoveAll(m => m.ID == modifier.ID);
        Rebuild();
    }

    public void UpdateLevel(int targetLevel)
    {
        _mods.RemoveAll(m => m.ID == "CurrentLevel");

        int amount = targetLevel - 1;
        _mods.Add(new StatModifier("CurrentLevel", StatType.Level, ModType.Add, amount));

        Rebuild();
        Debug.Log(Current.Level);
    }

    public string ApplyEffect(EffectType type, float duration, float value)
    {
        var effect = EffectFactory.Create(gameObject, type, duration, value);

        if(effect != null)
        {
            _activeEffects[effect.EffectID] = effect;
            return effect.EffectID;
        }

        return null;
    }

    public string RefreshEffect(EffectType type, float duration, float value)
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
            return ApplyEffect(type, duration, value);
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
        ApplyEffect(EffectType.SpeedBuff, 3f, 1.1f);
    }
}
