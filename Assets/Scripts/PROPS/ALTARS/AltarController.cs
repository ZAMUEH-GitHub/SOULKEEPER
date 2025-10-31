using UnityEngine;

public class AltarController : MonoBehaviour, IAltar, IInteractable
{
    [Header("Altar Setup")]
    [SerializeField] private PowerUpDefinition powerUp;
    [SerializeField] private bool oneShot = true;

    [Header("Feedback (optional)")]
    [SerializeField] private ParticleSystem useVFX;
    [SerializeField] private AudioSource useSFX;
    [SerializeField] private GameObject disableOnUse;

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
        if (used && oneShot) return;

        var target = player;
        if (target == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.GetComponent<PlayerPowerUpController>();
        }

        target.UnlockPowerUp(powerUp);

        if (oneShot) used = true;

        if (useVFX != null)
            Instantiate(useVFX, transform.position, Quaternion.identity);

        if (useSFX != null)
            useSFX.Play();

        if (disableOnUse != null && oneShot)
            disableOnUse.SetActive(false);

        if (oneShot)
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }
}
