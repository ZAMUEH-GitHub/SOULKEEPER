using UnityEngine;

public class WallJumpCollectible : MonoBehaviour
{
    public bool isWallJumpUnlocked;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPowerUpController powerUpController = collision.GetComponent<PlayerPowerUpController>();
            isWallJumpUnlocked = true;
            powerUpController.WallJumpUnlocker(isWallJumpUnlocked);
            Destroy(gameObject);
        }
    }
}
