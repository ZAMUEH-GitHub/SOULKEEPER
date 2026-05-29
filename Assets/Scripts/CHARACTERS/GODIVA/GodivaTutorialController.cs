using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TutorialPhase
{
    [Header("Departure Phase")]
    [Tooltip("The flag that starts this phase's movement (e.g., 'IntroDialogueFinished').")]
    public string triggerFlag;

    [Tooltip("The trigger to send to Godiva's Animator to start moving.")]
    public string animatorTrigger;

    [Tooltip("The dialogue to load for when the player talks to her at the next location.")]
    public DialogueSequenceSO nextDialogue;

    [Header("Arrival Phase")]
    [Tooltip("The flag fired by the Animation Event when Godiva arrives at her destination.")]
    public string arrivalFlag;
}

public class GodivaTutorialController : MonoBehaviour
{
    [Header("Godiva References")]
    [SerializeField] private Animator godivaAnimator;
    [SerializeField] private BoxCollider2D interactionCollider;
    [SerializeField] private DialogueTrigger dialogueTrigger;

    [Header("Tutorial Flow")]
    [SerializeField] private List<TutorialPhase> tutorialPhases = new List<TutorialPhase>();

    private void OnEnable()
    {
        GameEvents.OnFlagUnlocked += HandleFlagUnlocked;
    }

    private void OnDisable()
    {
        GameEvents.OnFlagUnlocked -= HandleFlagUnlocked;
    }

    private void HandleFlagUnlocked(string flagID)
    {
        foreach (var phase in tutorialPhases)
        {
            // Step 1: The Departure
            if (flagID == phase.triggerFlag)
            {
                // Disable interaction while she moves
                if (interactionCollider != null)
                    interactionCollider.enabled = false;

                // Load the next dialogue sequence
                if (dialogueTrigger != null)
                    dialogueTrigger.dialogueSequence = phase.nextDialogue;

                // Trigger the movement animation
                if (godivaAnimator != null && !string.IsNullOrEmpty(phase.animatorTrigger))
                {
                    godivaAnimator.SetTrigger(phase.animatorTrigger);
                }

                return; // Stop searching once we found a match
            }

            // Step 2: The Arrival
            if (flagID == phase.arrivalFlag)
            {
                // Re-enable interaction now that she is waiting in Idle
                if (interactionCollider != null)
                    interactionCollider.enabled = true;

                return; // Stop searching once we found a match
            }
        }
    }
}