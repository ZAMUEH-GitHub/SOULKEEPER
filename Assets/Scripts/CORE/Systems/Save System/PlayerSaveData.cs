using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    [Header("General Stats")]
    public int health;
    public int score;
    public float speed;

    [Header("Jump Stats")]
    public int maxJumpCount;
    public float jumpForce;
    public float jumpRate;

    [Header("Wall Stats")]
    public float wallSlidingSpeed;
    public float wallJumpForce;
    public float wallJumpLenght;
    public float wallJumpRate;

    [Header("Dash Stats")]
    public float dashForce;
    public float dashRate;
    public float dashLenght;

    [Header("Attack Stats")]
    public int damage;
    public float attackRate;
    public float attackRange;
    public float knockback;
    public float knockbackLenght;

    [Header("Damage Stats")]
    public float damageRate;
    public float damageForce;
    public float damageLenght;

    [Header("Player Abilities")]
    public bool jumpUnlocked;
    public bool wallSlideUnlocked;
    public bool wallJumpUnlocked;
    public bool dashUnlocked;
    public bool attackUnlocked;

    [Header("Meta Data")]
    public int saveSlot;
    public string timestamp;
    public float playtime;
    public string version = "0.1.0";

    public void FromRuntime(PlayerStatsSO runtimeStats, int slot)
    {
        health = runtimeStats.health;
        score = runtimeStats.score;
        speed = runtimeStats.speed;

        maxJumpCount = runtimeStats.maxJumpCount;
        jumpForce = runtimeStats.jumpForce;
        jumpRate = runtimeStats.jumpRate;

        wallSlidingSpeed = runtimeStats.wallSlidingSpeed;
        wallJumpForce = runtimeStats.wallJumpForce;
        wallJumpLenght = runtimeStats.wallJumpLenght;
        wallJumpRate = runtimeStats.wallJumpRate;

        dashForce = runtimeStats.dashForce;
        dashRate = runtimeStats.dashRate;
        dashLenght = runtimeStats.dashLenght;

        damage = runtimeStats.damage;
        attackRate = runtimeStats.attackRate;
        attackRange = runtimeStats.attackRange;
        knockback = runtimeStats.knockback;
        knockbackLenght = runtimeStats.knockbackLenght;

        damageRate = runtimeStats.damageRate;
        damageForce = runtimeStats.damageForce;
        damageLenght = runtimeStats.damageLenght;

        saveSlot = slot;
        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        playtime = Time.time;
    }

    public void ApplyToRuntime(PlayerStatsSO runtimeStats)
    {
        runtimeStats.health = health;
        runtimeStats.score = score;
        runtimeStats.speed = speed;

        runtimeStats.maxJumpCount = maxJumpCount;
        runtimeStats.jumpForce = jumpForce;
        runtimeStats.jumpRate = jumpRate;

        runtimeStats.wallSlidingSpeed = wallSlidingSpeed;
        runtimeStats.wallJumpForce = wallJumpForce;
        runtimeStats.wallJumpLenght = wallJumpLenght;
        runtimeStats.wallJumpRate = wallJumpRate;

        runtimeStats.dashForce = dashForce;
        runtimeStats.dashRate = dashRate;
        runtimeStats.dashLenght = dashLenght;

        runtimeStats.damage = damage;
        runtimeStats.attackRate = attackRate;
        runtimeStats.attackRange = attackRange;
        runtimeStats.knockback = knockback;
        runtimeStats.knockbackLenght = knockbackLenght;

        runtimeStats.damageRate = damageRate;
        runtimeStats.damageForce = damageForce;
        runtimeStats.damageLenght = damageLenght;
    }
}
