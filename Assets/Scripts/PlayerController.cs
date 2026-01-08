using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    PositionValidator _positionValidator;
    TargetValidator _targetValidator;

    IUnitMovement _movement;
    NavMeshAgent _agent;

    private void Awake()
    {
        _positionValidator = GetComponent<PositionValidator>();
        _targetValidator = GetComponent<TargetValidator>();
        _movement = GetComponent<PlayerMovement>() as IUnitMovement;
        _agent = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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



        if (!_positionValidator.OnNavMesh(ctx))
            return;

        _movement?.Move(_agent, ctx.position);
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
