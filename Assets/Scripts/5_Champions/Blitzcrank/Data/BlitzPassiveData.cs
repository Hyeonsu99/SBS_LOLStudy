using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Blitzcrank/Passive Data")]
public class BlitzPassiveData : SkillData
{
    public float MP_TO_SHILID_RATIO = 0.35f;
    public float TriggerHp = 0.3f;
    public float ShieldDuration = 10.0f;

    public override void OnEquip(GameObject owner, UnitStat stat, int level)
    {
        var handler = owner.GetOrAddComponent<BlitzPassiveHandler>();
        handler.SetUp(stat, owner.GetComponent<SkillHandler>(), this);

        base.OnEquip(owner, stat, level);
    }

    public override void OnUnEquip(GameObject owner)
    {
        base.OnUnEquip(owner);
    }

    public override void Execute(GameObject owner, GameObject target, Vector3 position, int level)
    {
        if(owner.TryGetComponent(out UnitStat stat))
        {
            float maxMana = stat.Current.Get(StatType.Mp);
            float shieldAmount = maxMana * MP_TO_SHILID_RATIO;

            stat.AddShield(shieldAmount, ShieldDuration);

            Debug.Log($"블리츠 패시브 발동! 쉴드량 {shieldAmount}");
        }

        base.Execute(owner, target, position, level);
    }
}
