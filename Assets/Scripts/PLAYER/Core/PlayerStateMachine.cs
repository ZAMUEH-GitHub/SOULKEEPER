public class PlayerStateMachine
{
    public IPlayerState CurrentState { get; private set; }
    public string CurrentStateName { get; private set; }

    public void Initialize(IPlayerState startState)
    {
        CurrentState = startState;
        CurrentState?.Enter();
        CurrentStateName = startState?.GetType().Name;
    }

    public void ChangeState(IPlayerState newState)
    {
        if (CurrentState == newState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();

        CurrentStateName = CurrentState?.GetType().Name;
    }

    public void Update() => CurrentState?.Update();
    public void FixedUpdate() => CurrentState?.FixedUpdate();
}