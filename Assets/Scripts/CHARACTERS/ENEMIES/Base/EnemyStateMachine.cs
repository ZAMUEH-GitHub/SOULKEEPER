public class EnemyStateMachine
{
    public IEnemyState CurrentState { get; private set; }
    public string CurrentStateName { get; private set; }

    public void Initialize(IEnemyState startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
        CurrentStateName = CurrentState.GetType().Name;
    }

    public void ChangeState(IEnemyState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        CurrentStateName = CurrentState.GetType().Name;
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}
