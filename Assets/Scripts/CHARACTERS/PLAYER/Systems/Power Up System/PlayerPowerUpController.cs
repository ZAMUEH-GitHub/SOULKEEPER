using UnityEngine;

public class PlayerPowerUpController : MonoBehaviour
{
    public PlayerStatsSO playerStats;

    public void UnlockPowerUp(PowerUpDefinition def)
    {
        playerStats.Grant(def);
    }

}
