using Sirenix.OdinInspector.Editor.Internal;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Jhin/Q Data")]
public class JhinQData : SkillData
{
    public float[] BaseDamages = { 44, 69, 94, 119, 144 };
    public float[] AdRatios = { 0.44f, 0.515f, 0.59f, 0.665f, 0.74f };
    public float ApRatio = 0.6f;

    public float BounceRange = 4f;
    public float KillBouns = 0.35f;
    public int MaxBounces = 4;
    public GameObject ProjectilePrefab;

    public override void Execute(GameObject owner, GameObject target, Vector3 position, int level)
    {
        int index = Mathf.Clamp(level - 1, 0, BaseDamages.Length - 1);

        float baseDamage = BaseDamages[index];
        float adRatio = AdRatios[index];
        float apRatio = ApRatio;

        var go = Instantiate(ProjectilePrefab, owner.transform.position + Vector3.up, Quaternion.identity);
        var projectile = go.GetComponent<JhinQProjectile>();
        
        projectile.Initialize(owner, target, baseDamage, adRatio, apRatio, this);
        base.Execute(owner, target, position, level);
    }
}
