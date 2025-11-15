using UnityEngine;

public class SaveSlotPanelManager : MonoBehaviour
{
    [SerializeField] private SaveSlotUIManager[] slots;

    private void OnEnable()
    {
        slots = GetComponentsInChildren<SaveSlotUIManager>(true);
        RefreshAllSlots();
    }

    public void RefreshAllSlots()
    {
        foreach (var slot in slots)
            slot.RefreshSlotUI();
    }
}
