using UnityEngine;

public class PlayerPowerUpController : MonoBehaviour
{
    public PlayerStatsSO playerStats;

    private bool jumpUnlocked;
    private bool attackUnlocked;
    private bool wallSlideUnlocked;
    private bool wallJumpUnlocked;
    private bool dashUnlocked;

    private void Update()
    {
        PlayerStatsUpdater();
    }

    private void PlayerStatsUpdater()
    {
        playerStats.jumpUnlocked = jumpUnlocked;
        playerStats.attackUnlocked = attackUnlocked;
        playerStats.wallSlideUnlocked = wallSlideUnlocked;
        playerStats.wallJumpUnlocked = wallJumpUnlocked;
        playerStats.dashUnlocked = dashUnlocked;
    }
}
