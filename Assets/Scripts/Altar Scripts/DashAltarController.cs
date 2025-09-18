using UnityEngine;
using UnityEngine.InputSystem;

public class DashAltarController : MonoBehaviour, IAltar
{
    [SerializeField] private GameObject dashUnlockCollectible;
    [SerializeField] private GameObject dashUpgradeCollectible;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int unlockCost = 100;
    [SerializeField] private int upgradeCost = 200;

    private bool isPlayerInRange = false;
    private bool isUnlocked = false;
    private bool isUpgraded = false;
    private GameObject player;
    private AltarCanvasManager canvasManager;

    private void Start()
    {
        canvasManager = FindFirstObjectByType<AltarCanvasManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = collision.gameObject;
            ShowUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
            HideUI();
        }
    }

    // This gets called from the Input System's Interact action via Unity Events
    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed || !isPlayerInRange) return;
        Interact(); // Delegate to the core interaction logic
    }

    // Core logic
    public void Interact()
    {
        Debug.Log("Interacted with altar");

        int playerScore = GameManager.Instance.playerScore;

        if (!isUnlocked && playerScore >= unlockCost)
        {
            GameManager.Instance.SpendScore(unlockCost);
            Instantiate(dashUnlockCollectible, spawnPoint.position, Quaternion.identity);
            isUnlocked = true;
        }
        else if (isUnlocked && !isUpgraded && playerScore >= upgradeCost)
        {
            GameManager.Instance.SpendScore(upgradeCost);
            Instantiate(dashUpgradeCollectible, spawnPoint.position, Quaternion.identity);
            isUpgraded = true;
        }

        UpdateUI();
    }

    public void ShowUI()
    {
        if (!isUnlocked)
            canvasManager.ShowAltarInfo("Unlock Dash", unlockCost);
        else if (!isUpgraded)
            canvasManager.ShowAltarInfo("Upgrade Dash", upgradeCost);
        else
            canvasManager.HideAltarInfo();
    }

    public void HideUI()
    {
        canvasManager.HideAltarInfo();
    }

    private void UpdateUI()
    {
        ShowUI(); // Refresh based on new state
    }
}
