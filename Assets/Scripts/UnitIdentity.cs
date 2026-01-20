using UnityEngine;

public enum UnitType { Champion, Minion, Monster, Structure }

public enum TeamID { None, Blue, Red, Neutral }


public class UnitIdentity : MonoBehaviour
{
    [SerializeField] private UnitType _unitType;
    [SerializeField] private TeamID _teamID;

    public UnitType Type => _unitType;
    public TeamID Team => _teamID;
    public bool IsAlive { get; set; } = true;
    public bool IsTargetable { get; set; } = true;

    public bool IsAlly(UnitIdentity other)
    {
        if(other == null) return false;
        return _teamID != TeamID.None && _teamID == other.Team;
    }

    public bool IsEnemy(UnitIdentity other)
    {
        if(other == null || _teamID == TeamID.None || other.Team == TeamID.None) return false;
        return _teamID != other.Team;
    }
}
