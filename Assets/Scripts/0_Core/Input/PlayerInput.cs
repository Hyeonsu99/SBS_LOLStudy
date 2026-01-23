using System.Linq.Expressions;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerController _controller;
    private UnitStat _unitStat;
    private Invoker _invoker;
    private Camera _mainCamera;

    public GameObject HoverTarget { get; private set; }

    private void Awake()
    {
        _invoker = GetComponent<Invoker>();
        _unitStat = GetComponent<UnitStat>();
        _controller = GetComponent<PlayerController>();
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                Enqueue(PlayerAction.CastSkill, new InputContext { position = hit.point, target = hit.transform.gameObject == null ? HoverTarget : hit.transform.gameObject, skillCommand = SkillCommand.Q });
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                Enqueue(PlayerAction.CastSkill, new InputContext { position = hit.point, target = hit.transform.gameObject, skillCommand = SkillCommand.W });
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                Enqueue(PlayerAction.CastSkill, new InputContext { position = hit.point, target = hit.transform.gameObject, skillCommand = SkillCommand.E });
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                Enqueue(PlayerAction.CastSkill, new InputContext { position = hit.point, target = hit.transform.gameObject, skillCommand = SkillCommand.R });
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                Enqueue(PlayerAction.CastSummoner, new InputContext { position = hit.point, target = hit.transform.gameObject, summonerCommand = SummonerCommand.D });
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                Enqueue(PlayerAction.CastSummoner, new InputContext { position = hit.point, target = hit.transform.gameObject, summonerCommand = SummonerCommand.F });
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (_unitStat.IsRoot)
                return;

            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                Enqueue(PlayerAction.Move, new InputContext { position = hit.point, target = hit.transform.gameObject });
            }
        }

        UpdateMoveHover();
    }

    private void Enqueue(PlayerAction action, InputContext ctx)
    {
        _invoker.EnqueueCommand(new PlayerCommand(_controller, action, ctx));
    }

    private void UpdateMoveHover()
    {
        if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
        {
            HoverTarget = hit.transform.gameObject;
        }
        else
        {
            HoverTarget = null;
        }
    }
}