using UnityEngine;

public class TestDialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("Test Data")]
    [Tooltip("Drag your test DialogueSequenceSO here.")]
    public DialogueSequenceSO testSequence;

    public void Interact()
    {
        if (testSequence != null && DialoguePanelManager.Instance != null)
        {
            Debug.Log("[TestDialogueTrigger] Triggering test dialogue...");
            DialoguePanelManager.Instance.StartDialogue(testSequence);
        }
        else
        {
            Debug.LogWarning("[TestDialogueTrigger] Missing DialogueSequenceSO or DialoguePanelManager!");
        }
    }

    public string GetInteractionText()
    {
        return "Read (Test)";
    }
}