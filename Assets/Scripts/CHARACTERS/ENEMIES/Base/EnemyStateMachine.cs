public class EnemyStateMachine
{
    public IEnemyState CurrentState { get; private set; }
    public string CurrentStateName { get; private set; }

    public void Initialize(IEnemyState startState)
    {
        CurrentState = startState;
        CurrentState?.Enter();
        CurrentStateName = startState?.GetType().Name;
    }

    public void ChangeState(IEnemyState newState)
    {
        if (CurrentState == newState) return;
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
        CurrentStateName = CurrentState?.GetType().Name;
    }

    public void Update() => CurrentState?.Update();
}

public class EnemyMovementStateMachine : EnemyStateMachine { }
public class EnemyVerticalStateMachine : EnemyStateMachine { }
