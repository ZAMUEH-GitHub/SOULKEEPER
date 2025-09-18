using UnityEngine;
using System.Collections;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Attack Settings")]
    public int enemyDamage;

    [Header("Knockback Settings")]
    public Vector2 knockbackVector;
    public float knockbackForce;
    public float knockbackDuration;

    private IEnemy enemyController;
    public GameObject enemyObject;

    private void Start()
    {
        enemyController = GetComponentInParent<IEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null && enemyObject != null)
            {
                knockbackVector = (collision.transform.position - enemyObject.transform.position).normalized;

                enemyController?.EnemyKnockback(knockbackVector, knockbackForce / 3);

                player.PlayerKnockback(knockbackVector, knockbackForce, knockbackDuration);
                player.TakeDamage(enemyDamage, knockbackVector);
            }
        }
    }
}