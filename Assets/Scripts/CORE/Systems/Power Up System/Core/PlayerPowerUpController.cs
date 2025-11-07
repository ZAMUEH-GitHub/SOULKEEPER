using UnityEngine;

public class PlayerPowerUpController : MonoBehaviour
{
    private PlayerStatsSO playerStats;

    private void Awake()
    {
        playerStats = PlayerController.Instance.playerRuntimeStats;
    }

    public void ApplyPowerUp(PowerUpDefinition def)
    {
        playerStats.Grant(def);
    }
}
