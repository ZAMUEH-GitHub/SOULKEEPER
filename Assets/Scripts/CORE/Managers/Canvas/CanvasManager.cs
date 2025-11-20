using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PanelType
{
    [Header("Main Menu Panels")]
    TitleScreen, MainMenu, PlayGame, Settings, VideoSettings, AudioSettings, KeyBindings, Credits,

    [Header("Gameplay Panels")]
    HUD, PauseMenu, PauseSettings, PauseAudioSettings, PauseKeybindings,

    [Header("Global Panels")]
    BlackScreen, ConfirmationPanel, ToastPanel, AreaTitlePanel
}

[Serializable]
public class PanelFadeSettings
{
    [Header("Panel Settings")]
    public PanelType panelType;
    public CanvasGroup panel;
    public GameObject firstSelected;

    [Header("Fade Properties")]
    public float fadeDuration = 0.3f;
    public bool interactable = true;
    public bool blockRaycasts = true;
    public bool disablesPlayerInput = false;

    [Header("Selection Behavior")]
    public bool rememberLastSelection = true;
}

public class CanvasManager : Singleton<CanvasManager>
{
    protected override bool IsPersistent => false;

    [Header("Canvas Groups")]
    [SerializeField] private GameObject _MainMenuCanvas;
    [SerializeField] private GameObject _GameplayCanvas;
    [SerializeField] private GameObject _GlobalCanvas;

    [Header("Panels Configuration")]
    [SerializeField] private List<PanelFadeSettings> panelSettingsList = new();
    private readonly Dictionary<PanelType, PanelFadeSettings> panelSettings = new();
    private readonly Dictionary<CanvasGroup, Coroutine> activeFades = new();

    private PlayerController playerController;
    private readonly Dictionary<PanelType, GameObject> _lastSelectedByPanel = new();
    private int inputFreezeCount = 0;

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();

        foreach (var settings in panelSettingsList)
        {
            if (settings.panel != null && !panelSettings.ContainsKey(settings.panelType))
                panelSettings.Add(settings.panelType, settings);
        }

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerController = playerObj.GetComponent<PlayerController>();

        if (_GlobalCanvas != null && panelSettings.ContainsKey(PanelType.BlackScreen))
            FadeIn(PanelType.BlackScreen);
    }

    private void OnEnable() => GameManager.OnGameStateChanged += HandleGameStateChanged;
    private void OnDisable() => GameManager.OnGameStateChanged -= HandleGameStateChanged;

    private void HandleGameStateChanged(GameState state)
    {
        bool isMainMenu = state == GameState.MainMenu;

        ToggleCanvasInteractivity(_MainMenuCanvas, isMainMenu);
        ToggleCanvasInteractivity(_GameplayCanvas, !isMainMenu);
        ToggleCanvasInteractivity(_GlobalCanvas, true);

        if (isMainMenu) FadeIn(PanelType.MainMenu);
        else FadeIn(PanelType.HUD);
    }
    #endregion

    #region Fade API
    public void FadeIn(PanelType type)
    {
        if (!panelSettings.TryGetValue(type, out var s) || s.panel == null)
            return;

        StartFade(s, s.panel.alpha, 1f, s.fadeDuration);
    }

    public void FadeOut(PanelType type)
    {
        if (!panelSettings.TryGetValue(type, out var s) || s.panel == null)
            return;

        StartFade(s, s.panel.alpha, 0f, s.fadeDuration);
    }

    public float GetFadeDuration(PanelType type)
        => panelSettings.TryGetValue(type, out var s) ? s.fadeDuration : 0.5f;
    #endregion

    #region Fade Logic
    private void StartFade(PanelFadeSettings s, float startAlpha, float finalAlpha, float duration)
    {
        var panel = s.panel;

        if (activeFades.TryGetValue(panel, out var oldFade))
        {
            StopCoroutine(oldFade);
            activeFades.Remove(panel);
        }

        if (s.rememberLastSelection && finalAlpha == 0f && EventSystem.current != null)
        {
            var current = EventSystem.current.currentSelectedGameObject;
            if (current != null && current.transform.IsChildOf(panel.transform))
            {
                _lastSelectedByPanel[s.panelType] = current;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        var fadeRoutine = StartCoroutine(FadeRoutine(s, startAlpha, finalAlpha, duration));
        activeFades[panel] = fadeRoutine;
    }

    private IEnumerator FadeRoutine(PanelFadeSettings s, float startAlpha, float finalAlpha, float duration)
    {
        var panel = s.panel;
        bool fadingIn = finalAlpha > startAlpha;
        bool fadingOut = finalAlpha < startAlpha;

        if (fadingIn && s.disablesPlayerInput)
            FreezePlayerInput();
        else if (fadingOut && s.disablesPlayerInput)
            UnfreezePlayerInput();

        if (fadingIn)
        {
            panel.interactable = s.interactable;
            panel.blocksRaycasts = s.blockRaycasts;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            panel.alpha = Mathf.Lerp(startAlpha, finalAlpha, t / duration);
            yield return null;
        }

        panel.alpha = finalAlpha;

        if (finalAlpha > 0f)
        {
            panel.interactable = s.interactable;
            panel.blocksRaycasts = s.blockRaycasts;
            yield return StartCoroutine(ApplyPanelSelectionNextFrame(s));
        }
        else
        {
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }

        activeFades.Remove(panel);
    }

    private IEnumerator ApplyPanelSelectionNextFrame(PanelFadeSettings s)
    {
        yield return null;
        if (EventSystem.current == null || s == null) yield break;

        GameObject candidate = null;

        if (s.rememberLastSelection &&
            _lastSelectedByPanel.TryGetValue(s.panelType, out var remembered) &&
            remembered != null)
        {
            candidate = remembered;
        }

        if (candidate == null) candidate = s.firstSelected;
        if (candidate == null || !candidate.activeInHierarchy) yield break;

        var selectable = candidate.GetComponent<Selectable>();
        if (selectable == null || !selectable.IsInteractable()) yield break;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(candidate);
        selectable.OnSelect(null);
    }
    #endregion

    #region Player Input Management
    private void EnsurePlayerController()
    {
        if (playerController != null) return;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
        }
    }

    private void FreezePlayerInput()
    {
        EnsurePlayerController();
        inputFreezeCount++;

        if (inputFreezeCount == 1 && playerController != null)
        {
            playerController.FreezeAllInputs();
        }
    }

    private void UnfreezePlayerInput()
    {
        EnsurePlayerController();
        inputFreezeCount = Mathf.Max(0, inputFreezeCount - 1);

        if (inputFreezeCount == 0 && playerController != null)
        {
            playerController.UnfreezeAllInputs();
        }
    }
    #endregion

    #region Delegated API
    public void ShowConfirmation(string title, string subtitle, Action confirmAction, Action cancelAction = null)
        => ConfirmationPanelManager.Instance?.ShowConfirmation(title, subtitle, confirmAction, cancelAction);

    public void ShowToast(string message, float duration = 1.5f)
        => ToastPanelManager.Instance?.ShowToast(message, duration);

    public IReadOnlyDictionary<PanelType, PanelFadeSettings> GetPanelSettings() => panelSettings;
    #endregion

    public void ToggleCanvasInteractivity(GameObject canvasObject, bool enable)
    {
        if (canvasObject == null || canvasObject == _GlobalCanvas)
            return;

        var raycaster = canvasObject.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
            raycaster.enabled = enable;

        foreach (var panel in panelSettings.Values)
        {
            if (panel.panel == null)
                continue;

            bool isGlobalPanel = panel.panelType == PanelType.ConfirmationPanel
                              || panel.panelType == PanelType.ToastPanel
                              || panel.panelType == PanelType.AreaTitlePanel
                              || panel.panelType == PanelType.BlackScreen;

            if (isGlobalPanel)
                continue;

            if (enable)
            {
                if (panel.panel.alpha > 0f)
                {
                    panel.panel.interactable = panel.interactable;
                    panel.panel.blocksRaycasts = panel.blockRaycasts;
                }
                else
                {
                    panel.panel.interactable = false;
                    panel.panel.blocksRaycasts = false;
                }
            }
            else
            {
                panel.panel.interactable = false;
                panel.panel.blocksRaycasts = false;
            }
        }
    }
}
