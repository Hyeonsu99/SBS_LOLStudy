using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Jhin/E Data")]
public class JhinEData : SkillData
{
    public float[] BaseDamages = { 20, 80, 140, 200, 260 };
    public float AdRatio = 1.2f;
    public float ApRatio = 1.0f;
    public float MinionDamageRatio = 0.65f;

    public float SlowPercentage = 0.35f;
    public float TrapDuration = 180f;
    public float ExplosionDelay = 2.0f;

    public GameObject TrapProjectilePrefab;
    public GameObject TrapPrefab;
    public GameObject ZonePrefab;

    public override void OnEquip(GameObject owner, UnitStat stat, int level)
    {
        var handler = owner.GetOrAddComponent<JhinEHandler>();
        handler.SetUp(this);
        base.OnEquip(owner, stat, level);
    }

    public override void Execute(GameObject owner, GameObject target, Vector3 position, int level)
    {
        Vector3 targetPos = position;
        
        int groundMask = LayerMask.GetMask(StringValue.GroundLayerName);
        RaycastHit hit;

        if (Physics.Raycast(targetPos + Vector3.up * 50f, Vector3.down, out hit, 100f, groundMask))
        {
            targetPos.y = hit.point.y;
        }
        else
        {
            targetPos.y = 0f; // 바닥을 못 찾으면 0으로 초기화
        }

        Vector3 startPos = owner.transform.position + Vector3.up;

        if(TrapProjectilePrefab != null)
        {
            GameObject projObj = Instantiate(TrapProjectilePrefab, startPos, Quaternion.identity);

            if(projObj.TryGetComponent(out JhinEProjectile proj))
            {
                proj.Initialize(owner, this, level, targetPos);
            }
        }
        else
        {
            GameObject trapObj = Instantiate(TrapPrefab, targetPos, Quaternion.identity);
            if (trapObj.TryGetComponent(out JhinETrap trap))
            {
                trap.Initialize(owner, this, level);
            }
        }

        base.Execute(owner, target, position, level);
    }
}
