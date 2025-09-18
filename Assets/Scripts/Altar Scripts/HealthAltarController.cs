using UnityEngine;
using UnityEngine.InputSystem;

public class HealthAltarController : MonoBehaviour, IAltar
{
    [SerializeField] private GameObject healthUpgradeCollectible;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int upgradeCost = 100;
    [SerializeField] private int maxUpgrades = 5;

    private int upgradesPurchased = 0;
    private bool isPlayerInRange = false;
    private AltarCanvasManager canvasManager;

    private void Start() => canvasManager = FindFirstObjectByType<AltarCanvasManager>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HideUI();
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed || !isPlayerInRange) return;
        Interact();
    }

    public void Interact()
    {
        if (upgradesPurchased >= maxUpgrades)
        {
            // No more upgrades left
            UpdateUI();
            return;
        }

        int score = GameManager.Instance.playerScore;

        if (score >= upgradeCost)
        {
            GameManager.Instance.SpendScore(upgradeCost);
            Instantiate(healthUpgradeCollectible, spawnPoint.position, Quaternion.identity);
            upgradesPurchased++;
        }

        UpdateUI();
    }

    public void ShowUI()
    {
        if (upgradesPurchased < maxUpgrades)
            canvasManager.ShowAltarInfo($"Health Upgrade ({upgradesPurchased}/{maxUpgrades})", upgradeCost);
        else
            canvasManager.ShowAltarInfo("Health Fully Upgraded", 0);
    }

    public void HideUI() => canvasManager.HideAltarInfo();

    private void UpdateUI() => ShowUI();
}
