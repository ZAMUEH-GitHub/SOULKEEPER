/*using UnityEngine;

public class PlayerPowerUpControllerOLD : MonoBehaviour
{
    [Header("Dash")]
    public bool dashUnlocked;
    public bool dashUpgraded;

    [Header("Double Jump")]
    public bool doubleJumpUnlocked;
    public bool doubleJumpUpgraded;

    [Header("Wall Jump")]
    public bool wallJumpUnlocked;
    public bool wallJumpUpgraded;

    [Header("Attack")]
    public bool attackSpeedUpgraded;
    public bool attackDamageUpgraded;

    public PlayerController playerController;
    public PlayerJumpController jumpController;
    public PlayerAttackController attackController;

    private void Start()
    {
        if (GameManager.Instance.dashUnlocked) DashUnlocker(true);
        if (GameManager.Instance.dashUpgraded) DashUpgrader(true);

        if (GameManager.Instance.doubleJumpUnlocked) DoubleJumpUnlocker(true);
        if (GameManager.Instance.doubleJumpUpgraded) DoubleJumpUpgrader(true);

        if (GameManager.Instance.wallJumpUnlocked) WallJumpUnlocker(true);
        if (GameManager.Instance.wallJumpUpgraded) WallJumpUpgrader(true);

        if (GameManager.Instance.attackSpeedUpgraded) AttackSpeedUpgrader(true);
        if (GameManager.Instance.attackDamageUpgraded) AttackDamageUpgrader(true);

        GameManager.Instance.UpdatePowerUpHUD(); // Make sure it's updated on load
    }

    public void DashUnlocker(bool isDashUnlocked)
    {
        dashUnlocked = true;
        GameManager.Instance.dashUnlocked = true;
        GameManager.Instance.UpdatePowerUpHUD();
    }

    public void DashUpgrader(bool isDashUpgraded)
    {
        dashUpgraded = true;
        playerController.dashRate = 1;
        GameManager.Instance.dashUpgraded = true;
        GameManager.Instance.UpdatePowerUpHUD();
    }

    public void DoubleJumpUnlocker(bool isDoubleJumpunlocked)
    {
        doubleJumpUnlocked = true;
        jumpController.maxJumpCount++;
        GameManager.Instance.doubleJumpUnlocked = true;
        GameManager.Instance.UpdatePowerUpHUD();
    }

    public void DoubleJumpUpgrader(bool isDoubleJumpUpgraded)
    {
        doubleJumpUpgraded = true;
        jumpController.maxJumpCount++;
        GameManager.Instance.doubleJumpUpgraded = true;
        GameManager.Instance.UpdatePowerUpHUD();
    }

    public void WallJumpUnlocker(bool isWallJumpUnlocked)
    {
        wallJumpUnlocked = true;
        GameManager.Instance.wallJumpUnlocked = true;
        GameManager.Instance.UpdatePowerUpHUD();
    }

    public void WallJumpUpgrader(bool isWallJumpUpgraded)
    {
        wallJumpUpgraded = true;
        playerController.wallJumpForce = 25;
        playerController.wallJumpDivider = 3;
        GameManager.Instance.wallJumpUpgraded = true;
        GameManager.Instance.UpdatePowerUpHUD();
    }

    public void AttackSpeedUpgrader(bool isAttackSpeedUpgraded)
    {
        attackSpeedUpgraded = true;
        attackController.attackRate = 0.25f;
        GameManager.Instance.attackSpeedUpgraded = true;
        GameManager.Instance.UpdatePowerUpHUD();
    }

    public void AttackDamageUpgrader(bool isAttackDamageUpgraded)
    {
        attackDamageUpgraded = true;
        attackController.playerDamage++;
        GameManager.Instance.attackDamageUpgraded = true;
        GameManager.Instance.UpdatePowerUpHUD();
    }
}*/