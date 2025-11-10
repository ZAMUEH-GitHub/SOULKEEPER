using UnityEngine;

public class PlayerPowerUpController : MonoBehaviour, IPlayerSubController
{
    private PlayerStatsSO playerStats;

    public void Initialize(PlayerStatsSO stats)
    {
        playerStats = stats;
    }

    private void OnEnable()
    {
        AltarController.OnPowerUpUnlocked += ApplyPowerUp;
    }

    private void OnDisable()
    {
        AltarController.OnPowerUpUnlocked -= ApplyPowerUp;
    }

    public void ApplyPowerUp(PowerUpDefinition def)
    {
        if (playerStats == null)
        {
            Debug.LogWarning("[PlayerPowerUpController] Player stats not initialized yet!");
            return;
        }

        playerStats.Grant(def);
        Debug.Log($"[PlayerPowerUpController] Power-up applied: {def.name}");
    }
}
