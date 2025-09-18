using UnityEngine;
using UnityEngine.InputSystem;

public class JumpAltarController : MonoBehaviour, IAltar
{
    [SerializeField] private GameObject jumpUnlockCollectible;
    [SerializeField] private GameObject jumpUpgradeCollectible;
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
            Instantiate(jumpUnlockCollectible, spawnPoint.position, Quaternion.identity);
            isUnlocked = true;
        }
        else if (isUnlocked && !isUpgraded && score >= upgradeCost)
        {
            GameManager.Instance.SpendScore(upgradeCost);
            Instantiate(jumpUpgradeCollectible, spawnPoint.position, Quaternion.identity);
            isUpgraded = true;
        }

        UpdateUI();
    }

    public void ShowUI()
    {
        if (!isUnlocked)
            canvasManager.ShowAltarInfo("Unlock Double Jump", unlockCost);
        else if (!isUpgraded)
            canvasManager.ShowAltarInfo("Upgrade to Triple Jump", upgradeCost);
        else
            canvasManager.HideAltarInfo();
    }

    public void HideUI() => canvasManager.HideAltarInfo();

    private void UpdateUI() => ShowUI();
}
