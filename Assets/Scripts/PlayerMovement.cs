using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour, IUnitMovement
{
    private UnitStat _stat;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stat = GetComponent<UnitStat>();      
    }

    private void Update()
    {
        if (_stat != null && _agent != null)
        {
            _agent.speed = _stat.Current.Get(StatType.MoveSpeed) / 100f;
        }
    }

    public void Move(Vector3 position)
    {
        if(_agent.isOnNavMesh)
        {
            _agent.SetDestination(position);
        }     
    }

    public void Stop()
    {
        if(_agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }

    public bool IsMoving() => _agent.velocity.sqrMagnitude > 0.01f;


    public bool IsArrived(float epsilon = 0.05F)
    {
        if(!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance + epsilon)
                return true;
        }
        return false;
    }
}
