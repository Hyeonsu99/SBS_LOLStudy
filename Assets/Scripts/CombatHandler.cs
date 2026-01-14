using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    private UnitStat _unitStat;
    private TargetValidator _targetValidator;
    private GameObject _currentTarget;

    private IAttackConstraint _attackConstraint;
    private float _lastAttackTime;

    private List<IAttackConstraint> _constraints = new();

    private void Awake()
    {
        _unitStat = GetComponent<UnitStat>();
        _targetValidator = GetComponent<TargetValidator>();
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

    public void SetTarget(GameObject target) => _currentTarget = target;
    public void ClearTarget() => _currentTarget = null;
    public bool HasTarget() => _currentTarget != null;
    public Vector3 GetTargetPosition() => _currentTarget != null ? _currentTarget.transform.position : transform.position;  

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
            if(!constraint.CanAttack())
            {
                Debug.Log("공격 제약 조건에 의해 차단됨");
                return;
            }
        }

        // 공격 쿨타임 갱신 처리
        float attackCooldown = 1f / _unitStat.Current.AttackSpeed;

        if(Time.time > _lastAttackTime + attackCooldown)
        {
            Debug.Log($"{_currentTarget.name}에게 공격 성공!");
            _lastAttackTime = Time.time;

            foreach(var constraint in _constraints)
            {
                constraint.OnAttack();
            }
        }
    }
}
