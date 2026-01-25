using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

[CreateAssetMenu(menuName = "Skill/Blitzcrank/R Data")]
public class BlitzRData : SkillData
{
    // Passive Data
    public float[] PassiveBaseDamages = { 50, 100, 150 };
    public float PassiveApRatio = 0.3f;
    public float PassiveManaRatio = 0.03f;
    public float MarkDelay = 1.0f;

    public float[] ActiveBaseDamages = { 275, 400, 525 };
    public float ActiveApRatio = 1.0f;
    public float ActiveRadius = 6.0f;
    public float SilenceDuration = 0.75f;

    public override void OnEquip(GameObject owner, UnitStat stat, int level)
    {
        int index = Mathf.Clamp(level - 1, 0, PassiveBaseDamages.Length - 1);

        float passiveDmg = PassiveBaseDamages[index];
        float activeDmg = ActiveBaseDamages[index];

        var handler = owner.GetOrAddComponent<BlitzRHandler>();
        // 쿨타임 체크를 위해 SkillHandler도 전달
        handler.SetUp(stat, owner.GetComponent<SkillHandler>(), this, passiveDmg, activeDmg);
        base.OnEquip(owner, stat, level);
    }

    public override void Execute(GameObject owner, GameObject target, Vector3 position, int level)
    {
        var handler = owner.GetComponent<BlitzRHandler>();
        if (handler != null)
        {
            handler.Activate();
        }
    }
}

