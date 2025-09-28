/*using UnityEngine;

public class DoubleJumpCollectibleOLD : MonoBehaviour
{
    public bool isDoubleJumpUnlocked;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPowerUpController powerUpController = collision.GetComponent<PlayerPowerUpController>();
            isDoubleJumpUnlocked = true;
            powerUpController.DoubleJumpUnlocker(isDoubleJumpUnlocked);
            Destroy(gameObject);
        }
    }
}
*/