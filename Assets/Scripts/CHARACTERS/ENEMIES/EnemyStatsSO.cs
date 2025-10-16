using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("General Stats")]
    public int health;
    public int score;
    public bool unlocker;

    [Header("Movement Stats")]
    public float speed;
    public float idleDuration;
    [Space(5)]
    public bool canRun;
    public float speedMultiplier;

    [Header("Jump Stats")]
    public bool canJump;
    public float jumpForce;
    public float jumpRate;
    public float jumpChargeDuration;

    [Header("Attack Stats")]
    public int damage;
    public bool canComboAttack;
    public bool hasChargedAttack;
    [Space(5)]
    public float attackRate;
    public float attackRange;
    [Space(5)]
    public float chargeAttackSpeed;
    public float chargeAttackDuration;

    [Header("Knockback Stats")]
    public float knockback;
    public float knockbackDuration;
}
