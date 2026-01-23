using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatData", menuName = "Stats/StatData")]
public class StatData : ScriptableObject
{
    public float Level = 1f;

    [Header("Base Stat : Level 1")]   
    public float Hp;
    public float HpRegen;
    public float Mp;
    public float MpRegen;
    public float AttackDamage;
    public float CriticalAmount;
    public float CriticalDamage;
    public float AttackRange;
    public float AttackSpeed;
    public float AbilityPower = 0f;
    public float Armor;
    public float MagicResist;
    public float MoveSpeed;

    [Header("Growth Stat (Per Level)")]
    public float LevelGrowth;
    public float HpGrowth;
    public float HpRegenGrowth;
    public float MpGrowth;
    public float MpRegenGrowth;
    public float AdGrowth;
    public float AsGrowth;
    public float ArmorGrowth;
    public float ResistGrowth;


    public float GetBase(StatType type)
    {
        return type switch
        {
            StatType.Level => Level,
            StatType.Hp => Hp,
            StatType.HpRegen => HpRegen,
            StatType.Mp => Mp,
            StatType.MpRegen => MpRegen,
            StatType.AttackDamage => AttackDamage,
            StatType.CriticalAmount => CriticalAmount,
            StatType.CriticalDamage => CriticalDamage,
            StatType.AttackRange => AttackRange,
            StatType.AttackSpeed => AttackSpeed,
            StatType.AbilityPower => AbilityPower,
            StatType.Armor => Armor,
            StatType.MagicResist => MagicResist,
            StatType.MoveSpeed => MoveSpeed,
            _ => 0f
        };
    }

    public float GetGrowth(StatType type)
    {
        return type switch
        {
            StatType.Level => LevelGrowth,
            StatType.Hp => HpGrowth,
            StatType.HpRegen => HpRegenGrowth,
            StatType.Mp => MpGrowth,
            StatType.MpRegen => MpRegenGrowth,
            StatType.AttackDamage => AdGrowth,
            StatType.AttackSpeed => AsGrowth,
            StatType.Armor => ArmorGrowth,
            StatType.MagicResist => ResistGrowth,
            _ => 0f
        };
    }
}
