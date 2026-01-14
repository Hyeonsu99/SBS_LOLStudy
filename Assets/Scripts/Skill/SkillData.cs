using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum TargetType
{
    None,
    Self,
    EnemyTarget,
    AllyTarget,
    Ground,
    EnemyAll
}

public enum DeliveryType
{
    Passive,
    Instant,
    Projectile,
    Area,
    Buff
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
