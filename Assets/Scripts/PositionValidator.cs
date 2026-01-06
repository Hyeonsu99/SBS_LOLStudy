using UnityEngine;
using UnityEngine.AI;

public class PositionValidator : MonoBehaviour
{
    // 나중에 수정이 필요할수도... 
    public bool OnNavMesh(Vector3 point)
    {
        if(NavMesh.SamplePosition(point, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            return true;
        }

        return false;
    }
}
