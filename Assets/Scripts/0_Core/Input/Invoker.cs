using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Invoker : MonoBehaviour
{
    private struct TimedCommand
    {
        public ICommand command;
        public float time;
    }

    [SerializeField] private float _commandLifeTime = 0.25f;

    private Queue<TimedCommand> _commands = new();

    public int Count => _commands.Count;

    public void ExecuteCommand()
    {
        if (_commands.Count == 0) return;
        
        var timed = _commands.Dequeue();

        if (Time.time - timed.time > _commandLifeTime)
            return;

        timed.command.Execute();
    }

    public void EnqueueCommand(ICommand command)
    {
        if(command == null) return;
        _commands.Enqueue(new TimedCommand
        {
            command = command,
            time = Time.time
        });
    }

    public void ClearCommand() => _commands.Clear();

    private void LateUpdate()
    {
        ExecuteCommand();
    }
}
