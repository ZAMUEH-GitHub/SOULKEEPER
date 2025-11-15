using UnityEngine;

public enum StatType
{
    MaxHealth, MoveSpeed, MaxJumpCount, DashRate, DashLenght, AttackSpeed, AttackDamage
}

public enum ModifyMode { Add, Subtract, Set }

[CreateAssetMenu(menuName = "Scriptable Objects/PowerUps/Effects/Modify Stat")]
public class ModifyStatEffect : PowerUp
{
    public StatType stat;
    public ModifyMode mode = ModifyMode.Add;
    public float value;

    public override void Apply(PlayerStatsSO stats)
    {
        switch (stat)
        {
            case StatType.MaxHealth:
                if (mode == ModifyMode.Add) stats.maxHealth += Mathf.RoundToInt(value);
                else if (mode == ModifyMode.Subtract) stats.health -= Mathf.RoundToInt(value);
                else stats.health = Mathf.RoundToInt(value);
                break;

            case StatType.MoveSpeed:
                if (mode == ModifyMode.Add) stats.speed += value;
                else if (mode == ModifyMode.Subtract) stats.speed -= value;
                else stats.speed = value;
                break;

            case StatType.MaxJumpCount:
                if (mode == ModifyMode.Add) stats.maxJumpCount += Mathf.RoundToInt(value);
                else if (mode == ModifyMode.Subtract) stats.maxJumpCount -= Mathf.RoundToInt(value);
                else stats.maxJumpCount = Mathf.RoundToInt(value);
                break;

            case StatType.DashRate:
                if (mode == ModifyMode.Add) stats.dashRate += value;
                else if (mode == ModifyMode.Subtract) stats.dashRate -= value;
                else stats.dashRate = value;
                break;

            case StatType.DashLenght:
                if (mode == ModifyMode.Add) stats.dashLenght += value;
                else if (mode == ModifyMode.Subtract) stats.dashLenght -= value;
                else stats.dashLenght = value;
                break;

            case StatType.AttackSpeed:
                if (mode == ModifyMode.Add) stats.attackRate += value;
                else if (mode == ModifyMode.Subtract) stats.attackRate -= value;
                else stats.attackRate = value;
                break;

            case StatType.AttackDamage:
                if (mode == ModifyMode.Add) stats.damage += Mathf.RoundToInt(value);
                else if (mode == ModifyMode.Subtract) stats.damage -= Mathf.RoundToInt(value);
                else stats.damage = Mathf.RoundToInt(value);
                break;
        }
    }
}
