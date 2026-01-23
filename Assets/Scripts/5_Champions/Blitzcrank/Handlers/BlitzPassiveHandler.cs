using JetBrains.Annotations;
using UnityEngine;

public class BlitzPassiveHandler : MonoBehaviour
{
    private BlitzPassiveData _data;
    private UnitStat _ownerStat;
    private SkillHandler _ownerSkillhandler;

    public void SetUp(UnitStat stat, SkillHandler handler, BlitzPassiveData data)
    {
        _ownerStat = stat;
        _ownerSkillhandler = handler;
        _data = data;

        _ownerStat.OnTakeDamage += CheckOwnerCondition;
    }

    private void OnDestroy()
    {
        if (_ownerStat != null)
            _ownerStat.OnTakeDamage -= CheckOwnerCondition;
    }

    private void CheckOwnerCondition(float damage, GameObject attacker, bool b)
    {
        if(_ownerStat.IsDead) return;

        float maxHp = _ownerStat.Current.Get(StatType.Hp);
        float hpPercent = (maxHp > 0) ? (_ownerStat.CurrentHP / maxHp) : 0;

        if (hpPercent <= _data.TriggerHp)
        {
            SkillSlot passiveSlot = _ownerSkillhandler.Slot_Passive;

            if (passiveSlot != null && passiveSlot.IsReady)
            {
                passiveSlot.TryCast(null, transform.position);
            }
        }
    }

}
