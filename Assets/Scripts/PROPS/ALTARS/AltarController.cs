using UnityEngine;

public class AltarController : MonoBehaviour, IAltar, IInteractable
{
    [Header("Altar Setup")]
    [SerializeField] private PowerUpDefinition powerUp;

    private bool used;
    private PlayerPowerUpController player;

    public bool IsUsed => used;

    private void Awake()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.GetComponent<PlayerPowerUpController>();
    }

    public void Interact()
    {
        UnlockPowerUp();
    }

    public void UnlockPowerUp()
    {
        var target = player;
        if (target == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.GetComponent<PlayerPowerUpController>();
        }

        target.UnlockPowerUp(powerUp);
    }
}
