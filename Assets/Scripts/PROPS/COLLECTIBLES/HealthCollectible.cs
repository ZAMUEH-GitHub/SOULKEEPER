using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [Header("Health Collectible Stats")]
    [SerializeField] private int healAmount;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.playerRuntimeStats.health >= player.playerRuntimeStats.maxHealth)
                player.playerRuntimeStats.score += healAmount * 20;
            else
                player.playerRuntimeStats.health += healAmount;

            Destroy(gameObject);
        }
    }
}