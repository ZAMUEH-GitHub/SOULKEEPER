using UnityEngine;
using System.Collections;
using System;

public class EndingSequenceController : MonoBehaviour
{
    [Header("Ending Data")]
    [Tooltip("The lore sequence to play after the dialogue.")]
    [SerializeField] private LoreSequenceSO endingLoreSequence;
    [Tooltip("The Credits scene to load into.")]
    [SerializeField] private SceneField creditsScene;

    [Header("Flags")]
    [SerializeField] private string dialogueCompleteFlag = "MinosPureDialogue_Complete";
    [SerializeField] private string gameCompletedFlag = "Game_Completed";

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
        if (flagID == dialogueCompleteFlag)
        {
            StartEndingLore();
        }
    }

    private void StartEndingLore()
    {
        if (LorePanelManager.Instance != null && endingLoreSequence != null)
        {
            LorePanelManager.Instance.StartLoreSequence(endingLoreSequence, OnEndingLoreComplete);
        }
        else
        {
            Debug.LogWarning("[EndingSequence] Missing LorePanelManager or LoreSequenceSO!");
        }
    }

    private void OnEndingLoreComplete()
    {
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.UnlockProgressFlag(gameCompletedFlag);
        }

        if (GameSceneManager.Instance != null && creditsScene != null)
        {
            GameSceneManager.Instance.LoadSceneDirect(creditsScene, Vector2.zero);
        }
    }
}