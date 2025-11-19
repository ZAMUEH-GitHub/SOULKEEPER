using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConfirmationRequest
{
    public string message;
    public Action onConfirm;
    public Action onCancel;

    public ConfirmationRequest(string message, Action confirmAction, Action cancelAction = null)
    {
        this.message = message;
        this.onConfirm = confirmAction;
        this.onCancel = cancelAction;
    }
}

public class ConfirmationPanelManager : Singleton<ConfirmationPanelManager>
{
    protected override bool IsPersistent => false;

    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private GameObject confirmButton;

    private ConfirmationRequest currentRequest;
    private bool isClosingConfirmation = false;
    private readonly List<PanelFadeSettings> temporarilyDisabledPanels = new();

    private PanelType? previousPanelType = null;
    private GameObject previousSelectedObject = null;

    public void ShowConfirmation(string title, string subtitle, Action confirmAction, Action cancelAction = null)
    {
        var canvasManager = CanvasManager.Instance;
        currentRequest = new ConfirmationRequest(title, confirmAction, cancelAction);

        if (titleText != null) titleText.text = title;
        if (subtitleText != null) subtitleText.text = subtitle;

        previousSelectedObject = EventSystem.current != null
            ? EventSystem.current.currentSelectedGameObject
            : null;
        previousPanelType = null;

        temporarilyDisabledPanels.Clear();

        foreach (var kvp in canvasManager.GetPanelSettings())
        {
            var p = kvp.Value;
            if (p.panelType == PanelType.ConfirmationPanel || p.panelType == PanelType.ToastPanel)
                continue;

            if (previousPanelType == null && previousSelectedObject != null && p.panel != null &&
                previousSelectedObject.transform.IsChildOf(p.panel.transform))
            {
                previousPanelType = p.panelType;
            }

            if (p.panel != null && p.panel.alpha > 0.95f && p.panel.interactable)
            {
                p.panel.interactable = false;
                p.panel.blocksRaycasts = false;
                temporarilyDisabledPanels.Add(p);
            }
        }

        if (previousPanelType == null && temporarilyDisabledPanels.Count > 0)
        {
            var candidate = temporarilyDisabledPanels.Find(pp => pp.firstSelected != null);
            previousPanelType = candidate != null ? candidate.panelType : temporarilyDisabledPanels[0].panelType;
        }

        canvasManager.FadeIn(PanelType.ConfirmationPanel);

        StartCoroutine(SelectConfirmButtonNextFrame());
    }

    private IEnumerator SelectConfirmButtonNextFrame()
    {
        yield return null;

        if (confirmButton != null && EventSystem.current != null)
        {
            var selectable = confirmButton.GetComponent<Selectable>();
            if (confirmButton.activeInHierarchy && selectable != null && selectable.IsInteractable())
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(confirmButton);
                selectable.OnSelect(null);
            }
        }
    }

    public void OnConfirmPressed()
    {
        currentRequest?.onConfirm?.Invoke();
        CloseConfirmation();
    }

    public void OnCancelPressed()
    {
        currentRequest?.onCancel?.Invoke();
        CloseConfirmation();
    }

    private void CloseConfirmation()
    {
        if (isClosingConfirmation) return;
        isClosingConfirmation = true;

        foreach (var p in temporarilyDisabledPanels)
        {
            if (p.panel == null) continue;
            p.panel.interactable = true;
            p.panel.blocksRaycasts = true;
        }
        temporarilyDisabledPanels.Clear();

        CanvasManager.Instance.FadeOut(PanelType.ConfirmationPanel);
        StartCoroutine(RestoreFocusAfterFade());

        currentRequest = null;
        isClosingConfirmation = false;
    }

    private IEnumerator RestoreFocusAfterFade()
    {
        yield return new WaitForSeconds(CanvasManager.Instance.GetFadeDuration(PanelType.ConfirmationPanel));
        if (EventSystem.current == null) yield break;

        if (previousSelectedObject != null)
        {
            var sel = previousSelectedObject.GetComponent<Selectable>();
            if (previousSelectedObject.activeInHierarchy && sel != null && sel.IsInteractable())
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(previousSelectedObject);
                sel.OnSelect(null);
                previousSelectedObject = null;
                previousPanelType = null;
                yield break;
            }
        }

        if (previousPanelType.HasValue)
        {
            var map = CanvasManager.Instance.GetPanelSettings();
            if (map.TryGetValue(previousPanelType.Value, out var s) && s.firstSelected != null)
            {
                var sel = s.firstSelected.GetComponent<Selectable>();
                if (s.firstSelected.activeInHierarchy && sel != null && sel.IsInteractable())
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(s.firstSelected);
                    sel.OnSelect(null);
                    previousSelectedObject = null;
                    previousPanelType = null;
                    yield break;
                }
            }
        }

        foreach (var s in CanvasManager.Instance.GetPanelSettings().Values)
        {
            if (s.panel != null && s.panel.alpha > 0.95f && s.panel.interactable && s.firstSelected != null)
            {
                var sel = s.firstSelected.GetComponent<Selectable>();
                if (s.firstSelected.activeInHierarchy && sel != null && sel.IsInteractable())
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(s.firstSelected);
                    sel.OnSelect(null);
                    previousSelectedObject = null;
                    previousPanelType = null;
                    yield break;
                }
            }
        }

        var gm = GameManager.Instance;
        PanelType fallback = (gm != null && gm.CurrentState == GameState.MainMenu)
            ? PanelType.MainMenu : PanelType.HUD;

        var dict = CanvasManager.Instance.GetPanelSettings();
        if (dict.TryGetValue(fallback, out var fs) && fs.firstSelected != null)
        {
            var sel = fs.firstSelected.GetComponent<Selectable>();
            if (fs.firstSelected.activeInHierarchy && sel != null && sel.IsInteractable())
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(fs.firstSelected);
                sel.OnSelect(null);
            }
        }

        previousSelectedObject = null;
        previousPanelType = null;
    }
}
