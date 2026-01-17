using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class JhinPassiveHandler : MonoBehaviour, IStatTransformer, IAttackConstraint, IAttackProvider
{
    private JhinPassiveData _data;
    private UnitStat _stat;

    public int CurrentAmmo { get; private set; } = 4;
    public bool IsReloading { get; private set; }
    private float _lastActionTime;

    private void Update()
    {
        if(!IsReloading && CurrentAmmo < 4 && Time.time - _lastActionTime >= _data.AutoReloadDelay)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void SetUp(JhinPassiveData data, UnitStat stat)
    {
        _data = data;
        _stat = stat;
        _stat.AddTransformer(this);

        if(TryGetComponent(out CombatHandler combat))
        {
            combat.OnHitUpdate += (info) => { if (info.isCritical) OnCriticalHit(); };
        }
    }

    public float Transform(StatType type, float value, IStat baseStat, IStat chain)
    {
        if(type == StatType.AttackSpeed) return baseStat.Get(StatType.AttackSpeed);

        if(type == StatType.AttackDamage)
        {
            float bonusAS = _stat.GetBonusStat(StatType.AttackSpeed);
            float critAmount = _stat.Current.Get(StatType.CriticalAmount);
            float currentLevel = _stat.Current.Get(StatType.Level);

            float levelFactor = Mathf.Lerp(0.04f, 0.44f, (Mathf.Clamp(currentLevel, 1, 18) - 1) / 17f);
            float statFactor = (bonusAS * _data.AS_to_AD_Ratio + critAmount * _data.Cri_to_AD_Ratio);

            // 레벨 관련 증가수치 추가
            return value + (value * (levelFactor + statFactor));
        }

        // 치명타 피해량 25% 감소 패시브
        if(type == StatType.CriticalDamage)
        {
            return value * 0.75f;
        }
        
        return value;
    }

    public void DecorateDamage(ref DamageInfo info, UnitStat attacker, UnitStat target)
    {
        if(CurrentAmmo == 1)
        {
            info.isCritical = true;

            float maxHp = target.Current.Get(StatType.Hp);
            float currentHp = target.CurrentHP;
            float missingHp = maxHp - currentHp;

            float level = attacker.Current.Get(StatType.Level);
            float percent = Mathf.Lerp(0.15f, 0.24f, (Mathf.Clamp(level, 1, 18) - 1) / 17f);

            info.BonusDamage += missingHp * percent;
        }
    }

    public void OnAttack()
    {

        _lastActionTime = Time.time;

        CurrentAmmo--;

        // 장전 중 탄환이 남은 상태에서 행동하면 장전 취소
        if (IsReloading && CurrentAmmo > 0)
        {
            StopAllCoroutines();
            IsReloading = false;
        }

        if(CurrentAmmo <= 0)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    // 치명타 시 이동속도 증가 버프
    public void OnCriticalHit()
    {
        float bonusAS = _stat.GetBonusStat(StatType.AttackSpeed);

        float statFactor = 0.14f + (bonusAS * _data.AS_to_SPD_Ratio);

        _stat.ApplyEffect(EffectType.SpeedBuff, ModType.PercentAdd, 2f, statFactor, "Jhin_Haste");

        Debug.Log("치명타 발생! 이동속도 2초간 증가");
    }

    private IEnumerator ReloadCoroutine()
    {
        IsReloading = true;
        yield return new WaitForSeconds(_data.ReloadTime);
        CurrentAmmo = 4;
        IsReloading = false;
    }

    public bool CanAttack() => !(CurrentAmmo <= 0 && IsReloading);
}
