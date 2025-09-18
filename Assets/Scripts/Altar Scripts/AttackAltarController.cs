using UnityEngine;
using UnityEngine.InputSystem;

public class AttackAltarController : MonoBehaviour, IAltar
{
    [SerializeField] private GameObject attackSpeedCollectible;
    [SerializeField] private GameObject attackDamageCollectible;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int speedCost = 100;
    [SerializeField] private int damageCost = 200;

    private bool isPlayerInRange = false;
    private bool speedUnlocked = false;
    private bool damageUnlocked = false;
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

        if (!speedUnlocked && score >= speedCost)
        {
            GameManager.Instance.SpendScore(speedCost);
            Instantiate(attackSpeedCollectible, spawnPoint.position, Quaternion.identity);
            speedUnlocked = true;
        }
        else if (speedUnlocked && !damageUnlocked && score >= damageCost)
        {
            GameManager.Instance.SpendScore(damageCost);
            Instantiate(attackDamageCollectible, spawnPoint.position, Quaternion.identity);
            damageUnlocked = true;
        }

        UpdateUI();
    }

    public void ShowUI()
    {
        if (!speedUnlocked)
            canvasManager.ShowAltarInfo("Unlock Attack Speed", speedCost);
        else if (!damageUnlocked)
            canvasManager.ShowAltarInfo("Upgrade Attack Damage", damageCost);
        else
            canvasManager.HideAltarInfo();
    }

    public void HideUI() => canvasManager.HideAltarInfo();

    private void UpdateUI() => ShowUI();
}
