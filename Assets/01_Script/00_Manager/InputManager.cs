using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputState : byte
{
    Started,
    Performed,
    Canceled
}

public struct MoveArgs
{
    public Vector2 dir;
    public InputState state;
}

public struct InputArgs
{
    public InputState state;
}


public class InputManager : Singleton<InputManager>, 
    PlayerInputSystem.IPlayerActions, 
    PlayerInputSystem.IGameUIActions
{
    public class MoveEvent : GameAction<MoveArgs> { }
    public class DashEvent : GameAction<InputState> { }
    public class InteractEvent : GameAction<InputState> { }
    public class AttackEvent : GameAction<InputState> { }

    PlayerInputSystem input;
    MoveEvent moveEvent;
    DashEvent dashEvent;
    InteractEvent interactEvent;
    AttackEvent attackEvent;

    public Vector2 MoveInput => input.Player.Move.ReadValue<Vector2>();

    /// <summary>
    /// ΩÃ±€≈Ê √ ±‚»≠ «‘ºˆ
    /// </summary>
    protected override void Init()
    {
        input = new PlayerInputSystem();
        moveEvent = new MoveEvent();
        dashEvent = new DashEvent();
        interactEvent = new InteractEvent();
        attackEvent = new AttackEvent();
    }

    private void Start()
    {
        input.GameUI.Enable();
        input.GameUI.SetCallbacks(this);

        input.Player.Enable();
        input.Player.SetCallbacks(this);

        EventAdd();

    }
    public void EventAdd()
    {
        EventManager.Instance.AddEvent<MoveEvent>(moveEvent);
        EventManager.Instance.AddEvent<DashEvent>(dashEvent);
        EventManager.Instance.AddEvent<InteractEvent>(interactEvent);
        EventManager.Instance.AddEvent<AttackEvent>(attackEvent);
    }

    /// <summary>
    /// ΩÃ±€≈Ê º“∏Í Ω√
    /// </summary>
    private void OnApplicationQuit()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.RemoveEvent<MoveEvent>();
            EventManager.Instance.RemoveEvent<DashEvent>();
            EventManager.Instance.RemoveEvent<InteractEvent>();
            EventManager.Instance.RemoveEvent<AttackEvent>();
        }

        input.GameUI.Disable();
        input.GameUI.RemoveCallbacks(this);

        input.Player.Disable();
        input.Player.RemoveCallbacks(this);

    }

    public void InputEnableAll()
    {
        input.GameUI.Enable();
        input.Player.Enable();
    }

    public void InputDisalbeAll()
    {
        input.GameUI.Disable();
        input.Player.Disable();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        InputState state = GetState(context);
        attackEvent.Invoke(state);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        InputState state = GetState(context);
        interactEvent.Invoke(state);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        InputState state = GetState(context);

        moveEvent.Invoke(new MoveArgs()
        {
            dir = context.ReadValue<Vector2>(),
            state = state
        });
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        InputState state = GetState(context);
        dashEvent.Invoke(state);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        if (UIManager.Instance.TryGetPanel(UIName.Inventory, out UIBase uiBase))
        {
            PopupUI inventory = uiBase as PopupUI;
            if (inventory.IsActive)
            {
                input.Player.Enable();
                inventory.HidePanel();
            }
            else
            {
                input.Player.Disable();
                inventory.ShowPanel();
            }
        }
    }

    InputState GetState(InputAction.CallbackContext context)
    {
        InputState state = InputState.Performed;

        if (context.started) state = InputState.Started;
        else if (context.canceled) state = InputState.Canceled;
        return state;
    }
}
