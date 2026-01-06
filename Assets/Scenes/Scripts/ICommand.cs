using UnityEngine;

public enum PlayerAction
{
    Move,
    AttackMove,
    CastSkill,
    CastSummoner
}

public enum SkillSlot
{
    Q,W,E,R
}

public enum SummonerSlot
{
    D,F
}

public struct InputContext
{
    public Vector3 position;
    public GameObject target;
    public Vector3 direction;

    public SkillSlot skillSlot;
    public SummonerSlot summonerSlot;

    public static InputContext Empty => default;
}

public interface ICommand
{
    void Execute();
}
