using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour, IUnitMovement
{
    public bool IsArrived(NavMeshAgent agent, float epsilon = 0.05F)
    {
        return true;
    }

    public bool IsMoving(NavMeshAgent agent)
    {
        return true;
    }

    public void Move(NavMeshAgent agent, Vector3 position)
    {
        agent.SetDestination(position);
    }

    public void Stop(NavMeshAgent agent)
    {
       agent.velocity = Vector3.zero;
       agent.ResetPath();
    }
}
