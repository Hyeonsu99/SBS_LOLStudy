using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Skill/Jhin/Passive Data")]
public class JhinPassiveData : SkillData
{
    public float AS_to_AD_Ratio = 0.25f;
    public float Cri_to_AD_Ratio = 0.4f;
    public float AS_to_SPD_Ratio = 0.44f;
    public float ReloadTime = 2.5f;
    public float AutoReloadDelay = 7.5f;

    public override void OnEquip(GameObject owner, UnitStat stat, int level)
    {
        var handler = owner.GetOrAddComponent<JhinPassiveHandler>();
        handler.SetUp(this, stat);

        var combat = owner.GetComponent<CombatHandler>();
        combat.RegisterConstraint(handler);

        combat.RegisterProvider(handler);

        base.OnEquip(owner, stat, level);
    }

    public override void OnUnEquip(GameObject owner)
    {
        // 필요할 시에 구현
        base.OnUnEquip(owner);
    }
}
