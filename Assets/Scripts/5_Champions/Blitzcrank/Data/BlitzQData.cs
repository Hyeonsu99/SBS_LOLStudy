using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Skill/Blitzcrank/Q Data")]
public class BlitzQData : SkillData
{
    public float[] BaseDamages = { 110, 160, 210, 260, 310 };
    public float ApRatio = 1.2f;

    public float ProjectileSpeed = 18f;
    public float ReturnSpeed = 18f;
    public float MaxDistance = 10.79f;

    public float PullOffsetDistance = 0.75f;
    public float MaxStunDuration = 0.65f;

    public GameObject ProjectilePrefab;

    public override void OnEquip(GameObject owner, UnitStat stat, int level)
    {
        var handler = owner.GetOrAddComponent<BlitzQHandler>();
        handler.SetUp(stat, this);

        base.OnEquip(owner, stat, level);
    }

    public override void Execute(GameObject owner, GameObject target, Vector3 position, int level)
    {
        var handler = owner.GetComponent<BlitzQHandler>();
        if (handler != null)
        {
            // 레벨에 따른 기본 데미지 계산
            int index = Mathf.Clamp(level - 1, 0, BaseDamages.Length - 1);
            float baseDamage = BaseDamages[index];

            handler.Fire(position, baseDamage, level);
        }
    }
}
