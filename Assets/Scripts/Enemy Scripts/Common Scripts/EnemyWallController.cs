using UnityEngine;

public class EnemyWallController : MonoBehaviour
{
    public bool isWalled;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle"))
        {
            isWalled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle"))
        {
            isWalled = false;
        }
    }
}
