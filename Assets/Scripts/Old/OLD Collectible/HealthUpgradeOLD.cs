/*using UnityEngine;

public class HealthUpgradeOLD : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            playerController.playerMaxHealth ++;
            playerController.Heal(playerController.playerMaxHealth - playerController.playerHealth);
            Destroy(gameObject);
        }
    }
}
*/