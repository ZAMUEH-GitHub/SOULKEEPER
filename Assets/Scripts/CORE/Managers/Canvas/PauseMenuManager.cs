using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuManager : Singleton<PauseMenuManager>
{
    [Header("Managers")]
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private GameSceneManager sceneManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private SaveSlotManager saveSlotManager;

    [Header("Panels")]
    [SerializeField] private PanelType startPanel = PanelType.HUD;
    [SerializeField] private PanelType pausePanel = PanelType.PauseMenu;
    [SerializeField] private PanelType pauseSettings = PanelType.PauseSettings;
    [SerializeField] private PanelType pauseAudioSettings = PanelType.PauseAudioSettings;
    [SerializeField] private PanelType pauseKeybindings = PanelType.PauseKeybindings;
    [SerializeField] private PanelType fadePanel = PanelType.BlackScreen;
    [field: SerializeField] private PanelType currentPanel;

    [Header("Scene Settings")]
    [SerializeField] private SceneField mainMenuScene;

    [Header("Input Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused;
    private GameObject gameplayCanvas;

    protected override void Awake()
    {
        base.Awake();

        canvasManager ??= CanvasManager.Instance;
        sceneManager ??= GameSceneManager.Instance;
        gameManager ??= GameManager.Instance;
        timeManager ??= TimeManager.Instance;
        saveSlotManager ??= SaveSlotManager.Instance;
    }

    private void Start()
    {
        currentPanel = startPanel;

        if (canvasManager != null)
        {
            canvasManager.FadeIn(currentPanel);
            canvasManager.FadeOut(pausePanel);
            canvasManager.FadeOut(pauseSettings);
            canvasManager.FadeOut(pauseAudioSettings);
            canvasManager.FadeOut(pauseKeybindings);
        }

        gameplayCanvas = gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
            TogglePause();
    }

    #region Pause Logic
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;

        if (canvasManager != null)
            canvasManager.ToggleCanvasInteractivity(gameplayCanvas, true);

        var raycaster = gameplayCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster != null && !raycaster.enabled)
            raycaster.enabled = true;

        timeManager?.FreezeTime();
        GoToPanel(pausePanel);
    }

    private void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;

        var raycaster = gameplayCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
            raycaster.enabled = false;

        foreach (var t in gameplayCanvas.GetComponentsInChildren<EventTrigger>(true))
            t.enabled = false;

        timeManager?.ResetTime();
        StartCoroutine(CrossFadePanels(currentPanel, startPanel));
        currentPanel = startPanel;
    }
    #endregion

    #region Inspector-Friendly UI Methods
    public void OnResumeGame() => ResumeGame();
    public void GoToPauseMenu() => GoToPanel(pausePanel);
    public void GoToSettingsPanel() => GoToPanel(pauseSettings);
    public void GoToAudioSettings() => GoToPanel(pauseAudioSettings);
    public void GoToKeybindingsPanel() => GoToPanel(pauseKeybindings);
    #endregion

    #region Navigation
    public void GoToPanel(PanelType newPanel)
    {
        if (newPanel == currentPanel) return;
        StartCoroutine(CrossFadePanels(currentPanel, newPanel));
        currentPanel = newPanel;
    }

    private IEnumerator CrossFadePanels(PanelType fromPanel, PanelType toPanel)
    {
        if (canvasManager == null) yield break;

        canvasManager.FadeOut(fromPanel);
        canvasManager.FadeIn(toPanel);

        yield return new WaitForSeconds(canvasManager.GetFadeDuration(toPanel));
    }
    #endregion

    #region Exit Logic
    public void OnExitToMainMenu()
    {
        if (canvasManager == null) return;

        canvasManager.ShowConfirmation(
            "EXIT TO MAIN MENU?",
            "(All unsaved progress will be lost)",
            () => StartCoroutine(ExitToMainMenuRoutine()),
            () => Debug.Log("[PauseMenuManager] Exit cancelled.")
        );
    }

    private IEnumerator ExitToMainMenuRoutine()
    {
        timeManager?.ResetTime();

        if (canvasManager != null)
        {
            canvasManager.FadeOut(pausePanel);
            canvasManager.FadeIn(fadePanel);
            yield return new WaitForSeconds(canvasManager.GetFadeDuration(fadePanel));
        }

        sceneManager?.LoadSceneDirect(mainMenuScene);
    }
    #endregion

    #region Save Logic
    public void OnSaveGame()
    {
        var stats = SessionManager.Instance.RuntimeStats;
        if (stats == null)
        {
            Debug.LogWarning("[PauseMenuManager] No runtime PlayerStats found — cannot save!");
            return;
        }

        int slot = saveSlotManager?.ActiveSlotIndex ?? 1;

        if (canvasManager != null)
        {
            canvasManager.ShowConfirmation(
                "SAVE GAME?",
                $"Do you want to save your progress to Slot {slot}?",
                () =>
                {
                    SaveSystem.Save(slot, stats);
                    Debug.Log($"[PauseMenuManager] Game saved to slot {slot}.");
                    canvasManager.ShowToast("Progress Saved", 3f);
                },
                () => Debug.Log("[PauseMenuManager] Save cancelled by player.")
            );
        }
        else
        {
            SaveSystem.Save(slot, stats);
            Debug.Log($"[PauseMenuManager] Game saved to slot {slot} (no canvas).");
        }
    }
    #endregion

    #region Reset
    public void ResetToGameplay()
    {
        currentPanel = startPanel;
        canvasManager?.FadeIn(currentPanel);
        isPaused = false;
        timeManager?.ResetTime();
    }
    #endregion
}
