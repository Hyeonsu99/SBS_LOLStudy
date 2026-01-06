using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour, IUnitMovement
{
    public bool IsArrived(float epsilon = 0.05F)
    {
        return true;
    }

    public bool IsMoving()
    {
        return true;
    }

    public void Move(NavMeshAgent agent, Vector3 position)
    {
        agent.SetDestination(position);
    }

    public void Stop()
    {
        
    }
}
