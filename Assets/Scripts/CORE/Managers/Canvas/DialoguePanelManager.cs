using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialoguePanelManager : Singleton<DialoguePanelManager>
{
    protected override bool IsPersistent => false;

    [Header("UI References")]
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueContentText;
    [SerializeField] private GameObject nextButton;

    private DialogueSequenceSO currentSequence;
    private int currentNodeIndex = 0;
    private bool isClosingDialogue = false;
    private GameObject previousSelectedObject = null;

    public void StartDialogue(DialogueSequenceSO sequence)
    {
        if (sequence == null || sequence.nodes.Count == 0) return;

        currentSequence = sequence;
        currentNodeIndex = 0;

        previousSelectedObject = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;

        CanvasManager.Instance.FadeOut(PanelType.HUD);
        CanvasManager.Instance.FadeIn(PanelType.DialoguePanel);

        DisplayNode(currentNodeIndex);
        StartCoroutine(SelectNextButtonNextFrame());
    }

    private void DisplayNode(int index)
    {
        if (index < 0 || index >= currentSequence.nodes.Count)
        {
            CloseDialogue();
            return;
        }

        DialogueNode node = currentSequence.nodes[index];

        if (!string.IsNullOrEmpty(node.requiredFlag) && SessionManager.Instance != null)
        {
            if (!SessionManager.Instance.UnlockedFlags.Contains(node.requiredFlag))
            {
                DisplayNode(node.fallbackNodeIndex);
                return;
            }
        }

        if (speakerNameText != null) speakerNameText.text = node.speakerName;
        if (dialogueContentText != null) dialogueContentText.text = node.dialogueText;

        if (!string.IsNullOrEmpty(node.unlockFlag) && SessionManager.Instance != null)
        {
            SessionManager.Instance.UnlockProgressFlag(node.unlockFlag);
        }
    }

    public void OnNextPressed()
    {
        if (currentSequence == null) return;

        DialogueNode currentNode = currentSequence.nodes[currentNodeIndex];

        if (currentNode.nextNodeIndex == -1)
        {
            CloseDialogue();
        }
        else
        {
            currentNodeIndex = currentNode.nextNodeIndex;
            DisplayNode(currentNodeIndex);
        }
    }

    private void CloseDialogue()
    {
        if (isClosingDialogue) return;
        isClosingDialogue = true;

        CanvasManager.Instance.FadeOut(PanelType.DialoguePanel);
        CanvasManager.Instance.FadeIn(PanelType.HUD);

        StartCoroutine(RestoreFocusAfterFade());

        currentSequence = null;
        isClosingDialogue = false;
    }

    private IEnumerator SelectNextButtonNextFrame()
    {
        yield return null;
        if (nextButton != null && EventSystem.current != null)
        {
            var selectable = nextButton.GetComponent<Selectable>();
            if (nextButton.activeInHierarchy && selectable != null && selectable.IsInteractable())
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(nextButton);
                selectable.OnSelect(null);
            }
        }
    }

    private IEnumerator RestoreFocusAfterFade()
    {
        yield return new WaitForSeconds(CanvasManager.Instance.GetFadeDuration(PanelType.DialoguePanel));

        if (EventSystem.current == null || previousSelectedObject == null) yield break;

        var sel = previousSelectedObject.GetComponent<Selectable>();
        if (previousSelectedObject.activeInHierarchy && sel != null && sel.IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(previousSelectedObject);
            sel.OnSelect(null);
        }
        previousSelectedObject = null;
    }
}