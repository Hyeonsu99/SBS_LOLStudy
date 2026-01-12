using Unity.VisualScripting;
using UnityEngine;

public class TargetValidator : MonoBehaviour
{
    [SerializeField] private TeamID ID;

    public bool IsValidTargetForBasicAttack(GameObject targetObj)
    {
        if(!TryGetUnitIdentity(targetObj, out UnitIdentity identity))
        {
            return false;
        }

        if(!identity.IsAlive)
            return false;

        if(!identity.IsTargetable)
            return false;

        if (!IsEnemy(identity))
            return false;

        return true;
    }

    private bool TryGetUnitIdentity(GameObject obj, out UnitIdentity identity)
    {
        identity = null;

        return obj != null && obj.TryGetComponent(out identity);
    }

    private bool IsEnemy(UnitIdentity identity)
    {
        return identity.ID != ID;
    }
}
