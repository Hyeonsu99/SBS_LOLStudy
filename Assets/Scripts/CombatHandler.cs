using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    private UnitStat _unitStat;
    private TargetValidator _targetValidator;
    private GameObject _currentTarget;

    private float _lastAttackTime;

    private List<IAttackConstraint> _constraints = new();
    private List<IAttackProvider> _damageProviders = new();

    public Action<DamageInfo> OnHitUpdate;

    private void Awake()
    {
        _unitStat = GetComponent<UnitStat>();
        _targetValidator = GetComponent<TargetValidator>();

        _damageProviders.AddRange(GetComponents<IAttackProvider>());
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

    public bool IsTargetValid()
    {
        if(_currentTarget == null) return false;

        return _targetValidator.IsValidTargetForBasicAttack(_currentTarget);
    }

    public void UpdateCombat()
    {
        if(!IsTargetValid())
        {
            ClearTarget();
            return;
        }

        float range = _unitStat.Current.AttackRange / 100f;
        float distance = Vector3.Distance(transform.position, _currentTarget.transform.position); 
        
        if(distance <= range)
        {
            TryExecuteBasicAttack();
        }
    }

    // 기본 공격
    private void TryExecuteBasicAttack()
    {
        foreach(var constraint in _constraints)
        {
            if (!constraint.CanAttack()) return;
        }

        // 공격 쿨타임 갱신 처리
        float attackCooldown = 1f / _unitStat.Current.AttackSpeed;

        if(Time.time > _lastAttackTime + attackCooldown)
        {
            _lastAttackTime = Time.time;

            if (!_currentTarget.TryGetComponent(out UnitStat targetStat)) return;

            DamageInfo info = new DamageInfo
            {
                Attacker = gameObject,
                Target = _currentTarget,
                RawDamage = _unitStat.Current.Get(StatType.AttackDamage),
                BonusDamage = 0,
                Type = DamageType.Physical,
                isCritical = IsCritical()
            };

            foreach(var provider in _damageProviders)
            {
                provider.DecorateDamage(ref info, _unitStat, targetStat);
            }

            float finalDamage = DamageCalculater.CalculateFinalDamage(_unitStat, targetStat, info);
            targetStat.TakeDamage(finalDamage, gameObject);

            Debug.Log($"기본 공격! 데미지 : {finalDamage}");

            foreach (var constraint in _constraints)
            {
                constraint.OnAttack();
            }

            OnHitUpdate?.Invoke(info);
        }
    }

    private bool IsCritical()
    {
        float change = _unitStat.Current.Get(StatType.CriticalAmount);
        return UnityEngine.Random.Range(0f, 100f) < change;
    }

    public void SetTarget(GameObject target) => _currentTarget = target;
    public void ClearTarget() => _currentTarget = null;
    public bool HasTarget() => _currentTarget != null;
    public Vector3 GetTargetPosition() => _currentTarget != null ? _currentTarget.transform.position : transform.position;

}
