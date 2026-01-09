using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField] private StatData StatData;

    private readonly List<StatModifier> _mods = new();

    public IStat Current { get; private set; }

    [Title("Final Statistics")]
    [ShowInInspector, ReadOnly]
    private Dictionary<StatType, float> FinalStats
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

    private void Awake() => Rebuild();

    public void Rebuild()
    {
        if (StatData == null) return;

        IStat result = new StatEntity(StatData);

        foreach(var mod in _mods)
        {
            result = new StatDecorator(result, mod);
        }

        result = new LevelStatDecorator(result, StatData);

        Current = result;
    }

    public void AddModifier(StatModifier modifier)
    {
        _mods.Add(modifier);
        Rebuild();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        foreach(var mod in _mods)
        {
            if(mod.ID == modifier.ID)
            {
                _mods.Remove(mod);
                _mods.Add(modifier);
            }
        }
    }

    public void UpdateLevel(int targetLevel)
    {
        _mods.RemoveAll(m => m.ID == "CurrentLevel");

        int amount = targetLevel - 1;
        _mods.Add(new StatModifier("CurrentLevel", StatType.Level, ModType.Add, amount));

        Rebuild();
        Debug.Log(Current.Level);
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

}
