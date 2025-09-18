using UnityEngine;
using UnityEngine.InputSystem;

public class WallJumpAltarController : MonoBehaviour, IAltar
{
    [SerializeField] private GameObject wallJumpUnlockCollectible;
    [SerializeField] private GameObject wallJumpUpgradeCollectible;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int unlockCost = 100;
    [SerializeField] private int upgradeCost = 200;

    private bool isPlayerInRange = false;
    private bool isUnlocked = false;
    private bool isUpgraded = false;
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
        int score = GameManager.Instance.playerScore;

        if (!isUnlocked && score >= unlockCost)
        {
            GameManager.Instance.SpendScore(unlockCost);
            Instantiate(wallJumpUnlockCollectible, spawnPoint.position, Quaternion.identity);
            isUnlocked = true;
        }
        else if (isUnlocked && !isUpgraded && score >= upgradeCost)
        {
            GameManager.Instance.SpendScore(upgradeCost);
            Instantiate(wallJumpUpgradeCollectible, spawnPoint.position, Quaternion.identity);
            isUpgraded = true;
        }

        UpdateUI();
    }

    public void ShowUI()
    {
        if (!isUnlocked)
            canvasManager.ShowAltarInfo("Unlock Wall Jump", unlockCost);
        else if (!isUpgraded)
            canvasManager.ShowAltarInfo("Upgrade Wall Jump", upgradeCost);
        else
            canvasManager.HideAltarInfo();
    }

    public void HideUI() => canvasManager.HideAltarInfo();

    private void UpdateUI() => ShowUI();
}
