using UnityEngine;

public enum TargetType
{
    AllyChampion,
    EnemyChampion,
    EnemyAll,
    Ground,
    Any
}

public enum DeliveryType
{
    Single,
    PointAOE,
    NonTargetPoint,
    NonTargetLine
}

public abstract class SkillData : ScriptableObject
{
    public string SkillId;
    public string SkillName;

    public TargetType Type_Target;
    public DeliveryType Type_Delivery;
    public float Range;
    // 장판기 혹은 범위기일 경우 반경
    public float Radius;

    public abstract void Execute(GameObject owner, GameObject target, Vector3 position);
}
