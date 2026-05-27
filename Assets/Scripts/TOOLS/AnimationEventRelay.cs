using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerAnimationController animationController;
    private PlayerAttackController attackController;
    private PlayerDeathController deathController;
    private EnemyBaseController enemyController;
    private PlayerAudioController audioController;
    private EnemyAudioController enemyAudioController;

    [HideInInspector] public float attackProgress;
    [HideInInspector] public float speedMultiplier = 1f;

    private void Start()
    {
        if (PlayerController.Instance != null && transform.IsChildOf(PlayerController.Instance.transform))
        {
            animationController = PlayerController.Instance.animController;
            attackController = PlayerController.Instance.attackController;
            deathController = PlayerController.Instance.deathController;

            audioController = PlayerController.Instance.GetComponent<PlayerAudioController>();
        }

        enemyController = GetComponentInParent<EnemyBaseController>();

        if (enemyController != null)
        {
            enemyAudioController = enemyController.GetComponent<EnemyAudioController>();
        }
    }

    private void Update()
    {
        if (enemyController != null) 
            enemyController.moveSpeedMultiplier = speedMultiplier;
    }

    public void FinishJumpCharge()
    {
        if (enemyController != null)
            enemyController.FinishJumpCharge();
    }

    public void AttackEnd()
    {
        if (attackController != null) attackController.EndAttack();
        else if (enemyController != null) enemyController.EndAttack();

    }

    public void StartChargeLunge()
    {
        if (enemyController != null)
            enemyController.StartChargeLunge();
    }

    public void ImpactFinished()
    {
        if (enemyController != null)
            enemyController.ImpactFinished();
    }

    public void DeathEnd()
    {
        if (deathController != null) 
            deathController.TriggerDeathEffects();
    }


    public void PlayFootstep()
    {
        if (audioController != null) audioController.PlayFootstepSound();
        else if (enemyAudioController != null) enemyAudioController.PlayFootstepSound();
    }

    public void PlayAttackSound()
    {
        if (audioController != null) audioController.PlayAttackSound();
        else if (enemyAudioController != null) enemyAudioController.PlayAttackSound();
    }

    public void PlayDashSound()
    {
        if (audioController != null) 
            audioController.PlayDashSound();
    }

    public void PlayJumpSound()
    {
        if (audioController != null) audioController.PlayJumpSound();
        else if (enemyAudioController != null) enemyAudioController.PlayJumpSound();
    }

    public void PlayFallSound()
    {
        if (audioController != null) 
            audioController.PlayFallSound();
    }

    public void PlayDamageSound()
    {
        if (enemyAudioController != null) 
            enemyAudioController.PlayDamageSound();
    }

    public void PlayIdleSound()
    {
        if (enemyAudioController != null) 
            enemyAudioController.PlayIdleSound();
    }

    public void PlayPrepareAttackSound()
    {
        if (enemyAudioController != null) 
            enemyAudioController.PlayPrepareAttackSound();
    }

    public void PlayPrepareJumpSound()
    {
        if (enemyAudioController != null)
            enemyAudioController.PlayPrepareJumpSound();
    }
}