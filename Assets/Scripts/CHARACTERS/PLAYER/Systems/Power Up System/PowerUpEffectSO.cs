using UnityEngine;

public abstract class PowerUpEffect : ScriptableObject
{
    public abstract void Apply(PlayerStatsSO stats);
}

public enum PowerUpType { Jump, Attack, WallSlide, WallJump, Dash }

[CreateAssetMenu(menuName = "Scriptable Objects/PowerUps/Effects/Unlock Ability")]
public class UnlockAbilityEffect : PowerUpEffect
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

public enum StatType { MaxHealth, MoveSpeed, MaxJumpCount, DashRate, DashLenght, AttackSpeed, AttackDamage }
public enum ModifyMode { Add, Set }

[CreateAssetMenu(menuName = "Scriptable Objects/PowerUps/Effects/Modify Stat")]
public class ModifyStatEffect : PowerUpEffect
{
    public StatType stat;
    public ModifyMode mode = ModifyMode.Add;
    public float value;

    public override void Apply(PlayerStatsSO stats)
    {
        switch (stat)
        {
            case StatType.MaxHealth:
                if (mode == ModifyMode.Add) stats.health += Mathf.RoundToInt(value);
                else stats.health = Mathf.RoundToInt(value);
                break;

            case StatType.MoveSpeed:
                if (mode == ModifyMode.Add) stats.speed += value;
                else stats.speed = value;
                break;   
                
            case StatType.MaxJumpCount:
                if (mode == ModifyMode.Add) stats.maxJumpCount += Mathf.RoundToInt(value);
                else stats.maxJumpCount = Mathf.RoundToInt(value);
                break;

            case StatType.DashRate:
                if (mode == ModifyMode.Add) stats.dashForce += value;
                else stats.dashRate = value;
                break;

            case StatType.DashLenght:
                if (mode == ModifyMode.Add) stats.dashLenght += value;
                else stats.dashLenght = value;
                break;

            case StatType.AttackSpeed:
                if (mode == ModifyMode.Add) stats.attackRate += value;
                else stats.attackRate = value;
                break;
        }
    }
}
