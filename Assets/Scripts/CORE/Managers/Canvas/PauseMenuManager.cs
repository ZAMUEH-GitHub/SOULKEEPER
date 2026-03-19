using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenuManager : Singleton<PauseMenuManager>
{
    protected override bool IsPersistent => false;

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
    }
    #endregion

    #region Pause Logic
    public void PlayerPauseInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    private void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;

        timeManager?.FreezeTime();
        canvasManager.FadeOut(startPanel);
        canvasManager.FadeIn(pausePanel);
        currentPanel = pausePanel;
    }

    private void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;

        timeManager?.ResetTime();
        canvasManager.FadeOut(pausePanel);
        canvasManager.FadeIn(startPanel);
        currentPanel = startPanel;
    }
    #endregion

    #region Navigation
    public void OnResumeGame() => ResumeGame();
    public void GoToPauseMenu() => GoToPanel(pausePanel);
    public void GoToSettingsPanel() => GoToPanel(pauseSettings);
    public void GoToAudioSettings() => GoToPanel(pauseAudioSettings);
    public void GoToKeybindingsPanel() => GoToPanel(pauseKeybindings);

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
            ExitToMainMenu
        );
    }

    private void ExitToMainMenu()
    {
        timeManager?.ResetTime();
        if (canvasManager != null) canvasManager.FadeOut(pausePanel);
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
        isPaused = false;
        timeManager?.ResetTime();
    }
}