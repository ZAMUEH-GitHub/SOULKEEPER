using UnityEngine;

public interface IKnockbackable
{
    void Knockback(Vector2 knockbackVector, float knockbackForce, float knockbackDuration);
}
