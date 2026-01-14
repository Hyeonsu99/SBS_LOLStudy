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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_combatHandler.HasTarget())
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
                _movement?.Move(_agent, _combatHandler.GetTargetPosition());
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
                _movement?.Move(_agent, ctx.position);
            }
        }
    }

    private void HandleSkill(InputContext ctx)
    {
        switch (ctx.skillCommand)
        {
            case SkillCommand.Q:
                Debug.Log("Q 스킬 발동");
                break;
            case SkillCommand.W:
                Debug.Log("W 스킬 발동");
                break;
            case SkillCommand.E:
                Debug.Log("E 스킬 발동");
                break;
            case SkillCommand.R:
                Debug.Log("R 스킬 발동");
                break;
            default:
                break;
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
}
