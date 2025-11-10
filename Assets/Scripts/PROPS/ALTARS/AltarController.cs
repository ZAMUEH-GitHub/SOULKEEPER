using UnityEngine;
using System.Collections;

public class AltarController : MonoBehaviour, IAltar, IInteractable
{
    [Header("Altar Setup")]
    public AltarStatsSO altarSO;
    public string altarID;
    private PlayerPowerUpController player;

    [Header("Altar State")]
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private bool completed = false;

    public bool IsUsed => completed;
    public bool IsCompleted => completed;
    public int CurrentStage => currentStageIndex;
    public int TotalStages => altarSO ? altarSO.StageCount : 0;

    private void Awake()
    {
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.GetComponent<PlayerPowerUpController>();
        }

        completed = (altarSO == null || altarSO.StageCount == 0);
        altarID = altarSO != null ? altarSO.displayName : "UnknownAltar";
    }

    public void Interact()
    {
        UnlockPowerUp();
        Debug.Log("[AltarController] Player Interacted with Altar");
    }

    public void UnlockPowerUp()
    {
        if (completed || altarSO == null) return;

        if (player == null || player.playerStats == null)
        {
            Debug.LogWarning("[AltarController] Player or stats not ready — retrying...");
            StartCoroutine(WaitForStatsAndApply());
            return;
        }

        ApplyCurrentStage();
    }

    private IEnumerator WaitForStatsAndApply()
    {
        while (player == null || player.playerStats == null)
            yield return null;

        Debug.Log("[AltarController] Player stats now ready — applying postponed PowerUp.");
        ApplyCurrentStage();
    }

    private void ApplyCurrentStage()
    {
        var def = altarSO.GetStage(currentStageIndex);
        if (def != null)
        {
            player.ApplyPowerUp(def);
        }

        currentStageIndex++;
        if (currentStageIndex >= altarSO.StageCount)
            CompleteAltar();
    }

    private void CompleteAltar()
    {
        Debug.Log("[AltarController] Altar Completed!!");
        completed = true;

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
    }

    public AltarSaveData ToSaveData() => new AltarSaveData(altarID, completed, currentStageIndex);

    public void FromSaveData(AltarSaveData data)
    {
        if (data == null || altarID != data.altarID) return;

        completed = data.completed;
        currentStageIndex = data.currentStage;

        if (completed)
        {
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }
    }
}
