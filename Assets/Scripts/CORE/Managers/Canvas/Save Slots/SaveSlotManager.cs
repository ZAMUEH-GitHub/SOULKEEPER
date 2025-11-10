using UnityEngine;

public class SaveSlotManager : Singleton<SaveSlotManager>
{
    protected override bool IsPersistent => false;

    [Header("Active Save Slot Info")]
    [SerializeField] private int activeSlotIndex = 1;
    public int ActiveSlotIndex => activeSlotIndex;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetActiveSlot(int slot)
    {
        if (slot < 1 || slot > 3)
        {
            Debug.LogError($"[SaveSlotManager] Invalid slot index: {slot}");
            return;
        }

        activeSlotIndex = slot;
        Debug.Log($"[SaveSlotManager] Active save slot set to {slot}");
    }

    public string GetActiveSaveFileName()
    {
        return $"SaveSlot_{activeSlotIndex}.json";
    }
}
