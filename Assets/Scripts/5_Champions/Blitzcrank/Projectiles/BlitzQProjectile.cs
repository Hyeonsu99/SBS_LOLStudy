using System;
using UnityEngine;
using UnityEngine.AI;

public class BlitzQProjectile : MonoBehaviour
{
    private enum State { Firing, Returning };

    private GameObject _owner;
    private UnitStat _ownerStat;
    private BlitzQData _data;
    private int _skillLevel;
    private Action _onReturnCallback; // 핸들러에게 복귀 알림

    private float _baseDamage;

    private State _currentState = State.Firing;
    private Vector3 _startPos;
    private Vector3 _direction;

    private Transform _grabbedTarget;
    private UnitStat _grabbedStat;

    public void Initialize(GameObject owner, Vector3 dir, BlitzQData data, float baseDamage, int level, Action onReturnCallback)
    {
        _owner = owner;
        _ownerStat = owner.GetComponent<UnitStat>();
        _data = data;
        _skillLevel = level;
        _baseDamage = baseDamage;
        _direction = dir;
        _onReturnCallback = onReturnCallback;
        _startPos = transform.position;

        Destroy(gameObject, 5f); // 안전장치
    }

    void Update()
    {
        if (_currentState == State.Firing) HandleFiring();
        else HandleReturning();
    }

    private void HandleFiring()
    {
        float move = _data.ProjectileSpeed * Time.deltaTime;
        transform.position += _direction * move;

        // 최대 사거리 도달 시 복귀
        if (Vector3.Distance(_startPos, transform.position) >= _data.MaxDistance)
        {
            StartReturn();
        }
    }

    private void HandleReturning()
    {
        // 주인 추적 (점멸 등 이동 고려)
        Vector3 ownerPos = _owner.transform.position + Vector3.up;
        Vector3 dirToOwner = (ownerPos - transform.position).normalized;

        float move = _data.ReturnSpeed * Time.deltaTime;
        transform.position += dirToOwner * move;

        // [끌어오기] 타겟 위치 동기화
        if (_grabbedTarget != null)
        {
            _grabbedTarget.position = transform.position;
        }

        // 주인에게 도착 확인
        if (Vector3.Distance(transform.position, ownerPos) < 1.0f)
        {
            FinishGrab();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_currentState != State.Firing) return;
        if (other.gameObject == _owner) return;

        // 적군 유닛 체크 (타워 제외)
        if (other.TryGetComponent(out UnitIdentity id))
        {
            if (id.Type == UnitType.Structure) return; // 타워 무시

            var ownerId = _owner.GetComponent<UnitIdentity>();
            if (ownerId != null && ownerId.IsEnemy(id))
            {
                HitTarget(other.gameObject);
            }
        }
    }

    private void HitTarget(GameObject target)
    {
        _grabbedTarget = target.transform;

        if (target.TryGetComponent(out UnitStat targetStat))
        {
            _grabbedStat = targetStat;

            // 1. 데미지 계산 (기본 + 성장 + 계수)
            float dmg = _baseDamage + (_ownerStat.Current.Get(StatType.AbilityPower) * _data.ApRatio);
            // 계수 계산 로직은 DamageCalculater나 여기서 추가 가능

            DamageInfo info = new DamageInfo
            {
                Attacker = _owner,
                Target = target,
                RawDamage = dmg,
                Type = DamageType.Magic
            };

            float finalDmg = DamageCalculater.CalculateFinalDamage(_ownerStat, targetStat, info);
            targetStat.TakeDamage(finalDmg, _owner);

            // 2. 기절(CC) 부여
            // 돌아갈 거리 계산 -> 소요 시간 예측
            Vector3 pullDest = _owner.transform.position + (_owner.transform.forward * _data.PullOffsetDistance);
            float dist = Vector3.Distance(transform.position, pullDest);
            float travelTime = dist / _data.ReturnSpeed;

            // 최대 0.65초로 제한
            float stunDuration = Mathf.Min(travelTime, _data.MaxStunDuration);

            // 기절 적용 (Enum에 Stun이 있어야 함)
            targetStat.ApplyEffect(EffectType.Stun, ModType.Flat, stunDuration, 1);
        }

        StartReturn();
    }

    private void StartReturn()
    {
        _currentState = State.Returning;
    }

    private void FinishGrab()
    {
        if (_grabbedTarget != null)
        {
            // 블리츠 앞쪽 위치 계산
            Vector3 dropPos = _owner.transform.position + (_owner.transform.forward * _data.PullOffsetDistance);

            // NavMeshAgent 사용 시 Warp 필수
            if (_grabbedTarget.TryGetComponent(out NavMeshAgent agent))
            {
                agent.Warp(dropPos);
            }
            else
            {
                _grabbedTarget.position = dropPos;
            }
        }

        // 핸들러에게 알림 (블리츠 속박 해제)
        _onReturnCallback?.Invoke();

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // 안전장치: 파괴 시 콜백 호출
        _onReturnCallback?.Invoke();
    }
}
