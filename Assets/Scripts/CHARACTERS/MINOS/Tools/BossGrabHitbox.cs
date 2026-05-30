using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossGrabHitbox : MonoBehaviour, IDamageable
{
    [Header("Boss Core Reference")]
    [Tooltip("Drag the main Boss GameObject here so damage is dealt to the central HP pool.")]
    public BossDamageController bossDamageController;
    [Tooltip("Drag the main Boss GameObject here to notify the combat logic of a successful grab.")]
    public Phase2Controller phase2Controller;

    private Collider2D grabCollider;

    private void Awake()
    {
        grabCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.attachedRigidbody;
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;

                playerRb.position = grabCollider.bounds.center;
                other.transform.position = grabCollider.bounds.center;
            }

            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.FreezeAllInputs();
            }

            PlayerAttachmentController attachment = other.GetComponent<PlayerAttachmentController>();
            if (attachment != null)
            {
                attachment.AttachToPlatform(grabCollider);
            }

            if (phase2Controller != null)
            {
                phase2Controller.PlayerGrabbed();
            }
        }
    }

    public void TakeDamage(int damage, Vector2 damageVector)
    {
        if (bossDamageController != null)
        {
            bossDamageController.TakeDamage(damage, damageVector);
        }
    }
}