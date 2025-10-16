/*using UnityEngine;

public class AttackDamageUpgradeOLD : MonoBehaviour
{
    public bool attackDamageUpgraded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPowerUpController powerUpController = collision.GetComponent<PlayerPowerUpController>();
            powerUpController.AttackDamageUpgrader(attackDamageUpgraded);
            Destroy(gameObject);
        }
    }
}
*/