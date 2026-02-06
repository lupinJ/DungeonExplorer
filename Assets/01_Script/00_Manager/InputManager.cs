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

    PlayerInputSystem input;
    MoveEvent moveEvent;
    DashEvent dashEvent;
    InteractEvent interactEvent;

    /// <summary>
    /// ΩÃ±€≈Ê √ ±‚»≠ «‘ºˆ
    /// </summary>
    protected override void Init()
    {
        input = new PlayerInputSystem();
        moveEvent = new MoveEvent();
        dashEvent = new DashEvent();
        interactEvent = new InteractEvent();
    }

    private void Start()
    {
        EventAdd();
    }
    public void EventAdd()
    {
        EventManager.Instance.AddEvent<MoveEvent>(moveEvent);
        EventManager.Instance.AddEvent<DashEvent>(dashEvent);
        EventManager.Instance.AddEvent<InteractEvent>(interactEvent);
    }

    /// <summary>
    /// ΩÃ±€≈Ê º“∏Í Ω√
    /// </summary>
    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.RemoveEvent<MoveEvent>();
            EventManager.Instance.RemoveEvent<DashEvent>();
            EventManager.Instance.RemoveEvent<InteractEvent>();
        }

    }

    private void OnEnable()
    {
        input.GameUI.Enable();
        input.GameUI.SetCallbacks(this);

        input.Player.Enable();
        input.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        input.GameUI.Disable();
        input.GameUI.RemoveCallbacks(this);

        input.Player.Disable();
        input.Player.RemoveCallbacks(this);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        InputState state = InputState.Performed;

        if (context.started) state = InputState.Started;
        else if (context.canceled) state = InputState.Canceled;
        interactEvent.Invoke(state);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        InputState state = InputState.Performed;
        
        if (context.started) state = InputState.Started;
        else if (context.canceled) state = InputState.Canceled;

        moveEvent.Invoke(new MoveArgs()
        {
            dir = context.ReadValue<Vector2>(),
            state = state
        });
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        InputState state = InputState.Performed;

        if (context.started) state = InputState.Started;
        else if (context.canceled) state = InputState.Canceled;
        dashEvent.Invoke(state);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        if (UIManager.Instance.TryGetPanel(AddressKeys.InventoryUI, out UIBase uiBase))
        {
            PopupUI inventory = uiBase as PopupUI;
            if (inventory.IsActive)
                inventory.HidePanel();
            else
                inventory.ShowPanel();
        }
    }

  
}
