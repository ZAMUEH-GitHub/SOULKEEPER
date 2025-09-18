using UnityEngine;

public class AttackingEnemyRangeController : MonoBehaviour
{
    public GameObject enemyObject;
    public AttackingEnemyController attackingEnemyController;

    private void Update()
    {
        transform.position = enemyObject.transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            attackingEnemyController.enemyTarget = attackingEnemyController.playerTargetVector;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            attackingEnemyController.enemyTarget = attackingEnemyController.enemyStartPosition;
        }
    }
}
