using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryPhaseRequirement
{
    [Tooltip("The Story State to advance to if conditions are met.")]
    public int targetStoryState;

    [Tooltip("All flags that must be unlocked to reach this state.")]
    public List<string> requiredFlags = new List<string>();
}

public class StoryManager : MonoBehaviour
{
    [Header("Story Progression Rules")]
    [Tooltip("Define the flag requirements to reach each story state in the Inspector.")]
    [SerializeField] private List<StoryPhaseRequirement> progressionPhases = new List<StoryPhaseRequirement>();

    private void OnEnable()
    {
        GameEvents.OnFlagUnlocked += EvaluateStoryProgression;
    }

    private void OnDisable()
    {
        GameEvents.OnFlagUnlocked -= EvaluateStoryProgression;
    }

    private void Start()
    {
        EvaluateStoryProgression(string.Empty);
    }

    private void EvaluateStoryProgression(string newlyUnlockedFlag)
    {
        if (SessionManager.Instance == null) return;

        int currentState = SessionManager.Instance.CurrentStoryState;

        foreach (var phase in progressionPhases)
        {
            if (phase.targetStoryState > currentState)
            {
                if (HasAllRequiredFlags(phase.requiredFlags))
                {
                    SessionManager.Instance.SetStoryState(phase.targetStoryState);
                    Debug.Log($"[StoryManager] Conditions met! Advanced to Story State {phase.targetStoryState}.");

                    currentState = phase.targetStoryState;
                }
            }
        }
    }

    private bool HasAllRequiredFlags(List<string> requiredFlags)
    {
        if (requiredFlags == null || requiredFlags.Count == 0) return false;

        foreach (string flag in requiredFlags)
        {
            if (!SessionManager.Instance.UnlockedFlags.Contains(flag))
            {
                return false;
            }
        }

        return true;
    }
}