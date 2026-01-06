using UnityEngine;

public class PlayerCommand : ICommand
{
    private readonly PlayerController _controller;
    private readonly PlayerAction _action;
    private readonly InputContext _context;

    public PlayerCommand(PlayerController controller, PlayerAction action, InputContext context)
    {
        _controller = controller;
        _action = action;
        _context = context;
    }

    public void Execute()
    {
        
    }
}
