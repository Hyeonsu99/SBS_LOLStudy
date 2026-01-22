using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Jhin/R Data")]
public class JhinRData : SkillData
{
    // 1,2,3타 기준 기본 데미지
    public float[] BaseDamages = { 64, 128, 192 };
    // 1,2,3타 기준 AD 계수
    public float AdRatio = 0.25f;

    // 잃은 체력 비례 최대 4배의 피해 증폭 
    public float MaxDamageMultiplier = 4.0f;

    public float SlowPercentage = 0.8f;
    public float SlowDuration = 0.5f;

    public float Range = 34f;
    // 사격 최소 딜레이
    public float FireDelay = 1.0f;
    public float MaxDuration = 10.0f;

    public float CameraZoomSize = 12.0f;
    public float CameraOffsetDistance = 15.0f;

    public GameObject ProjectilePrefab;

    public override void OnEquip(GameObject owner, UnitStat stat, int level)
    {
        // 핸들러 부착 및 초기화
        var handler = owner.GetOrAddComponent<JhinRHandler>();
        var skillHandler = owner.GetComponent<SkillHandler>();
        var playerController = owner.GetComponent<PlayerController>();

        handler.SetUp(this, skillHandler, playerController);

        base.OnEquip(owner, stat, level);
    }

    public override void Execute(GameObject owner, GameObject target, Vector3 position, int level)
    {
        var handler = owner.GetComponent<JhinRHandler>();
        if (handler != null)
        {
            handler.StartUltimate(position, level);
        }
    }
}
