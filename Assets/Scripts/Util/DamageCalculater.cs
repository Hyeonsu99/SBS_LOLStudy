using UnityEngine;

public enum DamageType
{
    Physical,
    Magic,
    True
}

public struct DamageInfo
{
    public GameObject Attacker;
    public GameObject Target;
    public float RawDamage;
    public float BonusDamage;
    public DamageType Type;
    public bool isCritical;
}

public static class DamageCalculater
{
    public static float CalculateFinalDamage(UnitStat attacker, UnitStat target, DamageInfo info)
    {
        if (target == null) return 0f;

        float baseDamage = info.RawDamage;

        if(info.isCritical)
        {
            float critMul = attacker.Current.Get(StatType.CriticalDamage) / 100f;
            baseDamage *= critMul;
        }

        // 감소 전 데미지 변수
        float totalRawDamage = baseDamage + info.BonusDamage;

        // 방어력/마저 적용될 데미지 변수
        float finalDamage = totalRawDamage;
        switch(info.Type)
        {
            case DamageType.Physical:
                float armor = target.Current.Get(StatType.Armor);
                finalDamage *= (armor >= 0) ? (100f / (100f + armor)) : (2f - (100f / (100f - armor)));
                break;
            case DamageType.Magic:
                float mr = target.Current.Get(StatType.MagicResist);
                finalDamage *= (mr >= 0) ? (100f / (100f + mr)) : (2f - (100f / (100f - mr)));
                break;
            case DamageType.True:
                // 고정 피해는 감소 로직 없음
                break;
        }

        return finalDamage;
    }
}
