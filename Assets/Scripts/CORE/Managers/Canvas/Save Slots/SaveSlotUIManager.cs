using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class SaveSlotUIManager : MonoBehaviour
{
    [Header("Slot Info")]
    [SerializeField] private int slotIndex;

    [Header("UI References")]
    [SerializeField] private TMP_Text slotLabel;
    [SerializeField] private TMP_Text sceneLabel;
    [SerializeField] private TMP_Text timestampLabel;
    [SerializeField] private TMP_Text playtimeLabel;
    [SerializeField] private Button actionButton;
    [SerializeField] private Button deleteButton;

    private MainMenuManager menuManager;
    private CanvasManager canvasManager;
    private bool hasSave;

    private void Start()
    {
        menuManager = FindFirstObjectByType<MainMenuManager>();
        canvasManager = FindFirstObjectByType<CanvasManager>();
        RefreshSlotUI();
    }

    public void RefreshSlotUI()
    {
        var meta = SaveSystem.GetSlotMetadata(slotIndex);
        hasSave = meta.exists;

        slotLabel.text = $"Slot {slotIndex}";

        if (hasSave)
        {
            sceneLabel.text = string.IsNullOrEmpty(meta.scene) ? "Unknown Scene" : meta.scene;
            timestampLabel.text = meta.timestamp;
            playtimeLabel.text = FormatPlaytime(meta.playtime);
            actionButton.GetComponentInChildren<TMP_Text>().text = "Continue";

            if (deleteButton != null)
                deleteButton.gameObject.SetActive(true);
        }
        else
        {
            sceneLabel.text = "- Empty Slot -";
            timestampLabel.text = "";
            playtimeLabel.text = "";
            actionButton.GetComponentInChildren<TMP_Text>().text = "New Game";

            if (deleteButton != null)
                deleteButton.gameObject.SetActive(false);
        }
    }

    private string FormatPlaytime(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600f);
        int minutes = Mathf.FloorToInt((seconds % 3600f) / 60f);
        return $"{hours:D2}h {minutes:D2}m";
    }

    public void OnSlotButtonPressed()
    {
        if (menuManager == null)
        {
            Debug.LogError("[SaveSlotUI] MainMenuManager not found!");
            return;
        }

        if (hasSave)
            menuManager.OnLoadGameButton(slotIndex);
        else
            menuManager.NewGame(slotIndex);
    }

    public void OnDeleteSave()
    {
        if (!hasSave)
            return;

        if (menuManager == null)
        {
            Debug.LogError("[SaveSlotUI] MainMenuManager not found for delete confirmation!");
            return;
        }

        var canvas = FindFirstObjectByType<CanvasManager>();
        if (canvas != null)
        {
            canvas.ShowConfirmation(
                $"DELETE SAVE SLOT {slotIndex}?",
                "(This cannot be undone)",
                () => ExecuteDelete()
            );
        }
        else
        {
            ExecuteDelete();
        }
    }

    private void ExecuteDelete()
    {
        string path = Path.Combine(Application.persistentDataPath, $"Saves/SaveSlot_{slotIndex}.json");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveSlotUI] Deleted save slot {slotIndex}");
            canvasManager.ShowToast("Save Slot Deleted", 3f);
        }

        RefreshSlotUI();
    }
}
