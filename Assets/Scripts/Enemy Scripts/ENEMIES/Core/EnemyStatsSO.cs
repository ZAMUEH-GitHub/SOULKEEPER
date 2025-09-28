using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("General Stats")]
    public int health;
    public int score;
    public float speed;
    public bool unlocker;

    [Header("Jump Stats")]
    public bool canJump;
    public float jumpForce;
    public float jumpRate;

    [Header("Attack Stats")]
    public int damage;
    public float knockback;
    public bool canComboAttack;
    public bool hasChargedAttack;
    public float attackRate;
    public float attackRange;
}
