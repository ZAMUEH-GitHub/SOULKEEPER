using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Scriptable Objects/PlayerStatsSO")]
public class PlayerStatsSO : ScriptableObject
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
    public float knockback;
    public float knockbackLenght;
}
