using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("Enemy Type")]
    public EnemyBehaviorType behaviorType = EnemyBehaviorType.Aggressive;

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
    [Space(5)]
    public float patrolRadius;

    [Header("Vision & Search Settings")]
    public float visionRange;
    public float visionAngle;
    public float proximityRange;
    [Space(5)]
    public float chaseRange;
    public float chaseAngle;
    [Space(5)]
    public float searchRadius;
    public float searchDuration;

    [Header("Jump Stats")]
    public bool canJump;
    public float jumpForce;
    public float jumpRate;
    public bool hasJumpCharge;

    [Header("Attack Stats")]
    public int damage;
    [Space(5)]
    public float attackRate;
    public float attackRange;
    public EnemyAttackType attackType = EnemyAttackType.Simple;
    public int simpleAttackVariations = 1;
    [Space(5)]
    public float chargeAttackSpeed;
    public float chargeAttackDuration;
    [Space(5)]
    public int maxComboSteps;
    public float[] comboStepRanges;

    [Header("Knockback Stats")]
    public float knockback;
    public float knockbackDuration;
    [Space(5)]
    public float stunDuration;

    public enum EnemyBehaviorType
    {
        Aggressive,
        Fearful,
        Defensive,
        Neutral
    }

    public enum EnemyAttackType
    {
        Simple,
        Charge,
        Combo
    }
}