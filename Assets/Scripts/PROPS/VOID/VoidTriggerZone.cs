using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class VoidTriggerZone : MonoBehaviour
{
    public enum VoidType { InstantDeath, BackToGround }

    [Header("Void Settings")]
    public VoidType voidBehavior = VoidType.BackToGround;

    [Header("Damage Settings")]
    public int voidDamage = 1;

    private bool isProcessingVoid;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isProcessingVoid && other.CompareTag("Player"))
        {
            StartCoroutine(HandleVoidFallRoutine());
        }
    }

    private IEnumerator HandleVoidFallRoutine()
    {
        var player = PlayerController.Instance;
        if (!player.isAlive) yield break;

        isProcessingVoid = true;

        if (voidBehavior == VoidType.BackToGround && player.damageController != null && player.damageController.playerHealth <= voidDamage)
        {
            player.stateMachine.ChangeState(player.deathState);
            player.damageController.TakeDamage(voidDamage, Vector2.up);
            isProcessingVoid = false;
            yield break;
        }

        if (voidBehavior == VoidType.InstantDeath)
        {
            player.stateMachine.ChangeState(player.deathState);
            isProcessingVoid = false;
        }
        else if (voidBehavior == VoidType.BackToGround)
        {
            player.FreezeAllInputs();

            var rb = player.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;

            var canvasManager = CanvasManager.Instance;
            float fadeDuration = 0.5f;

            if (canvasManager != null)
            {
                fadeDuration = canvasManager.GetFadeDuration(PanelType.BlackScreen);
                canvasManager.FadeIn(PanelType.BlackScreen);
            }

            yield return new WaitForSecondsRealtime(fadeDuration);

            if (player.damageController != null)
            {
                player.damageController.TakeDamage(voidDamage, Vector2.up);
            }

            player.transform.position = player.lastSafePosition;
            rb.gravityScale = 1f;
            player.stateMachine.ChangeState(player.respawnState);

            if (canvasManager != null)
            {
                canvasManager.FadeOut(PanelType.BlackScreen);
            }

            yield return new WaitForSecondsRealtime(fadeDuration);

            player.UnfreezeAllInputs();
            isProcessingVoid = false;
        }
    }
}