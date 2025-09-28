using UnityEngine;

public class EnemyRangeController : MonoBehaviour
{
    public IEnemy enemyController;

    private void Start()
    {
        enemyController = GetComponentInParent<IEnemy>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemyController.SetTargetPlayer(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemyController.SetTargetPlayer(false);
        }
    }
}