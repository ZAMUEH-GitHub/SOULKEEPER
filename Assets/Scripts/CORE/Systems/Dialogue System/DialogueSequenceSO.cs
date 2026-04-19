using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueSequence", menuName = "Scriptable Objects/Dialogue Sequence")]
public class DialogueSequenceSO : ScriptableObject
{
    [Tooltip("The sequence of dialogue nodes. The manager will always start at Element 0.")]
    public List<DialogueNode> nodes = new List<DialogueNode>();
}