using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    [Header("Display Info")]
    [Tooltip("The name of the character speaking.")]
    public string speakerName;
    [Tooltip("The actual text the character will say.")]
    [TextArea(3, 5)]
    public string dialogueText;

    [Header("Flow and Conditions")]
    [Tooltip("Optional: A flag the player MUST have unlocked to read this node. Leave empty to bypass.")]
    public string requiredFlag;
    [Tooltip("If the player is missing the required flag, the manager will jump to this node index instead. Set to -1 to close.")]
    public int fallbackNodeIndex = -1;
    [Tooltip("The index of the next node in the sequence. Typically current index + 1. Set to -1 to close the dialogue.")]
    public int nextNodeIndex = -1;

    [Header("Progression")]
    [Tooltip("Optional: A flag to unlock in the SessionManager when this node is reached. Leave empty if none.")]
    public string unlockFlag;
}