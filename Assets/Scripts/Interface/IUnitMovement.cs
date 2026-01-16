using UnityEngine;
using UnityEngine.AI;

public interface IUnitMovement
{
    void Move(Vector3 position);
    void Stop();
    bool IsMoving();
    bool IsArrived(float epsilon = 0.05f);
}
