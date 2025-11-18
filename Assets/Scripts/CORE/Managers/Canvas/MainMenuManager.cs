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

    [Header("New Game Defaults")]
    [SerializeField] private SceneField newGameScene;
    [SerializeField] private Vector2 defaultSpawnPosition;

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

        if (canvasManager == null)
        {
            Debug.LogWarning("[MainMenuManager] CanvasManager.Instance not found!");
            return;
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
            if (Input.anyKeyDown)
            {
                pressed = true;
                break;
            }
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

        if (canvasManager != null)
        {
            canvasManager.FadeOut(PanelType.PlayGame);
            canvasManager.FadeOut(PanelType.Settings);
            canvasManager.FadeOut(PanelType.VideoSettings);
            canvasManager.FadeOut(PanelType.AudioSettings);
            canvasManager.FadeOut(PanelType.KeyBindings);
            canvasManager.FadeOut(PanelType.Credits);

            canvasManager.FadeIn(PanelType.MainMenu);
            currentPanel = PanelType.MainMenu;
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

    #region New Game
    public void NewGame(int slotIndex)
    {
        saveSlotManager ??= SaveSlotManager.Instance;
        if (saveSlotManager == null)
        {
            Debug.LogError("[MainMenuManager] SaveSlotManager not found!");
            return;
        }

        saveSlotManager.SetActiveSlot(slotIndex);

        if (SaveSystem.SaveExists(slotIndex))
        {
            Debug.Log($"[MainMenuManager] Overwriting previous save slot {slotIndex}");
            string path = Path.Combine(Application.persistentDataPath, $"Saves/SaveSlot_{slotIndex}.json");
            if (File.Exists(path)) File.Delete(path);
        }

        StartCoroutine(StartNewGameRoutine());
    }

    private IEnumerator StartNewGameRoutine()
    {
        if (canvasManager != null)
        {
            canvasManager.FadeIn(fadePanel);
            yield return new WaitForSeconds(canvasManager.GetFadeDuration(fadePanel));
        }

        if (gameSceneManager != null)
        {
            gameSceneManager.LoadSceneDirect(newGameScene, defaultSpawnPosition);
        }
        else
        {
            Debug.LogError("[MainMenuManager] GameSceneManager.Instance not found!");
        }
    }
    #endregion

    #region Load Game
    public void OnLoadGameButton(int slotIndex)
    {
        if (!SaveSystem.SaveExists(slotIndex))
        {
            Debug.LogWarning($"[MainMenuManager] No save found in slot {slotIndex}");
            return;
        }

        saveSlotManager ??= SaveSlotManager.Instance;
        saveSlotManager.SetActiveSlot(slotIndex);

        if (canvasManager != null)
        {
            canvasManager.FadeIn(fadePanel);
            StartCoroutine(LoadSavedGameRoutine(slotIndex));
        }
        else
        {
            StartCoroutine(LoadSavedGameRoutine(slotIndex));
        }
    }

    private IEnumerator LoadSavedGameRoutine(int slotIndex)
    {
        yield return new WaitForSeconds(canvasManager?.GetFadeDuration(fadePanel) ?? 0.5f);

        if (gameSceneManager != null)
        {
            gameSceneManager.LoadSceneFromCheckpointSlot(slotIndex);
        }
        else
        {
            Debug.LogError("[MainMenuManager] GameSceneManager.Instance not found!");
        }
    }
    #endregion

    #region Exit Game
    public void OnExitGame()
    {
        if (canvasManager == null)
        {
            ExitGameExecutor();
            return;
        }

        canvasManager.ShowConfirmation(
            "EXIT GAME?",
            "(The application will close.)",
            ExitGameExecutor
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
    #endregion
}
