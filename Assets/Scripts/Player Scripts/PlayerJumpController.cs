using UnityEngine;

public class PlayerJumpController : MonoBehaviour
{
    [Header("Jump Parameters")]
    public bool isGrounded;
    public int jumpCount;
    public int maxJumpCount;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle") || collision.CompareTag("Bulb"))
        {
            isGrounded = true;
            jumpCount = maxJumpCount;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle") || collision.CompareTag("Bulb"))
        {   
            isGrounded = false;
        }
    }
}
