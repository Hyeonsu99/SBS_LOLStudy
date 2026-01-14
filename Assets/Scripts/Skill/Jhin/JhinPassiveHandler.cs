using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class JhinPassiveHandler : MonoBehaviour, IStatTransformer, IAttackConstraint
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
    }

    public float Transform(StatType type, float value, IStat baseStat, IStat chain)
    {
        if(type == StatType.AttackSpeed) return baseStat.Get(StatType.AttackSpeed);

        if(type == StatType.AttackDamage)
        {
            float bonusAS = _stat.GetBonusStat(StatType.AttackSpeed);
            float critAmount = _stat.Current.Get(StatType.CriticalAmount);
            float currentLevel = _stat.Current.Get(StatType.Level);

            // 레벨 관련 증가수치 추가
            return value + (bonusAS * _data.AS_to_AD_Ratio) + (critAmount * _data.Cri_to_AD_Ratio);
        }
        
        return value;
    }

    public bool CanAttack()
    {
        if (CurrentAmmo <= 0 && IsReloading) return false;

        return true;
    }

    public void OnAttack()
    {
        CurrentAmmo--;

        _lastActionTime = Time.time;

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

    private IEnumerator ReloadCoroutine()
    {
        IsReloading = true;
        yield return new WaitForSeconds(_data.ReloadTime);
        CurrentAmmo = 4;
        IsReloading = false;
    }
}
