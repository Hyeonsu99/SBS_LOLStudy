using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Jhin/W Data")]
public class JhinWData : SkillData
{
    public float[] BaseDamages = { 70, 105, 140, 175, 210 };
    public float AdRatio = 0.5f;
    public float MinionDamageRatio = 0.75f;

    public float[] RootDurations = { 1.25f, 1.5f, 1.75f, 2f, 2.25f };
    public float MarkDuration = 4.0f;

    public float ProjectileSpeed = 50f;
    // 롤 기준 사거리, 사용할땐 100으로 나눠 사용
    public float MaxDistance = 3000f;

    public GameObject ProjectilePrefab;

    public override void OnEquip(GameObject owner, UnitStat stat, int level)
    {
        var WHandler = owner.GetOrAddComponent<JhinWHandler>();
        var skillHandler = owner.GetComponent<SkillHandler>();
        var combatHandler = owner.GetComponent<CombatHandler>();

        WHandler.SetUp(this, skillHandler, combatHandler);

        base.OnEquip(owner, stat, level);
    }

    public override void Execute(GameObject owner, GameObject target, Vector3 position, int level)
    {
        int index = Mathf.Clamp(level -1, 0, BaseDamages.Length - 1);

        float baseDamage = BaseDamages[index];
        float rootDuration = RootDurations[index];

        Vector3 direction = (position - owner.transform.position).normalized;
        direction.y = 0f;

        GameObject go = Instantiate(ProjectilePrefab, owner.transform.position + Vector3.up, Quaternion.LookRotation(direction));

        if(go.TryGetComponent(out JhinWProjectile projectile))
        {
            float realDistance = MaxDistance / 100f;

            projectile.Initialize(owner, direction, ProjectileSpeed, baseDamage, AdRatio, rootDuration, realDistance, MinionDamageRatio);
        }

        base.Execute(owner, target, position, level);
    }
}
