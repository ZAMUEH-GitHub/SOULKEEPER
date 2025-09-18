using UnityEngine;

public class DashCollectible : MonoBehaviour
{
    public bool isDashUnlocked;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPowerUpController powerUpController = collision.GetComponent<PlayerPowerUpController>();
            isDashUnlocked = true;
            powerUpController.DashUnlocker(isDashUnlocked);
            Destroy(gameObject);
        }
    }
}
