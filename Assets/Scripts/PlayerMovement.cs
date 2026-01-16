using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour, IUnitMovement
{
    private UnitStat _stat;
    private NavMeshAgent _agent;


    private void Awake()
    {
        _stat = GetComponent<UnitStat>();
        _agent = GetComponent<NavMeshAgent>();
    }

    public bool IsArrived(float epsilon = 0.05F)
    {
        return true;
    }

    public bool IsMoving()
    {
        return true;
    }

    private void Update()
    {
        if(_stat != null && _agent != null)
        {
            _agent.speed = _stat.Current.Get(StatType.MoveSpeed) / 100f;
        }
    }

    public void Move(Vector3 position)
    {
        _agent.SetDestination(position);
    }

    public void Stop()
    {
       _agent.velocity = Vector3.zero;
       _agent.ResetPath();
    }
}
