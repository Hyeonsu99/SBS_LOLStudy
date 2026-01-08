using UnityEngine;

public enum UnitType
{
    Champion,
    Minion,
    Monster,
    Structure
}

public enum TeamID
{
    Neutral,
    Blue,
    Red
}

public class UnitIdentity : MonoBehaviour
{
    [SerializeField] private UnitType unitType;
    [SerializeField] private TeamID teamID;

    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool isTargetable = true;
    

    public Transform TransformInfo => transform;
    // Alive와 Targetable은 다른 스크립트를 참조하여 동적으로 변경되게 변경
    public bool IsAlive => isAlive;
    public bool IsTargetable => isTargetable;
    public UnitType Type => unitType;
    public TeamID ID => teamID;
}
