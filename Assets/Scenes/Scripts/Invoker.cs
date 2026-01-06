using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invoker : MonoBehaviour
{
    private Queue<PlayerCommand> _commands = new();

    public void ExecuteCommand()
    {
        var command = _commands.Dequeue();

        command.Execute();
    }

    public void EnqueueCommand(PlayerCommand command)
    {
        _commands.Enqueue(command);
    }

    public void DequeueCommand()
    {
        _commands.Dequeue();
    }
}
