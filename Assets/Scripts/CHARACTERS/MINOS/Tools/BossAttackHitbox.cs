using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossAttackHitbox : MonoBehaviour, IDamageable
{
    [Header("Damage Settings")]
    public int attackDamage = 1;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;

    [Header("Boss Core Reference")]
    [Tooltip("Drag the main Boss GameObject here so damage is dealt to the central HP pool.")]
    public BossDamageController bossDamageController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDamageController playerDamage = other.GetComponent<PlayerDamageController>();

            if (playerDamage != null && !playerDamage.isTakingDamage)
            {
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;

                playerDamage.TakeDamage(attackDamage, knockbackDir);
                playerDamage.Knockback(knockbackDir, knockbackForce, knockbackDuration);
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