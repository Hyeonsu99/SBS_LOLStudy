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

    private IInputOverride _inputOverrider;

    public GameObject _defaultCam;
    public GameObject _ultimateCamera;

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

    public void SetInputOverride(IInputOverride overrider)
    {
        _inputOverrider = overrider;
    }
        
    // Update is called once per frame
    void Update()
    {
        if(_inputOverrider != null)
        {
            _movement.Stop();
            return;
        }

        if (_stat.IsCC)
        {
            _movement.Stop();             
            _combatHandler.ClearTarget(); 
            CancelPendingSkill();         
            return;                       
        }

        if(_stat.IsRoot)
        {
            _movement.Stop();
        }

        if (_skillHandler.IsCasting)
        {
            _movement.Stop();
            _combatHandler.ClearTarget();
            return;
        }

        // 1. 예약된 스킬이 있는지 먼저 확인
        if (_isSkillPending)
        {
            HandlePendingSkill();
            return; // 스킬 예약 중에는 기본 공격 로직을 잠시 미룸
        }

        if (_combatHandler.HasTarget())
        {
            if (!_combatHandler.IsTargetValid())
            {
                _combatHandler.ClearTarget();
                _movement.Stop();
                return;
            }

            if (_combatHandler.IsTargetInAttackRange())
            {
                _movement.Stop();
                _combatHandler.UpdateCombat();
            }
            else
            {
                _movement.Move(_combatHandler.GetTargetPosition());
            }
        }
    }

    public void HandleCommand(PlayerAction action, InputContext ctx)
    {
        if(_inputOverrider != null)
        {
            bool isConsumed = false;

            switch(action)
            {
                case PlayerAction.Move:
                    isConsumed = _inputOverrider.OnMoveInput();
                    break;
                case PlayerAction.CastSkill:
                    isConsumed = _inputOverrider.OnSkillInput(ctx.skillCommand);
                    break;
            }

            if (isConsumed) return;
        }

        if (_stat.IsCC) return;

        if (_stat.IsRoot && action == PlayerAction.Move)
        {
            return;
        }

        if (_stat.IsSilenced && (action == PlayerAction.CastSkill || action == PlayerAction.CastSummoner))
        {
            return;
        }

        if (_skillHandler.IsCasting) return;

        switch (action)
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

        _combatHandler.ClearTarget();
        _movement?.Move(ctx.position);

        if (ctx.target != null)
        {
            _combatHandler.SetTarget(ctx.target);
        }
    }

    private void HandlePendingSkill()
    {
        var command = _pendingSkillCtx.skillCommand;

        if (_stat.IsCC || _stat.IsSilenced)
        {
            CancelPendingSkill();
            return;
        }

        // 추격 도중 스킬이 준비되지 않았거나 타겟이 사라지면 취소
        if (!_skillHandler.IsSkillReady(command))
        {
            _isSkillPending = false;
            return;
        }

        SkillSlot slot = _skillHandler.GetSKillSlot(command);

        if (slot.IsUnitTargeting && _pendingSkillCtx.target == null)
        {
            _isSkillPending = false;
            return;
        }

        // 사거리 체크
        if (_skillHandler.IsSkillInRange(_pendingSkillCtx))
        {
            _movement.Stop();
            _skillHandler.Execute(_pendingSkillCtx);
            _isSkillPending = false;
        }
        else
        {
            if (_stat.IsRoot)
            {
                _movement.Stop();
                _isSkillPending = false;
                return;
            }

            Vector3 dest;

            if (slot.IsUnitTargeting && _pendingSkillCtx.target != null)
            {
                dest = _pendingSkillCtx.target.transform.position;
            }
            else
            {
                dest = _pendingSkillCtx.position;
            }

            _movement.Move(dest);
        }
    }

    private void HandleSkill(InputContext ctx)
    {
        _combatHandler.ClearTarget();

        SkillSlot slot = _skillHandler.GetSKillSlot(ctx.skillCommand);
        if (slot == null) return;

        if(slot.IsUnitTargeting)
        {
            if(ctx.target != null && _targetValidator.IsValidTargetForSkill(ctx.target, slot.Data.TargetMask))
            {
                _pendingSkillCtx = ctx;
                _isSkillPending = true;
            }
            else
            {
                Debug.Log("스킬을 사용할 수 없는 대상입니다");
            }
        }
        else if(slot.TargetType == TargetType.Point)
        {
            if (_skillHandler.IsSkillInRange(ctx))
            {
                _movement.Stop();
                _skillHandler.Execute(ctx);
            }
            else
            {
                _pendingSkillCtx = ctx;
                _isSkillPending = true;
            }
        }
        else
        {
            _movement.Stop();
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

    // 임시
    public void HandleCameraOverride(bool active)
    {
        if(active)
        {
            _defaultCam.SetActive(false);
            _ultimateCamera.SetActive(true);
        }
        else
        {
            _defaultCam.SetActive(true);
            _ultimateCamera.SetActive(false);
        }
    }

    public void CancelPendingSkill()
    {
        _isSkillPending = false;
        _pendingSkillCtx = default;
    }
}
