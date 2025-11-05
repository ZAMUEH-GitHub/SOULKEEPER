using UnityEngine;

public class PlayerPowerUpController : MonoBehaviour
{
    public PlayerStatsSO playerStats;

    public void ApplyPowerUp(PowerUpDefinition def)
    {
        playerStats.Grant(def);
    }

}
