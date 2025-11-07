using UnityEngine;

public class AltarController : MonoBehaviour, IAltar, IInteractable
{
    [Header("Setup")]
    public AltarStatsSO altar;
    private PlayerPowerUpController player;

    [Header("State (read-only)")]
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private bool completed = false;

    public bool IsUsed => completed;
    public bool IsCompleted => completed;
    public int CurrentStage => currentStageIndex;
    public int TotalStages => altar ? altar.StageCount : 0;

    private void Awake()
    {
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.GetComponent<PlayerPowerUpController>();
        }
        completed = (altar == null || altar.StageCount == 0);
    }

    public void Interact()
    {
        UnlockPowerUp();
        Debug.Log("Player Interacted with Altar");
    }

    public void UnlockPowerUp()
    {
        if (completed || altar == null) return;

        var def = altar.GetStage(currentStageIndex);
        if (def != null)
        {
            player.ApplyPowerUp(def);
            Debug.Log("Apply Power Up!!");
        }

        currentStageIndex++;

        if (currentStageIndex >= altar.StageCount)
            CompleteAltar();
    }

    private void CompleteAltar()
    {
        Debug.Log("Altar Completed!!");
        completed = true;
        
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
    }
}
