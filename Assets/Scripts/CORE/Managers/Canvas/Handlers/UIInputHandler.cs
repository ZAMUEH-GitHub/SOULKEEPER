using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIInputHandler : MonoBehaviour
{
    [Header("Input Action")]
    [SerializeField] private InputActionReference uiCancelAction;

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
        if (uiCancelAction != null)
            uiCancelAction.action.performed += OnCancelPressed;
    }

    private void OnDisable()
    {
        if (uiCancelAction != null)
            uiCancelAction.action.performed -= OnCancelPressed;
    }

    private void OnCancelPressed(InputAction.CallbackContext ctx)
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
                HandleGameplayCancel();
                break;

            case GameState.MainMenu:
                HandleMainMenuCancel();
                break;

            default:
                Debug.Log("[UIInputManager] No valid context for Cancel input.");
                break;
        }
    }

    #region Gameplay Handling
    private void HandleGameplayCancel()
    {
        if (IsPanelVisible(PanelType.HUD))
        {
            pauseMenuManager?.TogglePause();
            return;
        }

        foreach (var entry in pauseMenuBackMap)
        {
            if (IsPanelVisible(entry.Key))
            {
                entry.Value?.Invoke();
                return;
            }
        }
    }
    #endregion

    #region Main Menu Handling
    private void HandleMainMenuCancel()
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
    #endregion

    #region Navigation Maps
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
    #endregion

    private bool IsPanelVisible(PanelType type)
    {
        var panels = canvasManager?.GetPanelSettings();
        if (panels == null || !panels.ContainsKey(type)) return false;

        var cg = panels[type].panel;
        return cg != null && cg.alpha > 0.95f && cg.interactable;
    }
}
