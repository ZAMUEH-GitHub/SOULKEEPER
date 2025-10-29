using UnityEngine;

public enum PowerUpType { Jump, Attack, WallSlide, WallJump, Dash }

[CreateAssetMenu(menuName = "Scriptable Objects/PowerUps/Effects/Unlock Ability")]
public class UnlockAbilityEffect : PowerUp
{
    public PowerUpType powerUp;

    public override void Apply(PlayerStatsSO stats)
    {
        switch (powerUp)
        {
            case PowerUpType.Jump: stats.jumpUnlocked = true; break;
            case PowerUpType.Attack: stats.attackUnlocked = true; break;
            case PowerUpType.WallSlide: stats.wallSlideUnlocked = true; break;
            case PowerUpType.WallJump: stats.wallJumpUnlocked = true; break;
            case PowerUpType.Dash: stats.dashUnlocked = true; break;
        }
    }
}
