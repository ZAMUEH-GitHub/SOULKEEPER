using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LorePanelManager : Singleton<LorePanelManager>
{
    protected override bool IsPersistent => false;

    [Header("UI References")]
    [SerializeField] private TMP_Text loreContentText;
    [SerializeField] private GameObject nextButton;

    [Header("Animation Settings")]
    [Tooltip("Time in seconds between each letter appearing.")]
    [SerializeField] private float typingSpeed = 0.03f;

    private LoreSequenceSO currentSequence;
    private int currentParagraphIndex = 0;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private System.Action onSequenceComplete;

    public void StartLoreSequence(LoreSequenceSO sequence, System.Action onComplete = null)
    {
        if (sequence == null || sequence.paragraphs.Count == 0) return;

        currentSequence = sequence;
        currentParagraphIndex = 0;
        onSequenceComplete = onComplete;

        CanvasManager.Instance.FadeOut(PanelType.HUD);
        CanvasManager.Instance.FadeOut(PanelType.DialoguePanel);
        CanvasManager.Instance.FadeIn(PanelType.LorePanel);

        DisplayParagraph(currentParagraphIndex);
        StartCoroutine(SelectNextButtonNextFrame());
    }

    private void DisplayParagraph(int index)
    {
        if (index < 0 || index >= currentSequence.paragraphs.Count)
        {
            EndLoreSequence();
            return;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeParagraph(currentSequence.paragraphs[index]));
    }

    private IEnumerator TypeParagraph(string text)
    {
        isTyping = true;
        loreContentText.text = "";

        foreach (char c in text.ToCharArray())
        {
            loreContentText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    public void OnNextPressed()
    {
        if (currentSequence == null) return;

        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            loreContentText.text = currentSequence.paragraphs[currentParagraphIndex];
            isTyping = false;
            return;
        }

        currentParagraphIndex++;
        DisplayParagraph(currentParagraphIndex);
    }

    private void EndLoreSequence()
    {
        currentSequence = null;

        onSequenceComplete?.Invoke();
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
}