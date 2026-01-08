using UnityEngine;
using UnityEngine.AI;

public class PositionValidator : MonoBehaviour
{
    // 나중에 수정이 필요할수도... 여기서 땅 유효검사..?
    public bool IsGroundLayer(int layer)
    {
        return true;
    }

    public bool IsOnNavMesh(Vector3 point, int layer)
    {
        return true;
    }
    public bool OnNavMesh(InputContext ctx)
    {
        if(ctx.target.layer != LayerMask.NameToLayer(StringValue.GroundLayerName))
            return false;

        if(NavMesh.SamplePosition(ctx.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            return true;

        return false;
    }
}
