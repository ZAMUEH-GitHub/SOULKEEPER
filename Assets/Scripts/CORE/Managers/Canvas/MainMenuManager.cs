using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private GameSceneManager gameSceneManager;

    [Header("Panels")]
    [SerializeField] private PanelType startPanel = PanelType.MainMenu;
    [SerializeField] private PanelType fadePanel = PanelType.BlackScreen;

    [field: SerializeField] private PanelType currentPanel;

    private SceneField currentGameplayScene;
    [SerializeField] private SceneField newGameScene;

    private string startingDoorID;

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
    public void GoToLoadGamePanel() => GoToPanel(PanelType.LoadGame);
    public void GoToSettingsPanel() => GoToPanel(PanelType.Settings);
    public void GoToVideoSettings() => GoToPanel(PanelType.VideoSettings);
    public void GoToAudioSettings() => GoToPanel(PanelType.AudioSettings);
    public void GoToCreditsPanel() => GoToPanel(PanelType.Credits);
    public void GoToKeybindingsPanel() => GoToPanel(PanelType.KeyBindings);
    public void ContinueGame() => StartCoroutine(StartGameRoutine(currentGameplayScene, startingDoorID));
    public void NewGame() => StartCoroutine(StartGameRoutine(newGameScene, "Cathedral_StartDoor"));
    public void GoToPanel(PanelType newPanel)
    {
        if (newPanel == currentPanel)
            return;

        StartCoroutine(CrossFadePanels(currentPanel, newPanel));
        currentPanel = newPanel;
    }

    private IEnumerator CrossFadePanels(PanelType fromPanel, PanelType toPanel)
    {
        canvasManager.FadeOut(fromPanel);
        canvasManager.FadeIn(toPanel);
        yield return new WaitForSeconds(canvasManager.GetFadeDuration(toPanel));
    }

    public void StartGame(SceneField gameplayScene, string startingDoorID)
    {
        StartCoroutine(StartGameRoutine(gameplayScene, startingDoorID));
    }

    private IEnumerator StartGameRoutine(SceneField gameplayScene, string startingDoorID)
    {
        canvasManager.FadeIn(fadePanel);
        yield return new WaitForSeconds(canvasManager.GetFadeDuration(fadePanel));
        gameSceneManager.LoadScene(gameplayScene, startingDoorID);
    }

    public void ResetToMainMenu()
    {
        currentPanel = startPanel;
        canvasManager.FadeIn(currentPanel);
    }

    public void OnExitGame()
    {
        canvasManager.ShowConfirmation(
            "EXIT GAME?",
            "(The application will close.)",
            () => ExitGameExecutor(),
            () => Debug.Log("Exit cancelled.")
        );
    }

    public void ExitGameExecutor()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit(); // Quit the built game
        #endif
    }
}
