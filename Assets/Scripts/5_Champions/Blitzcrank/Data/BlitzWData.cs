using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Blitzcrank/W Data")]
public class BlitzWData : SkillData
{
    public float[] AS_IncreaseAmount = { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f };
    public float[] MS_IncreaseAmount = { 0.6f, 0.65f, 0.7f, 0.75f, 0.8f };

    public float Duration = 5f;

    public float EndSlowPercent = 0.3f;
    public float EndSlowDuration = 1.5f;

    private BlitzWHandler _handler;

    public override void OnEquip(GameObject owner, UnitStat stat, int level)
    {
        var handler = owner.GetOrAddComponent<BlitzWHandler>();
        handler.SetUp(stat, this);

        _handler = handler;

        base.OnEquip(owner, stat, level);
    }

    public override void OnUnEquip(GameObject owner)
    {
        base.OnUnEquip(owner); 
    }

    public override void Execute(GameObject owner, GameObject target, Vector3 position, int level)
    {
        int index = Mathf.Clamp(level - 1, 0, AS_IncreaseAmount.Length - 1);

        float AS_Amount = AS_IncreaseAmount[index];
        float MS_Amount = MS_IncreaseAmount[index]; 

        if(_handler != null)
        {
            _handler.Activate(AS_Amount, MS_Amount);
        }

        base.Execute(owner, target, position, level);
    }
}
