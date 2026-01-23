using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class TargetValidator : MonoBehaviour
{
    private UnitIdentity _myIdentity;

    private void Awake()
    {
        _myIdentity = GetComponent<UnitIdentity>();
    }

    public bool IsValidTargetForSkill(GameObject targetObj, TargetFilter filter)
    {
        if (targetObj == null) return false;

        if (!TryGetUnitIdentity(targetObj, out UnitIdentity identity)) return false;
        if (!identity.IsAlive || !identity.IsTargetable) return false;

        TargetFilter targetBit = identity.Type switch
        {
            UnitType.Champion => TargetFilter.Champion,
            UnitType.Minion => TargetFilter.Minion,
            UnitType.Monster => TargetFilter.Monster,
            UnitType.Structure => TargetFilter.Structure,
            _ => TargetFilter.None
        };

        return (filter & targetBit) != 0;
    }

    public bool IsValidTargetForBasicAttack(GameObject targetObj)
    {
        if(targetObj == null) return false;

        if(!TryGetUnitIdentity(targetObj, out UnitIdentity identity))
        {
            return false;
        }

        if(!identity.IsAlive)
            return false;

        if (!identity.IsTargetable)
            return false;

        if(_myIdentity != null && !_myIdentity.IsEnemy(identity))
        {
            return false;
        }

        return true;
    }

    private bool TryGetUnitIdentity(GameObject obj, out UnitIdentity identity)
    {
        identity = null;
        return obj != null && obj.TryGetComponent(out identity);
    }
}
