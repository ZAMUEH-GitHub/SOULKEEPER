using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerAnimationController animationController;
    private PlayerAttackController attackController;

    [HideInInspector] public float attackProgress;

    private void Start()
    {
        animationController = PlayerController.Instance.animController;
        attackController = PlayerController.Instance.attackController;
    }

    public void AttackEnd()
    {
        if (attackController != null)
            attackController.EndAttack();
    }
}