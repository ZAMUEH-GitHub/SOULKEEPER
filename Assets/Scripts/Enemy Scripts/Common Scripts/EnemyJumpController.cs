using UnityEngine;

public class EnemyJumpController : MonoBehaviour
{
    public bool isGrounded;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle"))
        {
            isGrounded = false;
        }
    }
}
