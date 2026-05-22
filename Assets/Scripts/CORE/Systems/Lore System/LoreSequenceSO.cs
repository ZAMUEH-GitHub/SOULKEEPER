using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLoreSequence", menuName = "Scriptable Objects/Lore Sequence")]
public class LoreSequenceSO : ScriptableObject
{
    [Header("Lore Content")]
    [Tooltip("Each string in this list represents one 'page' or 'node' of text.")]
    [TextArea(3, 8)]
    public List<string> paragraphs = new List<string>();
}