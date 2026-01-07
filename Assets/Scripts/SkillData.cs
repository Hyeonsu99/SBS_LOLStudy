using UnityEngine;

public enum TargetType
{

}

public enum DeliveryType
{

}

public enum EffectType
{

}

public class SkillData : ScriptableObject
{
    public string skillId;
    public string skillName;

    public TargetType targetType;
    public float range;


}
