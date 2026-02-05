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


public class InputManager : Singleton<InputManager>, PlayerInputSystem.IPlayerActions
{
    public class MoveEvent : GameAction<MoveArgs> { }
    public class DashEvent : GameAction<InputState> { }

    PlayerInputSystem input;
    MoveEvent moveEvent;
    DashEvent dashEvent;

    
    /// <summary>
    /// ΩÃ±€≈Ê √ ±‚»≠ «‘ºˆ
    /// </summary>
    protected override void Init()
    {
        input = new PlayerInputSystem();
        moveEvent = new MoveEvent();
        dashEvent = new DashEvent();
    }

    private void Start()
    {
        EventAdd();
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
        }

    }

    private void OnEnable()
    {
        input.Player.Enable();
        input.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        input.Player.Disable();
        input.Player.RemoveCallbacks(this);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteractive(InputAction.CallbackContext context)
    {
        
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

    public void EventAdd()
    {
        EventManager.Instance.AddEvent<MoveEvent>(moveEvent);
        EventManager.Instance.AddEvent<DashEvent>(dashEvent);
    }
}
