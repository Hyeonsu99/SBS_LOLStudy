using System.Linq.Expressions;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerController _controller;
    private Invoker _invoker;
    private Camera _mainCamera;

    public GameObject HoverTarget { get; private set; }

    private void Awake()
    {
        _invoker = GetComponent<Invoker>();
        _controller = GetComponent<PlayerController>();
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {

        }

        if(Input.GetKeyDown(KeyCode.W))
        {

        }

        if(Input.GetKeyDown(KeyCode.E))
        {

        }

        if(Input.GetKeyDown(KeyCode.R))
        {

        }

        if(Input.GetKeyDown(KeyCode.D))
        {

        }

        if (Input.GetKeyDown(KeyCode.F))
        {

        }

        if(Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                Enqueue(PlayerAction.Move, new InputContext{ position = hit.point, target = hit.transform.gameObject });
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
        if(Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
        {
            HoverTarget = hit.transform.gameObject;
        }
        else
        {
            HoverTarget = null;
        }
    }
}
