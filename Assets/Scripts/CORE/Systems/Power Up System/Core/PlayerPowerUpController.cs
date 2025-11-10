using UnityEngine;

public class PlayerPowerUpController : MonoBehaviour
{
    private PlayerStatsSO playerStats;

    private void Awake()
    {
        playerStats = GameManager.RuntimePlayerStats;
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerController>()?.playerBaseStats;
        }
    }

    public void ApplyPowerUp(PowerUpDefinition def)
    {
        playerStats.Grant(def);
    }
}