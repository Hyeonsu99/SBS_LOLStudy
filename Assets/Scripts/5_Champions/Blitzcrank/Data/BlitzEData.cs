using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Blitzcrank/E Data")]
public class BlitzEData : SkillData
{
    public float BuffDuration = 5.0f;
    public float DamageMultiplier = 2.0f;
    public float ApRatio = 0.25f;
    public float AirbornDuration = 1.0f;

    public override void OnEquip(GameObject owner, UnitStat stat, int level)
    {
        var handler = owner.GetOrAddComponent<BlitzEHandler>();
        handler.SetUp(stat, this);
        base.OnEquip(owner, stat, level);
    }

    public override void Execute(GameObject owner, GameObject target, Vector3 position, int level)
    {
        base.Execute(owner, target, position, level);
    }
}
