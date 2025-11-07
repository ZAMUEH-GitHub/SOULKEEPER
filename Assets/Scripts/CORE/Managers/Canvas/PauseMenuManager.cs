using System.Collections;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private GameSceneManager sceneManager;
    [SerializeField] private TimeManager timeManager;

    [Header("Panels")]
    [SerializeField] private PanelType startPanel = PanelType.HUD;
    [SerializeField] private PanelType pausePanel = PanelType.PauseMenu;
    [SerializeField] private PanelType pauseSettings = PanelType.PauseSettings;
    [SerializeField] private PanelType pauseKeybindings = PanelType.PauseKeybindings;
    [SerializeField] private PanelType fadePanel = PanelType.BlackScreen;

    [field: SerializeField] private PanelType currentPanel;

    [Header("Scene Settings")]
    [SerializeField] private SceneField mainMenuScene;

    [Header("Input Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;

    private void Start()
    {
        if (canvasManager == null)
            canvasManager = FindFirstObjectByType<CanvasManager>();

        if (sceneManager == null)
            sceneManager = FindFirstObjectByType<GameSceneManager>();

        if (timeManager == null)
            timeManager = FindFirstObjectByType<TimeManager>();

        currentPanel = startPanel;

        canvasManager.FadeIn(currentPanel);
        canvasManager.FadeOut(pausePanel);
        canvasManager.FadeOut(pauseSettings);
        canvasManager.FadeOut(pauseKeybindings);
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

        timeManager.FreezeTime();
        GoToPanel(pausePanel);
    }

    private void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;

        timeManager.ResetTime();
        StartCoroutine(CrossFadePanels(currentPanel, startPanel));
        currentPanel = startPanel;
    }
    #endregion

    #region Inspector-Friendly UI Methods

    public void OnResumeGame() => ResumeGame();
    public void GoToPauseMenu() => GoToPanel(pausePanel);
    public void GoToSettingsPanel() => GoToPanel(pauseSettings);
    public void GoToKeybindingsPanel() => GoToPanel(pauseKeybindings);
    public void OnExitToMainMenu() => StartCoroutine(ExitToMainMenuRoutine());
    #endregion

    #region Navigation
    public void GoToPanel(PanelType newPanel)
    {
        if (newPanel == currentPanel)
            return;

        StartCoroutine(CrossFadePanels(currentPanel, newPanel));
        currentPanel = newPanel;
    }

    private IEnumerator CrossFadePanels(PanelType fromPanel, PanelType toPanel)
    {
        if (canvasManager == null)
            yield break;

        canvasManager.FadeOut(fromPanel);
        canvasManager.FadeIn(toPanel);

        yield return new WaitForSeconds(canvasManager.GetFadeDuration(toPanel));
    }
    #endregion

    #region Exit Logic
    private IEnumerator ExitToMainMenuRoutine()
    {
        timeManager.ResetTime();

        if (canvasManager != null)
            canvasManager.FadeOut(pausePanel);

        if (canvasManager != null)
            canvasManager.FadeIn(fadePanel);

        yield return new WaitForSeconds(canvasManager.GetFadeDuration(fadePanel));

        if (sceneManager != null)
            sceneManager.LoadScene(mainMenuScene, null);
    }
    #endregion

    #region Reset
    public void ResetToGameplay()
    {
        currentPanel = startPanel;
        canvasManager.FadeIn(currentPanel);
        isPaused = false;
        timeManager.ResetTime();
    }
    #endregion
}
