using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenuManager : Singleton<PauseMenuManager>
{
    protected override bool IsPersistent => false;

    [Header("UI References")]
    [SerializeField] private MenuBarManager localMenuBar;

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

    [Header("Keybindings Toggle")]
    [SerializeField] private GameObject keyboardControlsImage;
    [SerializeField] private GameObject gamepadControlsImage;

    private bool isPaused;
    private bool isTransitioning;
    private CanvasManager canvasManager;
    private GameSceneManager sceneManager;
    private TimeManager timeManager;

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
        canvasManager ??= CanvasManager.Instance;
        sceneManager ??= GameSceneManager.Instance;
        timeManager ??= TimeManager.Instance;
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

        if (localMenuBar != null) localMenuBar.SetVisible(false);
    }
    #endregion

    #region Pause Logic
    public void PlayerPauseInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) TogglePause();
    }

    public void TogglePause()
    {
        if (isTransitioning) return;

        if (!isPaused && sceneManager != null && sceneManager.IsLoadingScene)
            return;

        if (isPaused) StartCoroutine(ResumeGameRoutine());
        else StartCoroutine(PauseGameRoutine());
    }

    private IEnumerator PauseGameRoutine()
    {
        isTransitioning = true;
        isPaused = true;
        timeManager?.FreezeTime();

        canvasManager.FadeOut(startPanel);

        float fadeInTime = canvasManager.GetFadeDuration(pausePanel);
        float barOpenTime = 0f;

        if (localMenuBar != null)
        {
            localMenuBar.SetVisible(true);
            localMenuBar.OpenBar();
            barOpenTime = localMenuBar.OpenDuration;
        }

        float delay = Mathf.Max(0f, barOpenTime - fadeInTime);
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);

        canvasManager.FadeIn(pausePanel);
        currentPanel = pausePanel;

        yield return new WaitForSecondsRealtime(fadeInTime);

        isTransitioning = false;
    }

    private IEnumerator ResumeGameRoutine()
    {
        isTransitioning = true;

        canvasManager.FadeOut(currentPanel);

        float fadeOutTime = canvasManager.GetFadeDuration(currentPanel);
        float barCloseTime = 0f;

        if (localMenuBar != null)
        {
            localMenuBar.CloseBar();
            barCloseTime = localMenuBar.CloseDuration;
        }

        yield return new WaitForSecondsRealtime(Mathf.Max(fadeOutTime, barCloseTime));

        if (localMenuBar != null) localMenuBar.SetVisible(false);

        canvasManager.FadeIn(startPanel);
        currentPanel = startPanel;

        timeManager?.ResetTime();
        isPaused = false;
        isTransitioning = false;
    }
    #endregion

    #region Navigation
    public void OnResumeGame()
    {
        if (!isTransitioning) StartCoroutine(ResumeGameRoutine());
    }
    public void GoToPauseMenu() => GoToPanel(pausePanel);
    public void GoToSettingsPanel() => GoToPanel(pauseSettings);
    public void GoToAudioSettings() => GoToPanel(pauseAudioSettings);
    public void GoToKeybindingsPanel() => GoToPanel(pauseKeybindings);

    public void GoToPanel(PanelType newPanel)
    {
        if (isTransitioning || newPanel == currentPanel) return;
        StartCoroutine(CrossFadePanels(currentPanel, newPanel));
        currentPanel = newPanel;
    }

    private IEnumerator CrossFadePanels(PanelType fromPanel, PanelType toPanel)
    {
        if (canvasManager == null) yield break;

        isTransitioning = true;

        canvasManager.FadeOut(fromPanel);

        float fadeOutTime = canvasManager.GetFadeDuration(fromPanel);
        float barCloseTime = 0f;

        if (localMenuBar != null)
        {
            localMenuBar.CloseBar();
            barCloseTime = localMenuBar.CloseDuration;
        }

        yield return new WaitForSecondsRealtime(Mathf.Max(fadeOutTime, barCloseTime));


        float fadeInTime = canvasManager.GetFadeDuration(toPanel);
        float barOpenTime = 0f;

        if (localMenuBar != null)
        {
            barOpenTime = localMenuBar.OpenDuration;
        }

        float delayBeforeFadeIn = Mathf.Max(0f, barOpenTime - fadeInTime);

        if (barOpenTime > 0f)
        {
            localMenuBar.OpenBar();
        }

        if (delayBeforeFadeIn > 0f)
        {
            yield return new WaitForSecondsRealtime(delayBeforeFadeIn);
        }

        canvasManager.FadeIn(toPanel);

        yield return new WaitForSecondsRealtime(fadeInTime);

        isTransitioning = false;
    }
    #endregion

    #region Exit Logic
    public void OnExitToMainMenu()
    {
        if (canvasManager == null) return;

        canvasManager.ShowConfirmation(
            "EXIT TO MAIN MENU?",
            "(All unsaved progress will be lost)",
            ExecuteExitToMainMenu
        );
    }

    private void ExecuteExitToMainMenu()
    {
        StartCoroutine(ExitToMainMenuRoutine());
    }

    private IEnumerator ExitToMainMenuRoutine()
    {
        if (canvasManager != null) canvasManager.FadeOut(currentPanel);
        float fadeOutTime = canvasManager != null ? canvasManager.GetFadeDuration(currentPanel) : 0f;
        float barCloseTime = 0f;

        if (localMenuBar != null)
        {
            localMenuBar.CloseBar();
            barCloseTime = localMenuBar.CloseDuration;
        }

        yield return new WaitForSecondsRealtime(Mathf.Max(fadeOutTime, barCloseTime));

        if (localMenuBar != null) localMenuBar.SetVisible(false);

        timeManager?.ResetTime();
        sceneManager?.LoadSceneDirect(mainMenuScene, Vector2.zero);
    }
    #endregion

    #region Keybindings UI
    public void ToggleControlImages()
    {
        if (keyboardControlsImage != null && gamepadControlsImage != null)
        {
            bool isKeyboardActive = keyboardControlsImage.activeSelf;
            keyboardControlsImage.SetActive(!isKeyboardActive);
            gamepadControlsImage.SetActive(isKeyboardActive);
        }
    }
    #endregion

    public void ResetToGameplay()
    {
        currentPanel = startPanel;
        canvasManager?.FadeIn(currentPanel);
        if (localMenuBar != null) localMenuBar.SetVisible(false);
        isPaused = false;
        timeManager?.ResetTime();
    }
}