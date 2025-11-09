using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private GameSceneManager gameSceneManager;
    [SerializeField] private SaveSlotManager saveSlotManager;

    [Header("Panels")]
    [SerializeField] private PanelType startPanel = PanelType.MainMenu;
    [SerializeField] private PanelType fadePanel = PanelType.BlackScreen;

    [field: SerializeField] private PanelType currentPanel;

    [Header("Scenes")]
    [SerializeField] private SceneField newGameScene;

    private void Start()
    {
        if (canvasManager == null)
            canvasManager = FindFirstObjectByType<CanvasManager>();

        if (gameSceneManager == null)
            gameSceneManager = FindFirstObjectByType<GameSceneManager>();

        currentPanel = startPanel;
        canvasManager.FadeIn(currentPanel);
    }

    public void GoToMainMenuPanel() => GoToPanel(PanelType.MainMenu);
    public void GoToPlayGamePanel() => GoToPanel(PanelType.PlayGame);
    public void GoToSettingsPanel() => GoToPanel(PanelType.Settings);
    public void GoToAudioSettingsPanel() => GoToPanel(PanelType.AudioSettings);
    public void GoToVideoSettingsPanel() => GoToPanel(PanelType.VideoSettings);
    public void GoToKeybindingsPanel() => GoToPanel(PanelType.KeyBindings);
    public void GoToCreditsPanel() => GoToPanel(PanelType.Credits);

    public void GoToPanel(PanelType newPanel)
    {
        if (newPanel == currentPanel) return;
        StartCoroutine(CrossFadePanels(currentPanel, newPanel));
        currentPanel = newPanel;
    }

    private IEnumerator CrossFadePanels(PanelType fromPanel, PanelType toPanel)
    {
        canvasManager.FadeOut(fromPanel);
        canvasManager.FadeIn(toPanel);
        yield return new WaitForSeconds(canvasManager.GetFadeDuration(toPanel));
    }

    public void NewGame(int slotIndex)
    {
        saveSlotManager.SetActiveSlot(slotIndex);

        if (SaveSystem.SaveExists(slotIndex))
        {
            Debug.Log($"[MainMenuManager] Overwriting previous save slot {slotIndex}");
            System.IO.File.Delete(Path.Combine(Application.persistentDataPath, $"Saves/SaveSlot_{slotIndex}.json"));
        }

        StartCoroutine(StartNewGameRoutine());
    }

    private IEnumerator StartNewGameRoutine()
    {
        canvasManager.FadeIn(fadePanel);
        yield return new WaitForSeconds(canvasManager.GetFadeDuration(fadePanel));

        gameSceneManager.LoadSceneFromDoor(newGameScene, "Cathedral_StartDoor");
    }

    public void OnLoadGameButton(int slotIndex)
    {
        if (!SaveSystem.SaveExists(slotIndex))
        {
            Debug.LogWarning($"[MainMenuManager] No save found in slot {slotIndex}");
            return;
        }

        saveSlotManager.SetActiveSlot(slotIndex);

        string savedScene = SaveSystem.GetSavedScene(slotIndex);
        string savedDoor = SaveSystem.GetSavedDoor(slotIndex);

        if (string.IsNullOrEmpty(savedScene))
        {
            Debug.LogError("[MainMenuManager] Save data missing scene info!");
            return;
        }

        StartCoroutine(LoadSavedGameRoutine(savedScene, savedDoor));
    }

    private IEnumerator LoadSavedGameRoutine(SceneField savedScene, string savedDoor)
    {
        canvasManager.FadeIn(fadePanel);
        yield return new WaitForSeconds(canvasManager.GetFadeDuration(fadePanel));

        gameSceneManager.LoadSceneFromCheckpoint(savedScene);
    }

    public void OnExitGame()
    {
        canvasManager.ShowConfirmation(
            "EXIT GAME?",
            "(The application will close.)",
            ExitGameExecutor,
            () => Debug.Log("[MainMenuManager] Exit cancelled.")
        );
    }

    private void ExitGameExecutor()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
