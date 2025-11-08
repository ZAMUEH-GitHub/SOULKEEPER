using UnityEngine;

public class SaveSlotManager : MonoBehaviour
{
    [Header("Active Save Slot Info")]
    [SerializeField] private int activeSlotIndex = 1;
    public int ActiveSlotIndex => activeSlotIndex;

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
}
