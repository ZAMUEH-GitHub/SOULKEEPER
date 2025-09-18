using UnityEngine;

public class DashUpgrade : MonoBehaviour
{
    public bool isDashUpgraded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPowerUpController powerUpController = collision.GetComponent<PlayerPowerUpController>();
            isDashUpgraded = true;
            powerUpController.DashUpgrader(isDashUpgraded);
            Destroy(gameObject);
        }
    }
}
