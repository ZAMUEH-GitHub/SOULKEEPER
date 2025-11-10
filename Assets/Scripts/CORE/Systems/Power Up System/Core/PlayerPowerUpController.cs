using UnityEngine;

public class PlayerPowerUpController : MonoBehaviour
{
    public PlayerStatsSO playerStats;

    private void Awake()
    {
        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
        }
    }

    public void ApplyPowerUp(PowerUpDefinition def)
    {
        if (playerStats == null)
        {
            Debug.LogWarning("[PlayerPowerUpController] Player stats not assigned!");
            return;
        }

        playerStats.Grant(def);
    }
}
