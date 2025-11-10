using UnityEngine;
using System;

public class AltarController : MonoBehaviour, IInteractable
{
    [Header("Altar Setup")]
    public AltarStatsSO altarSO;
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private bool completed = false;

    public static event Action<PowerUpDefinition> OnPowerUpUnlocked;

    public bool IsUsed => completed;
    public bool IsCompleted => completed;
    public int CurrentStage => currentStageIndex;
    public int TotalStages => altarSO ? altarSO.StageCount : 0;

    private void Awake()
    {
        if (altarSO == null)
        {
            Debug.LogWarning($"[AltarController] Missing AltarStatsSO on {name}");
            completed = true;
        }
    }

    public void Interact()
    {
        if (completed || altarSO == null)
            return;

        Debug.Log($"[AltarController] Player interacted with {altarSO.displayName}");
        UnlockNextPowerUpStage();
    }

    private void UnlockNextPowerUpStage()
    {
        var def = altarSO.GetStage(currentStageIndex);
        if (def == null)
        {
            Debug.LogWarning($"[AltarController] Stage {currentStageIndex} missing definition.");
            return;
        }

        OnPowerUpUnlocked?.Invoke(def);

        Debug.Log($"[AltarController] Unlocked PowerUp: {def.name}");

        currentStageIndex++;

        if (currentStageIndex >= altarSO.StageCount)
            CompleteAltar();
    }

    private void CompleteAltar()
    {
        completed = true;
        Debug.Log($"[AltarController] {altarSO.displayName} completed!");

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
    }

    #region Save System
    public AltarSaveData ToSaveData() => new AltarSaveData(
        altarSO ? altarSO.displayName : "UnknownAltar",
        completed,
        currentStageIndex
    );

    public void FromSaveData(AltarSaveData data)
    {
        if (data == null || altarSO == null) return;

        completed = data.completed;
        currentStageIndex = data.currentStage;

        if (completed)
        {
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }
    }
    #endregion
}
