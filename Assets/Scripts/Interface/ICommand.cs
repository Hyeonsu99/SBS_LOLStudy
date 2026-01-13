using UnityEngine;

public enum PlayerAction
{
    Move,
    AttackMove,
    CastSkill,
    CastSummoner
}

public enum SkillCommand
{
    Q,W,E,R
}

public enum SummonerCommand
{
    D,F
}

public struct InputContext
{
    public Vector3 position;
    public GameObject target;
    public Vector3 direction;

    public SkillCommand skillSlot;
    public SummonerCommand summonerSlot;

    public static InputContext Empty => default;
}

public interface ICommand
{
    void Execute();
}
