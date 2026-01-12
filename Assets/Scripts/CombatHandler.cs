using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    private UnitStat _unitStat;
    private TargetValidator _targetValidator;
    private GameObject _currentTarget;
    private float _lastAttackTime;

    private void Awake()
    {
        _unitStat = GetComponent<UnitStat>();
        _targetValidator = GetComponent<TargetValidator>();
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
            TryExecuteAttack();
        }
    }

    private void TryExecuteAttack()
    {
        // 공격 쿨타임 갱신 처리
        float attackCooldown = 1f / _unitStat.Current.AttackSpeed;

        if(Time.time > _lastAttackTime + attackCooldown)
        {
            Debug.Log($"{_currentTarget.name}에게 공격 성공!");
            _lastAttackTime = Time.time;
        }
    }
}
