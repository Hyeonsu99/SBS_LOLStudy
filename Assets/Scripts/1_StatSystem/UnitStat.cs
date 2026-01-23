using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitStat : MonoBehaviour
{
    [SerializeField] private StatData StatData;

    public IStat Current { get; private set; }
    [ShowInInspector] public float CurrentHP { get; private set; }
    [ShowInInspector] public float CurrentMP { get; private set; }
    [ShowInInspector] public float CurrentShield => _activeShields.Sum(s => s.Amount);

    public bool IsRoot => HasEffect(EffectType.Root);
    public bool IsDead => CurrentHP <= 0;

    private readonly List<StatModifier> _mods = new();
    private readonly Dictionary<string, Effect> _activeEffects = new();
    private List<ShieldInfo> _activeShields = new();
    private readonly List<IStatTransformer> _transformers = new();

    public event Action<float, GameObject, bool> OnTakeDamage;
    public event Action<GameObject> OnDeath;
    public event Action OnStatChanged;

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
                    list.Add($"{effect.EffectID} ({effect.RemainTime:F1}s)");
                }
            }

            return list;
        }
    }

    private void Awake()
    {
        Rebuild();
        // 현재 체력 초기화..
        CurrentHP = Current.Get(StatType.Hp);
        CurrentMP = Current.Get(StatType.Mp);
    }

    private void Update()
    {
        if(_activeShields.Count > 0)
        {
            for(int i = _activeShields.Count - 1; i >- 0; i--)
            {
                _activeShields[i].Duration -= Time.deltaTime;
                if(_activeShields[i].Duration <= 0)
                {
                    _activeShields.RemoveAt(i);
                }
            }
        }
    }

    private void LateUpdate()
    {
        UpdateExpiredEffects();
    }

    private void UpdateExpiredEffects()
    {
        if (_activeEffects.Count == 0) return;

        var expiredKey = new List<string>();

        foreach(var kvp in _activeEffects)
        {
            if(kvp.Value == null || kvp.Value.IsExpired)
                expiredKey.Add(kvp.Key);
        }

        foreach(var key in expiredKey)
        {
            _activeEffects.Remove(key);
        }
    }

    public void RestoreHP(float amount)
    {
        float maxHp = Current.Get(StatType.Hp);
        CurrentHP = Mathf.Clamp(CurrentHP + amount, 0, maxHp);
    }

    public void RestoreMP(float amount)
    {
        float maxMp = Current.Get(StatType.Mp);
        CurrentMP = Mathf.Clamp(CurrentMP + amount, 0, maxMp);
    }

    public void AddShield(float amount, float duration, string id = "")
    {
        _activeShields.Add(new ShieldInfo(amount, duration, id));
        // 여기서 UI 이벤트 호출(실드 업데이트 시)
    }   

    public void TakeDamage(float damage, GameObject attacker)
    {
        if (IsDead) return;

        float remainDamage = damage;

        // 만료시간이 임박한 쉴드부터 깎기...
        if(_activeShields.Count > 0)
        {
            for(int i = 0; i < _activeShields.Count; i++)
            {
                ShieldInfo shield = _activeShields[i];

                float damageToShield = Mathf.Min(remainDamage, shield.Amount);

                shield.Amount -= damageToShield;
                remainDamage -= damageToShield;
            }

            _activeShields.RemoveAll(s => s.Amount <= 0);
        }

        if(remainDamage > 0)
        {
            CurrentHP = Mathf.Max(0, CurrentHP - damage);

            OnTakeDamage?.Invoke(damage, gameObject, false);
        }
        else
        {
            OnTakeDamage?.Invoke(damage, gameObject, false);
        }     

        if(IsDead)
        {
            Die(attacker);
        }
    }

    private void Die(GameObject killer)
    {
        OnDeath?.Invoke(killer);

        if(killer != null)
        {
            DistributeExp(killer);
        }
    }

    private void DistributeExp(GameObject killer)
    {
        // 경험치 공유나 그래프는 일단 제외
        if (killer.TryGetComponent(out EXPHandler expHandler))
        {
            float rewardExp = Current.Get(StatType.Level) * 50f;
            expHandler.AddExp(rewardExp);
        }
    }

    public void Rebuild()
    {
        if (StatData == null) return;

        IStat baseEntity = new StatEntity(StatData);
        IStat result = baseEntity;

        // 레벨 Modifier 적용
        foreach(var mod in _mods)
        {
            if(mod.Stat == StatType.Level)
                result = new StatDecorator(result, mod, baseEntity);
        }

        // 레벨당 성장 적용
        result = new LevelStatDecorator(result, StatData);

        foreach(var mod in _mods)
        {
            if(mod.Stat != StatType.Level && mod.Mod == ModType.Flat)
            {
                result = new StatDecorator(result, mod, baseEntity);
            }
        }

        foreach(StatType type in Enum.GetValues(typeof(StatType)))
        {
            float totalPercent = 0f;

            var percentMods = _mods.FindAll(m => m.Stat == type && m.Mod == ModType.PercentAdd);

            if(percentMods.Count > 0)
            {
                foreach (var mod in percentMods)
                    totalPercent += mod.Value;

                var sumMod = new StatModifier($"{type}_SumPercent", type, ModType.PercentAdd, totalPercent, ModifierType.Passive);
                result = new StatDecorator(result, sumMod, baseEntity);
            }
        }

        foreach(var mod in _mods)
        {
            if (mod.Stat != StatType.Level && mod.Mod == ModType.PercentMul)
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

        OnStatChanged?.Invoke();
    }

    public float GetBonusStat(StatType type)
    {
        float bonus = 0f;

        foreach (var mod in _mods)
        {
            if (mod.Stat == type && mod.Type != ModifierType.Growth)
            {
                bonus += mod.Value;
            }
        }

        return bonus;
    }

    public void AddModifier(StatModifier modifier) { _mods.Add(modifier); Rebuild(); }

    public void RemoveModifier(StatModifier modifier) { _mods.RemoveAll(m => m.ID == modifier.ID); Rebuild(); }

    public void AddTransformer(IStatTransformer transformer) { _transformers.Add(transformer); Rebuild(); }

    public void RemoveTransformer(IStatTransformer transformer) { _transformers.Remove(transformer); Rebuild(); }

    public void UpdateLevel(int targetLevel)
    {
        float maxHpBefore = Current.Get(StatType.Hp);
        float maxMpBefore = Current.Get(StatType.Mp);

        float hpPercent = (maxHpBefore > 0) ? CurrentHP / maxHpBefore : 1f;
        float mpPercent = (maxMpBefore > 0) ? CurrentMP / maxMpBefore : 1f;

        _mods.RemoveAll(m => m.ID == "CurrentLevel");

        int amount = targetLevel - 1;
        _mods.Add(new StatModifier("CurrentLevel", StatType.Level, ModType.Flat, amount, ModifierType.Growth));

        Rebuild();

        CurrentHP = Current.Get(StatType.Hp) * hpPercent;
        CurrentMP = Current.Get(StatType.Mp) * mpPercent;
    }

    public string ApplyEffect(EffectType type, ModType mod, float duration, float value, string customID = null)
    {
        if(!string.IsNullOrEmpty(customID) && _activeEffects.TryGetValue(customID, out Effect existingEffect))
        {
            existingEffect.Refresh(duration);
            return customID;
        }

        var exist = FindEffectByType(type);
        if(exist != null && string.IsNullOrEmpty(customID))
        {
            exist.Refresh(duration);
            return exist.EffectID;
        }

        var effect = EffectFactory.Create(gameObject, type, mod, duration, value);

        if(effect != null)
        {
            _activeEffects[effect.EffectID] = effect;
            return effect.EffectID;
        }

        return null;
    }

    public void RemoveEffect(string id)
    {
        if(_activeEffects.ContainsKey(id))
        {
            Destroy(_activeEffects[id]);
            _activeEffects.Remove(id);
        }
    }

    public void RemoveEffect(EffectType type)
    {
        var effect = FindEffectByType(type);
        if(effect != null)
        {
            RemoveEffect(effect.EffectID);
        }
    }

    public bool HasEffect(EffectType type)
    {
        foreach(var effect in _activeEffects.Values)
        {
            if(effect != null && !effect.IsExpired && effect.Type == type)
                return true;
        }

        return false;
    }

    // 특정 타입의 효과 찾기
    private Effect FindEffectByType(EffectType type)
    {
        foreach(var effect in _activeEffects.Values)
        {
            if(effect != null && effect.Type == type && !effect.IsExpired)
            {
                return effect;
            }
        }

        return null;
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
        TakeDamage(50, gameObject);
        CurrentMP -= 50f;
    }

    [BoxGroup("블리츠 쉴드 테스트용")]
    [Button(ButtonSizes.Medium)]
    public void TestBlitzPassive()
    {
        TakeDamage(CurrentHP * 0.8f, gameObject);
    }
}
