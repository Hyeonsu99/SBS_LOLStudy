using UnityEngine;

public class JhinWHandler : MonoBehaviour
{
    private JhinWData _data;
    private SkillHandler _skillHandler;
    private CombatHandler _combatHandler;
    private UnitIdentity _unitIdentity;

    public void SetUp(JhinWData data, SkillHandler skillHandler, CombatHandler combatHandler, UnitIdentity unitIdentity)
    {
        _data = data;
        _skillHandler = skillHandler;
        _combatHandler = combatHandler;
        _unitIdentity = unitIdentity;

        if(combatHandler != null)
        {
            _combatHandler.OnHitUpdate += OnMyAttackHit;
        }
    }

    private void OnMyAttackHit(DamageInfo info)
    {
        ApplyMark(info.Target);
    }

    // 아군이나 스킬로 데미지를 줬을 때 외부에서 호출해줘야 함..?
    // 추후 이벤트 등으로 수정
    public void OnEnemyDamaged(GameObject target)
    {
        ApplyMark(target);
    }

    private void ApplyMark(GameObject target)
    {
        if(target == null || _skillHandler == null) return;

        if(target.TryGetComponent(out UnitIdentity unitIdentity))
        {
            if (!_unitIdentity.IsEnemy(unitIdentity)) return;
        }
        else
        {
            return;
        }

        SkillSlot wSlot = _skillHandler.Slot_W;

        if(wSlot == null || wSlot.Level <= 0) return;

        bool canMark = wSlot.IsReady || (wSlot.CurrentCooldown < 4.0f);

        if(canMark)
        {
            if(target.TryGetComponent(out UnitStat stat))
            {
                stat.ApplyEffect(EffectType.JhinMark, ModType.Flat, _data.MarkDuration, 0); 
            }
        }
    }
}
