using System.Collections;
using System.IO;
using UnityEngine;

public class MainMenuManager : Singleton<MainMenuManager>
{
    protected override bool IsPersistent => false;

    [Header("Panels")]
    [SerializeField] private PanelType startPanel = PanelType.MainMenu;
    [SerializeField] private PanelType fadePanel = PanelType.BlackScreen;
    [field: SerializeField] private PanelType currentPanel;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDelay = 0.15f;
    [SerializeField] private MenuBarManager localMenuBar;

    [Header("New Game Defaults")]
    [SerializeField] private SceneField newGameScene;
    [SerializeField] private Vector2 defaultSpawnPosition;

    [Header("Keybindings Toggle")]
    [SerializeField] private GameObject keyboardControlsImage;
    [SerializeField] private GameObject gamepadControlsImage;

    private static bool hasInitializedOnce = false;
    private bool isTransitioning = false;

    private CanvasManager canvasManager;
    private GameSceneManager gameSceneManager;
    private SaveSlotManager saveSlotManager;

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
        canvasManager ??= CanvasManager.Instance;
        gameSceneManager ??= GameSceneManager.Instance;
        saveSlotManager ??= SaveSlotManager.Instance;
    }

    private void Start()
    {
        currentPanel = startPanel;

        if (canvasManager == null) return;

        if (localMenuBar != null)
        {
            localMenuBar.SetVisible(startPanel != PanelType.TitleScreen);
        }

        if (startPanel == PanelType.TitleScreen)
        {
            canvasManager.FadeIn(PanelType.TitleScreen);
            canvasManager.FadeOut(fadePanel);
            StartCoroutine(WaitForAnyKeyThenOpenMainMenu());
        }
        else
        {
            canvasManager.FadeIn(currentPanel);
            canvasManager.FadeOut(fadePanel);
        }
    }

    private IEnumerator WaitForAnyKeyThenOpenMainMenu()
    {
        yield return new WaitForSeconds(canvasManager.GetFadeDuration(PanelType.TitleScreen));

        bool pressed = false;
        while (!pressed)
        {
            if (Input.anyKeyDown) { pressed = true; break; }
            if (UnityEngine.InputSystem.Gamepad.current != null &&
               (UnityEngine.InputSystem.Gamepad.current.startButton.wasPressedThisFrame ||
                UnityEngine.InputSystem.Gamepad.current.buttonSouth.wasPressedThisFrame))
            {
                pressed = true;
                break;
            }
            yield return null;
        }

        canvasManager.FadeOut(PanelType.TitleScreen);
        yield return new WaitForSeconds(canvasManager.GetFadeDuration(PanelType.TitleScreen));
        GoToMainMenuPanel();
    }

    private void OnEnable()
    {
        canvasManager ??= CanvasManager.Instance;
        if (canvasManager == null) return;

        canvasManager.FadeOut(PanelType.MainMenu);
        canvasManager.FadeOut(PanelType.PlayGame);
        canvasManager.FadeOut(PanelType.Settings);
        canvasManager.FadeOut(PanelType.VideoSettings);
        canvasManager.FadeOut(PanelType.AudioSettings);
        canvasManager.FadeOut(PanelType.KeyBindings);
        canvasManager.FadeOut(PanelType.Credits);

        if (!hasInitializedOnce && startPanel == PanelType.TitleScreen)
        {
            canvasManager.FadeIn(PanelType.TitleScreen);
            currentPanel = PanelType.TitleScreen;
            hasInitializedOnce = true;

            if (localMenuBar != null) localMenuBar.SetVisible(false);
        }
        else
        {
            canvasManager.FadeIn(PanelType.MainMenu);
            currentPanel = PanelType.MainMenu;

            if (localMenuBar != null) localMenuBar.SetVisible(true);
        }
    }
    #endregion

    #region Panel Navigation
    public void GoToMainMenuPanel() => GoToPanel(PanelType.MainMenu);
    public void GoToPlayGamePanel() => GoToPanel(PanelType.PlayGame);
    public void GoToSettingsPanel() => GoToPanel(PanelType.Settings);
    public void GoToAudioSettingsPanel() => GoToPanel(PanelType.AudioSettings);
    public void GoToVideoSettingsPanel() => GoToPanel(PanelType.VideoSettings);
    public void GoToKeybindingsPanel() => GoToPanel(PanelType.KeyBindings);
    public void GoToCreditsPanel() => GoToPanel(PanelType.Credits);

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

        yield return new WaitForSecondsRealtime(transitionDelay);

        canvasManager.FadeOut(fromPanel);

        float fadeOutTime = canvasManager.GetFadeDuration(fromPanel);
        float barCloseTime = 0f;

        if (localMenuBar != null && fromPanel != PanelType.TitleScreen && fromPanel != PanelType.MainMenu)
        {
            localMenuBar.CloseBar();
            barCloseTime = localMenuBar.CloseDuration;
        }

        yield return new WaitForSecondsRealtime(Mathf.Max(fadeOutTime, barCloseTime));


        float fadeInTime = canvasManager.GetFadeDuration(toPanel);
        float barOpenTime = 0f;

        if (localMenuBar != null)
        {
            if (fromPanel == PanelType.TitleScreen)
            {
                localMenuBar.SetVisible(true);
            }

            if (toPanel != PanelType.MainMenu && toPanel != PanelType.TitleScreen)
            {
                barOpenTime = localMenuBar.OpenDuration;
            }
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

    #region Game Logic
    public void NewGame(int slotIndex)
    {
        saveSlotManager ??= SaveSlotManager.Instance;
        if (saveSlotManager == null) return;

        saveSlotManager.SetActiveSlot(slotIndex);

        if (SaveSystem.SaveExists(slotIndex))
        {
            string path = Path.Combine(Application.persistentDataPath, $"Saves/SaveSlot_{slotIndex}.json");
            if (File.Exists(path)) File.Delete(path);
        }

        if (gameSceneManager != null)
            gameSceneManager.LoadSceneDirect(newGameScene, defaultSpawnPosition);
    }

    public void OnLoadGameButton(int slotIndex)
    {
        if (!SaveSystem.SaveExists(slotIndex)) return;

        saveSlotManager ??= SaveSlotManager.Instance;
        saveSlotManager.SetActiveSlot(slotIndex);

        if (gameSceneManager != null)
            gameSceneManager.LoadSceneFromCheckpointSlot(slotIndex);
    }

    public void OnExitGame()
    {
        if (canvasManager == null) { ExitGameExecutor(); return; }

        canvasManager.ShowConfirmation("EXIT GAME?", "(The application will close.)", ExitGameExecutor);
    }

    private void ExitGameExecutor()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
}