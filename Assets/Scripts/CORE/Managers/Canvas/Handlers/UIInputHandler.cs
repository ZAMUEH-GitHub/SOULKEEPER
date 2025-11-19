using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIInputHandler : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference uiBackAction;
    [SerializeField] private InputActionReference uiPauseAction;
    [SerializeField] private InputActionReference uiNavigateAction;

    [Header("Switching Settings")]
    [Tooltip("How long (in seconds) after the mouse stops moving should keyboard control reactivate?")]
    [SerializeField, Range(0f, 1f)] private float mouseIdleThreshold = 1.0f;

    private CanvasManager canvasManager;
    private PauseMenuManager pauseMenuManager;
    private MainMenuManager mainMenuManager;
    private ConfirmationPanelManager confirmationPanelManager;

    private Dictionary<PanelType, System.Action> mainMenuBackMap;
    private Dictionary<PanelType, System.Action> pauseMenuBackMap;

    private Vector2 lastMousePos;
    private bool mouseActive;
    private float mouseIdleTimer;
    private GameObject lastKeyboardSelection;

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

        if (uiNavigateAction != null)
            uiNavigateAction.action.performed += OnNavigatePressed;
    }

    private void OnDisable()
    {
        if (uiBackAction != null)
            uiBackAction.action.performed -= OnBackPressed;

        if (uiPauseAction != null)
            uiPauseAction.action.performed -= OnPausePressed;

        if (uiNavigateAction != null)
            uiNavigateAction.action.performed -= OnNavigatePressed;
    }

    private void Update()
    {
        HandleMouseKeyboardSwitching();
        CacheKeyboardSelection();
    }

    private void HandleMouseKeyboardSwitching()
    {
        if (Mouse.current == null || EventSystem.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        float distanceMoved = (mousePos - lastMousePos).sqrMagnitude;

        if (distanceMoved > 1f)
        {
            mouseActive = true;
            mouseIdleTimer = 0f;

            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
        else if (mouseActive)
        {
            mouseIdleTimer += Time.unscaledDeltaTime;
            if (mouseIdleTimer > mouseIdleThreshold)
                mouseActive = false; // Allows keyboard to regain control
        }

        lastMousePos = mousePos;
    }

    private void CacheKeyboardSelection()
    {
        if (EventSystem.current == null) return;

        var current = EventSystem.current.currentSelectedGameObject;
        if (current != null && !mouseActive)
            lastKeyboardSelection = current;
    }

    private void OnNavigatePressed(InputAction.CallbackContext ctx)
    {
        // When keyboard/gamepad navigation is detected, restore focus if mouse not active
        if (!mouseActive)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (lastKeyboardSelection != null && lastKeyboardSelection.activeInHierarchy)
                {
                    EventSystem.current.SetSelectedGameObject(lastKeyboardSelection);
                }
                else
                {
                    // Try to find a suitable selectable element to focus again
                    foreach (var panel in canvasManager.GetPanelSettings().Values)
                    {
                        if (panel.panel != null && panel.panel.alpha > 0.95f && panel.firstSelected != null)
                        {
                            EventSystem.current.SetSelectedGameObject(panel.firstSelected);
                            lastKeyboardSelection = panel.firstSelected;
                            break;
                        }
                    }
                }
            }
        }
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
