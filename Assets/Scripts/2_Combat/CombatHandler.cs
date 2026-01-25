using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    private UnitStat _unitStat;
    private TargetValidator _targetValidator;

    private float _rangeBuffer = 0.1f;
    private const float RANGE_UNIT_SCALE = 100f;

    private GameObject _currentTarget;
    public GameObject CurrentTarget => _currentTarget;
    private UnitStat _targetStat;

    private float _lastAttackTime;
    [SerializeField] private bool _isRangeAttack = false;

    private List<IAttackConstraint> _constraints = new();
    private List<IAttackProvider> _damageProviders = new();

    public event Action<DamageInfo> OnHitUpdate;
    public event Action OnAttackPerformed;

    private void Awake()
    {
        _unitStat = GetComponent<UnitStat>();
        _targetValidator = GetComponent<TargetValidator>();

        _damageProviders.AddRange(GetComponents<IAttackProvider>());
        _constraints.AddRange(GetComponents<IAttackConstraint>());
    }

    public void RegisterConstraint(IAttackConstraint constraint)
    {
        if(!_constraints.Contains(constraint))  
            _constraints.Add(constraint); 
    }

    public void UnregisterConstraint(IAttackConstraint constraint)
    {
        _constraints.Remove(constraint);
    }

    public void RegisterProvider(IAttackProvider provider)
    {
        if (!_damageProviders.Contains(provider))
            _damageProviders.Add(provider);
    }

    public void UnregisterProvider(IAttackProvider provider)
    {
        _damageProviders.Remove(provider);
    }

    // 타겟이 사거리 내에 존재하는지?
    public bool IsTargetInAttackRange()
    {
        if (_currentTarget == null) return false;

        float range = _unitStat.Current.Get(StatType.AttackRange) / RANGE_UNIT_SCALE;
        float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);

        return distance <= range + _rangeBuffer;
    }

    // 타겟이 기본 공격 가능한 상태인지?
    public bool IsTargetValid()
    {
        if(_currentTarget == null) return false;   

        return _targetValidator != null && _targetValidator.IsValidTargetForBasicAttack(_currentTarget);    
    }

    public void UpdateCombat()
    {
        if (!IsTargetValid() || !IsTargetInAttackRange()) return;

        float attackSpeed = _unitStat.Current.Get(StatType.AttackSpeed);

        float attackDelay = 1f / attackSpeed;

        if(Time.time >= _lastAttackTime + attackDelay)
        {
            TryExecuteBasicAttack();
            _lastAttackTime = Time.time;
        }
    }

    // 기본 공격
    private void TryExecuteBasicAttack()
    {
        if(_targetStat == null) return;

        foreach(var constraint in _constraints)
        {
            if (!constraint.CanAttack()) return;
        }

        OnAttackPerformed?.Invoke();

        // 3. 자원 소모 (탄환 감소)
        foreach (var constraint in _constraints)
        {
            constraint.OnAttack();
        }

        if (_isRangeAttack)
        {
            // 원거리는 여기서 끝
        }
        else
        {
            ApplyDamage(_currentTarget);
        }
    }

    public void ApplyDamage(GameObject target, bool forceCrit = false)
    {
        if (target == null) return;
        if (!target.TryGetComponent(out UnitStat targetStat)) return;

        // 1. 데미지 정보 생성
        DamageInfo info = new DamageInfo
        {
            Attacker = gameObject,
            Target = target,
            RawDamage = _unitStat.Current.Get(StatType.AttackDamage),
            BonusDamage = 0,
            Type = DamageType.Physical,
            // 진 4타처럼 강제 치명타가 넘어오면 true, 아니면 확률 계산
            isCritical = forceCrit || IsCritical()
        };

        // 2. 데미지 데코레이팅 (진 패시브: 잃은 체력 비례 데미지 등 추가)
        foreach (var provider in _damageProviders)
        {
            provider.DecorateDamage(ref info, _unitStat, targetStat);
        }

        // 3. 최종 데미지 계산
        float finalDamage = DamageCalculater.CalculateFinalDamage(_unitStat, targetStat, info);

        // 4. 적용
        targetStat.TakeDamage(finalDamage, gameObject);

        // 5. 적중 시 이벤트 (흡혈 등 처리)
        OnHitUpdate?.Invoke(info);

        Debug.Log($"[Combat] {gameObject.name} -> {target.name} | Final Damage: {finalDamage} (Crit: {info.isCritical})");
    }

    private bool IsCritical()
    {
        float change = _unitStat.Current.Get(StatType.CriticalAmount);
        return UnityEngine.Random.Range(0f, 100f) < change;
    }

    public void SetTarget(GameObject target)
    {
        if(_currentTarget == target) return;

        if(target == null)
        {
            ClearTarget();
            return;
        }

        if (_targetValidator != null && !_targetValidator.IsValidTargetForBasicAttack(target))
        {
            return;
        }

        _currentTarget = target;
        _targetStat = target.GetComponent<UnitStat>();  
    }
    public void ClearTarget()
    {
        _currentTarget = null;
        _targetStat = null;
    }

    // 공격 타이머 초기화(평캔)
    public void ResetAttackTimer()
    {
        _lastAttackTime = -999f;
    }
    public bool HasTarget() => _currentTarget != null;
    public Vector3 GetTargetPosition() => _currentTarget != null ? _currentTarget.transform.position : transform.position;

}
