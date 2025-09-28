using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class EnemyBaseController : MonoBehaviour
{
    public EnemyStatsSO stats;
    public Animator Animator { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public Transform Player { get; private set; }

    public EnemyStateMachine stateMachine;

    void Start()
    {
        Animator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        stateMachine = new EnemyStateMachine();
        stateMachine.Initialize(new IdleState(this));
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void ChangeState(IEnemyState newState)
    {
        stateMachine.ChangeState(newState);
    }

    // Helper methods (to be expanded later)
    public void Move(Vector2 direction, float speed) { }
    public void Stop() { }
    public void Flip(Vector2 target) { }
}