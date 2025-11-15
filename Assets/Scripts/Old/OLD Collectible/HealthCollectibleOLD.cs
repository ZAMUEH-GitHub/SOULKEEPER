/*using UnityEngine;

public class HealthCollectibleOLD : MonoBehaviour
{
    public int healAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            playerController.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
*/