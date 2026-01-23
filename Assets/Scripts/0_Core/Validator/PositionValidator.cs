using UnityEngine;
using UnityEngine.AI;

public class PositionValidator : MonoBehaviour
{
    // 나중에 수정이 필요할수도... 여기서 땅 유효검사..?
    // 나중에 NavMesh Layer로 조정해야할 듯

    public bool IsValidMovePosition(Vector3 position, int hitLayer)
    {
        if (hitLayer != LayerMask.NameToLayer(StringValue.GroundLayerName))
            return false;

        return NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas);
    }
}
