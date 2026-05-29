using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class SaveSlotUIManager : MonoBehaviour
{
    [Header("Slot Info")]
    [SerializeField] private int slotIndex;

    [Header("UI References")]
    [SerializeField] private TMP_Text sceneLabel;
    [SerializeField] private TMP_Text timestampLabel;
    [SerializeField] private TMP_Text playtimeLabel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
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

        if (hasSave)
        {
            sceneLabel.text = string.IsNullOrEmpty(meta.scene) ? "Unknown Scene" : meta.scene;
            timestampLabel.text = meta.timestamp;
            playtimeLabel.text = FormatPlaytime(meta.playtime);

            if (continueButton != null) continueButton.gameObject.SetActive(true);
            if (newGameButton != null) newGameButton.gameObject.SetActive(false);

            if (deleteButton != null)
                deleteButton.gameObject.SetActive(true);
        }
        else
        {
            sceneLabel.text = "- Empty Slot -";
            timestampLabel.text = "";
            playtimeLabel.text = "";

            if (continueButton != null) continueButton.gameObject.SetActive(false);
            if (newGameButton != null) newGameButton.gameObject.SetActive(true);

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

    public void OnContinuePressed()
    {
        if (menuManager == null)
        {
            Debug.LogError("[SaveSlotUI] MainMenuManager not found!");
            return;
        }

        menuManager.OnLoadGameButton(slotIndex);
    }

    public void OnNewGamePressed()
    {
        if (menuManager == null)
        {
            Debug.LogError("[SaveSlotUI] MainMenuManager not found!");
            return;
        }

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