using UnityEngine;
using System.Collections;

public interface IEnemy
{
    void SetTargetPlayer(bool targetPlayer);
    void EnemyKnockback(Vector2 knockbackVector, float knockbackForce);
    IEnumerator TakeDamage(int damage, Vector2 damageVector);
}
