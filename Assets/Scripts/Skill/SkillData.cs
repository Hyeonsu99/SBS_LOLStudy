using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Flags]
public enum TargetFilter
{
    None = 0,
    Champion = 1 << 0,
    Minion = 1 << 1,
    Monster = 1 << 2,
    Structure = 1 << 3,
    Unit = Champion | Minion | Monster,
    All = Champion | Minion | Monster | Structure
}

public enum TargetType
{
    None, // 즉시 시전
    Unit, // 유닛 타겟팅 (추격 필요)
    Point, // 지점 타겟팅 (지점까지 이동)
    Direction // 방향 타겟팅(논타겟팅)
}

public enum DeliveryType
{
    Instant,
    Projectile,
    Area,
    Buff,
    Passive
}

[CreateAssetMenu(menuName = "Data/Skill Data")]
public class SkillData : ScriptableObject
{
    public string SkillId;
    public string SkillName;
    public Sprite Icon;
    public string Description;

    public TargetType Type_Target;
    public DeliveryType Type_Delivery;

    public TargetFilter TargetMask = TargetFilter.All;

    public bool IsMovementSkill;

    // 0이면 일반 스킬, 1 이상이면 충전형 스킬..
    public int MaxCharges = 0;

    [Header("Level Data")]
    public int MaxLevel = 5;
    public float[] Costs;
    public float[] Cooldowns;
    public float[] Ranges;
    // 장판기 혹은 범위기일 경우 반경
    public float Radius;

    public float GetCost(int level) => GetVal(Costs, level);
    public float GetCooldown(int level) => GetVal(Cooldowns, level);
    public float GetRange(int level) => GetVal(Ranges, level);

    protected float GetVal(float[] arr, int level)
    {
        if (arr == null || arr.Length == 0) return 0f;

        return arr[Mathf.Clamp(level - 1, 0, arr.Length- 1)];   
    }

    // 액티브 스킬 부분
    public virtual void Execute(GameObject owner, GameObject target, Vector3 position, int level) { }
    public virtual void OnEquip(GameObject owner, UnitStat stat, int level) { }
    public virtual void OnUnEquip(GameObject owner) { }
}
