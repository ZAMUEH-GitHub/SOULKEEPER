using UnityEngine;

public class PlayerWallController : MonoBehaviour
{
    [Header("Wall Parameters")]
    public bool isWalled;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle"))
        {
            isWalled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle"))
        {
            isWalled = false;
        }
    }
}
