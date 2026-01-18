using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    PositionValidator _positionValidator;
    TargetValidator _targetValidator;
    CombatHandler _combatHandler;
    SkillHandler _skillHandler;
    UnitStat _stat;

    IUnitMovement _movement;
    NavMeshAgent _agent;

    private InputContext _pendingSkillCtx;
    private bool _isSkillPending = false;

    private void Awake()
    {
        _positionValidator = GetComponent<PositionValidator>();
        _targetValidator = GetComponent<TargetValidator>();
        _combatHandler = GetComponent<CombatHandler>();
        _skillHandler = GetComponent<SkillHandler>();
        _movement = GetComponent<PlayerMovement>() as IUnitMovement;
        _agent = GetComponent<NavMeshAgent>();
        _stat = GetComponent<UnitStat>();
    }

    // Update is called once per frame
    void Update()
    {
        // 1. 예약된 스킬이 있는지 먼저 확인
        if (_isSkillPending)
        {
            HandlePendingSkill();
            return; // 스킬 예약 중에는 기본 공격 로직을 잠시 미룸
        }

        if (_combatHandler.HasTarget())
        {
            if(!_combatHandler.IsTargetValid())
            {
                _combatHandler.ClearTarget();
                _agent.ResetPath();
                return;
            }

            float range = _stat.Current.AttackRange / 100f;
            float distance = Vector3.Distance(transform.position, _combatHandler.GetTargetPosition());

            if(distance <= range)
            {
                _agent.ResetPath();
                _combatHandler.UpdateCombat();
            }
            else
            {
                _movement?.Move(_combatHandler.GetTargetPosition());
            }
        }
    }

    public void HandleCommand(PlayerAction action, InputContext ctx)
    {
        switch(action)
        {
            case PlayerAction.Move:
                HandleRightClick(ctx);
                break;
            case PlayerAction.CastSkill:
                HandleSkill(ctx);
                break;
            case PlayerAction.CastSummoner:
                HandleSummoner(ctx);
                break;

        }
    }

    private void HandleRightClick(InputContext ctx)
    {
        CancelPendingSkill(); // 스킬 예약 취소

        if (ctx.target == null)
            return;

        if(_targetValidator.IsValidTargetForBasicAttack(ctx.target))
        {
            _combatHandler.SetTarget(ctx.target);
        }
        else
        {
            _combatHandler.ClearTarget();
            if (_positionValidator.IsValidMovePosition(ctx.position, ctx.target.layer))
            {
                _movement?.Move(ctx.position);
            }
        }
    }

    private void HandlePendingSkill()
    {
        if (_pendingSkillCtx.target == null)
        {
            CancelPendingSkill();
            return;
        }

        // 해당 스킬의 사거리 가져오기
        var slot = _skillHandler.GetSKillSlot(_pendingSkillCtx.skillCommand);
        if (slot == null || slot.Level <= 0)
        {
            CancelPendingSkill();
            return;
        }

        // SkillData에서 현재 레벨의 사거리 가져오기 (단위 변환 /100f는 프로젝트 기준에 맞게 조정)
        float range = slot.GetRange() / 100f;
        float distance = Vector3.Distance(transform.position, _pendingSkillCtx.target.transform.position);

        if (distance <= range)
        {
            // 사거리 안이면 이동을 멈추고 시전
            _agent.ResetPath();
            _skillHandler.Execute(_pendingSkillCtx);
            _isSkillPending = false;
        }
        else
        {
            // 사거리 밖이면 타겟에게 다가감
            _agent.SetDestination(_pendingSkillCtx.target.transform.position);
        }
    }

    private void HandleSkill(InputContext ctx)
    {
        // 타겟팅 스킬인 경우에만 예약 시스템 사용
        if (ctx.target != null)
        {
            _combatHandler.ClearTarget(); // 기본 공격 대상 해제
            _pendingSkillCtx = ctx;
            _isSkillPending = true;
        }
        else
        {
            // 논타겟 스킬 등은 기존처럼 즉시 실행
            _skillHandler.Execute(ctx);
        }
    }

    private void HandleSummoner(InputContext ctx)
    {
        switch(ctx.summonerCommand)
        {
            case SummonerCommand.D:
                Debug.Log("D 스펠 발동");
                break;
            case SummonerCommand.F:
                Debug.Log("F 스펠 발동");
                break;
        }
    }

    public void CancelPendingSkill()
    {
        _isSkillPending = false;
        _pendingSkillCtx = default;
    }
}
