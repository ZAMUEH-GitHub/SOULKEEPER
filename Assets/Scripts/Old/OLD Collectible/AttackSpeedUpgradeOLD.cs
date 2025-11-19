/*using UnityEngine;

public class AttackSpeedUpgradeOLD : MonoBehaviour
{
    public bool isAttackSpeedUpgraded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPowerUpController powerUpController = collision.GetComponent<PlayerPowerUpController>();
            isAttackSpeedUpgraded = true;
            powerUpController.AttackSpeedUpgrader(isAttackSpeedUpgraded);
            Destroy(gameObject);
        }
    }
}
*/