using UnityEngine;

public class AltarController : MonoBehaviour, IAltar, IInteractable
{
    [Header("Setup")]
    public AltarStatsSO altarSO;
    public string altarID;
    private PlayerPowerUpController player;

    [Header("State (read-only)")]
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

        altarID = altarSO.displayName;
    }

    public void Interact()
    {
        UnlockPowerUp();
        Debug.Log("Player Interacted with Altar");
    }

    public void UnlockPowerUp()
    {
        if (completed || altarSO == null) return;

        var def = altarSO.GetStage(currentStageIndex);
        if (def != null)
        {
            player.ApplyPowerUp(def);
            Debug.Log("Apply Power Up!!");
        }

        currentStageIndex++;

        if (currentStageIndex >= altarSO.StageCount)
            CompleteAltar();
    }

    private void CompleteAltar()
    {
        Debug.Log("Altar Completed!!");
        completed = true;
        
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
    }

    public AltarSaveData ToSaveData()
    {
        return new AltarSaveData(altarID, completed, currentStageIndex);
    }

    public void FromSaveData(AltarSaveData data)
    {
        if (data == null) return;
        if (altarID != data.altarID) return;

        completed = data.completed;
        currentStageIndex = data.currentStage;

        if (completed)
        {
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }
    }
}
