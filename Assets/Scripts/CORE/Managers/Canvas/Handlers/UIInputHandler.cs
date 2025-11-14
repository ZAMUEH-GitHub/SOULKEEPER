using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIInputHandler : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference uiBackAction;
    [SerializeField] private InputActionReference uiPauseAction;

    private CanvasManager canvasManager;
    private PauseMenuManager pauseMenuManager;
    private MainMenuManager mainMenuManager;
    private ConfirmationPanelManager confirmationPanelManager;

    private Dictionary<PanelType, System.Action> mainMenuBackMap;
    private Dictionary<PanelType, System.Action> pauseMenuBackMap;

    private void Awake()
    {
        canvasManager = CanvasManager.Instance;
        pauseMenuManager = PauseMenuManager.Instance;
        mainMenuManager = MainMenuManager.Instance;
        confirmationPanelManager = ConfirmationPanelManager.Instance;

        BuildNavigationMaps();
    }

    private void OnEnable()
    {
        if (uiBackAction != null)
            uiBackAction.action.performed += OnBackPressed;

        if (uiPauseAction != null)
            uiPauseAction.action.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        if (uiBackAction != null)
            uiBackAction.action.performed -= OnBackPressed;

        if (uiPauseAction != null)
            uiPauseAction.action.performed -= OnPausePressed;
    }


    private void OnBackPressed(InputAction.CallbackContext ctx)
    {
        if (confirmationPanelManager != null && IsPanelVisible(PanelType.ConfirmationPanel))
        {
            confirmationPanelManager.OnCancelPressed();
            return;
        }

        var gameState = GameManager.Instance?.CurrentState;

        switch (gameState)
        {
            case GameState.Gameplay:
                HandleGameplayBack();
                break;

            case GameState.MainMenu:
                HandleMainMenuBack();
                break;

            default:
                Debug.Log("[UIInputHandler] No valid context for Back input.");
                break;
        }
    }

    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance?.CurrentState != GameState.Gameplay)
            return;

        bool isOnHUD = IsPanelVisible(PanelType.HUD);
        bool isOnPauseMenu = IsPanelVisible(PanelType.PauseMenu);

        if (isOnHUD || isOnPauseMenu)
            pauseMenuManager?.TogglePause();
    }

    private void HandleGameplayBack()
    {
        if (IsPanelVisible(PanelType.HUD))
            return;

        foreach (var entry in pauseMenuBackMap)
        {
            if (IsPanelVisible(entry.Key))
            {
                entry.Value?.Invoke();
                return;
            }
        }
    }

    private void HandleMainMenuBack()
    {
        if (IsPanelVisible(PanelType.MainMenu))
        {
            mainMenuManager?.OnExitGame();
            return;
        }

        foreach (var entry in mainMenuBackMap)
        {
            if (IsPanelVisible(entry.Key))
            {
                entry.Value?.Invoke();
                return;
            }
        }
    }

    private void BuildNavigationMaps()
    {
        mainMenuBackMap = new Dictionary<PanelType, System.Action>
        {
            { PanelType.PlayGame, () => mainMenuManager?.GoToMainMenuPanel() },
            { PanelType.Settings, () => mainMenuManager?.GoToMainMenuPanel() },
            { PanelType.VideoSettings, () => mainMenuManager?.GoToSettingsPanel() },
            { PanelType.AudioSettings, () => mainMenuManager?.GoToSettingsPanel() },
            { PanelType.KeyBindings, () => mainMenuManager?.GoToSettingsPanel() },
            { PanelType.Credits, () => mainMenuManager?.GoToMainMenuPanel() },
        };

        pauseMenuBackMap = new Dictionary<PanelType, System.Action>
        {
            { PanelType.PauseMenu, () => pauseMenuManager?.OnResumeGame() },
            { PanelType.PauseSettings, () => pauseMenuManager?.GoToPauseMenu() },
            { PanelType.PauseAudioSettings, () => pauseMenuManager?.GoToSettingsPanel() },
            { PanelType.PauseKeybindings, () => pauseMenuManager?.GoToSettingsPanel() },
        };
    }

    private bool IsPanelVisible(PanelType type)
    {
        var panels = canvasManager?.GetPanelSettings();
        if (panels == null || !panels.ContainsKey(type)) return false;

        var cg = panels[type].panel;
        return cg != null && cg.alpha > 0.95f && cg.interactable;
    }
}
