using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerAnimationController animationController;

    private void Start()
    {
        animationController = GetComponent<PlayerAnimationController>();
    }

    public void AttackEnd()
    {
        if (animationController != null)
            animationController.OnAttackAnimationEnd();
    }
}
