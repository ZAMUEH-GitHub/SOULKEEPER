using UnityEngine;

public class PowerUpCollectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public PowerUpDefinition powerUp;
    public bool destroyOnPickup;
    public ParticleSystem collectiblePickupParticles;

    private PlayerPowerUpController playerPowerUpController;

    private void OnTriggerEnter2D(Collider2D other)
    {     
        playerPowerUpController = other.GetComponent<PlayerPowerUpController>();

        if (other.CompareTag("Player"))
        {
            playerPowerUpController.ApplyPowerUp(powerUp);
            Instantiate(collectiblePickupParticles, transform.position, Quaternion.identity);

            if (destroyOnPickup) Destroy(gameObject);
        }
    }
}
