/*using UnityEngine;

public class WallJumpUpgradeOLD : MonoBehaviour
{
    public bool isWallJumpUpgraded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPowerUpController powerUpController = collision.GetComponent<PlayerPowerUpController>();
            isWallJumpUpgraded = true;
            powerUpController.WallJumpUpgrader(isWallJumpUpgraded);
            Destroy(gameObject);
        }
    }
}
*/