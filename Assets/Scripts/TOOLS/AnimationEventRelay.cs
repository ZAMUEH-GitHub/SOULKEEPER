using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerAnimationController animationController;
    private PlayerAttackController attackController;
    private EnemyBaseController enemyController;

    [HideInInspector] public float attackProgress;
    [HideInInspector] public float speedMultiplier;

    private void Start()
    {
        if (PlayerController.Instance != null && transform.IsChildOf(PlayerController.Instance.transform))
        {
            animationController = PlayerController.Instance.animController;
            attackController = PlayerController.Instance.attackController;
        }

        enemyController = GetComponentInParent<EnemyBaseController>();
    }

    private void Update()
    {
        if (enemyController != null)
        {
            enemyController.moveSpeedMultiplier = speedMultiplier;
        }
    }

    public void AttackEnd()
    {
        if (attackController != null)
        {
            attackController.EndAttack();
        }
        else if (enemyController != null)
        {
            enemyController.EndAttack();
        }
    }
}