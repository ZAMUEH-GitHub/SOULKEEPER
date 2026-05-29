using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossGrabHitbox : MonoBehaviour, IDamageable
{
    [Header("Boss Core Reference")]
    [Tooltip("Drag the main Boss GameObject here so damage is dealt to the central HP pool.")]
    public BossDamageController bossDamageController;

    private Collider2D grabCollider;

    private void Awake()
    {
        grabCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.FreezeAllInputs();
            }

            PlayerAttachmentController attachment = other.GetComponent<PlayerAttachmentController>();
            if (attachment != null)
            {
                attachment.AttachToPlatform(grabCollider);
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