using UnityEngine;

public class DoubleJumpUpgrade : MonoBehaviour
{
    public bool isDoubleJumpUpgraded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPowerUpController powerUpController = collision.GetComponent<PlayerPowerUpController>();
            isDoubleJumpUpgraded = true;
            powerUpController.DoubleJumpUpgrader(isDoubleJumpUpgraded);
            Destroy(gameObject);
        }
    }
}
