using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerAnimationController animationController;
    private PlayerAttackController attackController;

    [Header("Animation Driven Variables")]
    public float attackProgress;

    private void Start()
    {
        animationController = PlayerController.Instance.animController;
        attackController = PlayerController.Instance.attackController;
    }

    private void Update()
    {
        if (attackController != null && attackController.IsAttacking)
        {
            attackController.UpdateAttackColliderPosition(attackProgress);
        }
    }

    public void AttackEnd()
    {
        if (animationController != null)
            animationController.OnAttackAnimationEnd();
    }
}