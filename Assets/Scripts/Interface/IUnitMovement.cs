using UnityEngine;
using UnityEngine.AI;

public interface IUnitMovement
{
    void Move(NavMeshAgent agent, Vector3 position);
    void Stop(NavMeshAgent agent);
    bool IsMoving(NavMeshAgent agent);
    bool IsArrived(NavMeshAgent agent, float epsilon = 0.05f);
}
