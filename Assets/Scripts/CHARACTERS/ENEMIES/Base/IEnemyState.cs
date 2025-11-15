using UnityEngine;

public interface IEnemyState
{
    void Enter();
    void Update();
    void Exit();
}

public interface IMovementState : IEnemyState { }
public interface IVerticalState : IEnemyState { }
