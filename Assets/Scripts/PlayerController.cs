using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    PositionValidator _positionValidator;
    TargetValidator _targetValidator;
    CombatHandler _combatHandler;
    UnitStat _stat;

    IUnitMovement _movement;
    NavMeshAgent _agent;

    private void Awake()
    {
        _positionValidator = GetComponent<PositionValidator>();
        _targetValidator = GetComponent<TargetValidator>();
        _combatHandler = GetComponent<CombatHandler>();
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

    private void HandleBasicAttack()
    {

    }

    private void HandleQSkill()
    {

    }

    private void HandleWSkill()
    {

    }

    private void HandleESkill()
    {

    }

    private void HandleRSKill()
    {

    }
}
