using UnityEngine;

public class BlitzEHandler : MonoBehaviour, IAttackProvider
{
    private UnitStat _ownerStat;
    private BlitzEData _data;
    private CombatHandler _combatHandler;

    private bool _isSkillActive = false;
    private float _buffEndTime;

    public void SetUp(UnitStat stat, BlitzEData data)
    {
        _ownerStat = stat;
        _data = data;
        _combatHandler = GetComponent<CombatHandler>();

        // [중요] CombatHandler에게 "나도 데미지 계산에 끼워줘"라고 등록
        if (_combatHandler != null)
        {
            _combatHandler.RegisterProvider(this);
        }
    }

    private void Update()
    {
        // 시간 초과 시 버프 해제
        if (_isSkillActive && Time.time >= _buffEndTime)
        {
            _isSkillActive = false;
            Debug.Log("강철 주먹 지속시간 만료");
        }
    }

    private void OnDestroy()
    {
        if (_combatHandler != null)
        {
            _combatHandler.UnregisterProvider(this);
        }
    }

    public void Activate()
    {
        _isSkillActive = true;
        _buffEndTime = Time.time + _data.BuffDuration;

        // [핵심] 평타 타이머 리셋 (평타 캔슬)
        if (_combatHandler != null)
        {
            _combatHandler.ResetAttackTimer();
        }

        // TODO: 주먹이 빛나는 이펙트 재생
        Debug.Log("강철 주먹 장전!");
    }

    public void DecorateDamage(ref DamageInfo info, UnitStat attacker, UnitStat target)
    {
        if (!_isSkillActive) return;
        if (info.Type != DamageType.Physical) return;

        info.RawDamage *= _data.DamageMultiplier;
        info.RawDamage += _ownerStat.Current.Get(StatType.AbilityPower) * _data.ApRatio;

        target.ApplyEffect(EffectType.Airborne, ModType.Flat, _data.AirbornDuration, 1);

        _isSkillActive = false;
    }
}
