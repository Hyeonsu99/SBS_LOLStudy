using System;
using UnityEngine;

public class BlitzRHandler : MonoBehaviour, IAttackProvider
{
    private UnitStat _ownerStat;
    private SkillHandler _skillHandler;
    private BlitzRData _data;
    private CombatHandler _combatHandler;

    private float _levelPassiveDamage;
    private float _levelActiveDamage;

    private SkillSlot _rSlot;

    public void SetUp(UnitStat stat, SkillHandler skillHandler, BlitzRData data, float passiveDmg, float activeDmg)
    {
        _ownerStat = stat;
        _skillHandler = skillHandler;
        _data = data;
        _combatHandler = GetComponent<CombatHandler>();
        _levelActiveDamage = activeDmg;
        _levelPassiveDamage = passiveDmg;

        // 공격 시 이벤트 수신을 위해 등록
        if (_combatHandler != null)
        {
            _combatHandler.RegisterProvider(this);
        }

        // R 스킬 슬롯 찾기 (쿨타임 확인용)
        _rSlot = _skillHandler.GetSkillSlotByData(data);
    }

    private void OnDestroy()
    {
        if (_combatHandler != null)
        {
            _combatHandler.UnregisterProvider(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DecorateDamage(ref DamageInfo info, UnitStat attacker, UnitStat target)
    {
        // 기본 공격에만 반응
        if (info.Type != DamageType.Physical) return;

        if (_rSlot == null || !_rSlot.IsReady) return;

        float ownerAP = _ownerStat.Current.Get(StatType.AbilityPower);
        float ownerMP = _ownerStat.Current.Get(StatType.Mp);

        float passiveDmg = _levelPassiveDamage + (ownerAP * _data.PassiveApRatio) + (ownerMP * _data.PassiveManaRatio);

        string uniqueID = Guid.NewGuid().ToString();
        target.ApplyEffect(EffectType.BlitzRMark, ModType.Flat, _data.MarkDelay, passiveDmg, uniqueID);
    }

    public void Activate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _data.ActiveRadius);

        foreach (var col in colliders)
        {
            if (col.gameObject == gameObject) continue;

            if (col.TryGetComponent(out UnitIdentity id))
            {
                var ownerId = GetComponent<UnitIdentity>();
                if (ownerId.IsEnemy(id))
                {
                    ApplyActiveDamage(col.gameObject);
                }
            }
        }
    }

    private void ApplyActiveDamage(GameObject target)
    {
        if (target.TryGetComponent(out UnitStat targetStat))
        {
            float ownerAP = _ownerStat.Current.Get(StatType.AbilityPower);

            // 1. 데미지
            float dmg = _levelActiveDamage + (ownerAP * _data.ActiveApRatio);

            DamageInfo info = new DamageInfo
            {
                Attacker = gameObject,
                Target = target,
                RawDamage = dmg,
                Type = DamageType.Magic
            };

            float finalDmg = DamageCalculater.CalculateFinalDamage(_ownerStat, targetStat, info);
            targetStat.TakeDamage(finalDmg, gameObject);

            // 2. 침묵 적용
            targetStat.ApplyEffect(EffectType.Silence, ModType.Flat, _data.SilenceDuration, 1);

            Debug.Log(targetStat.gameObject.name);
        }
    }
}
