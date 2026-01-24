using UnityEngine;

public class CCDebuff : Effect
{
    public override void Initialize(UnitStat stat, float duration, EffectType type, string customID = null)
    {
        base.Initialize(stat, duration, type, customID);
    }

    protected override void Apply()
    {
        
    }

    protected override void Remove()
    {
        
    }

}
