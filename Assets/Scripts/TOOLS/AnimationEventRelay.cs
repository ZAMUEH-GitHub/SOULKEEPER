using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerAnimationController animationController;
    private PlayerAttackController attackController;
    private PlayerDeathController deathController;
    private EnemyBaseController enemyController;
    private PlayerAudioController audioController;
    private EnemyAudioController enemyAudioController;
    private SpiderEnemyController spiderEnemyController;
    private Phase2Controller phase2Controller;
    private BossDamageController bossDamageController;

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
        spiderEnemyController = GetComponentInParent<SpiderEnemyController>();
        enemyAudioController = GetComponentInParent<EnemyAudioController>();
        phase2Controller = GetComponentInParent<Phase2Controller>();
        bossDamageController = GetComponentInParent<BossDamageController>();
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
        if (attackController != null) 
            attackController.EndAttack();
        else if (enemyController != null) 
            enemyController.EndAttack();
        else if (spiderEnemyController != null) 
            spiderEnemyController.EndAttack();
        else if (phase2Controller != null)
            phase2Controller.EndAttack();
    }

    public void ApplyKnockback()
    {
        phase2Controller.ApplyPlayerKnockback();
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
        if (deathController != null) deathController.TriggerDeathEffects();
        else if (enemyController != null) enemyController.Destroy();
        else if (spiderEnemyController != null) spiderEnemyController.EndDeath();
        else if (bossDamageController != null) bossDamageController.EndDeath();
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

    public void PlayeSmashAttackSound()
    {
        if (enemyAudioController != null)
            enemyAudioController.PlaySmashAttackSound();
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
    public void SendFlag(string flagID) 
    {
        if (SessionManager.Instance != null && flagID != null)
        {
            SessionManager.Instance.UnlockProgressFlag(flagID);
        }
    }
}