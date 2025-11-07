using System.Collections.Generic;
using UnityEngine;

public enum AltarType { Health, Attack, Jump, Dash, Wall, Custom }

[CreateAssetMenu(menuName = "Scriptable Objects/Altars/Definition")]
public class AltarStatsSO : ScriptableObject
{
    public string displayName;
    public AltarType altarType = AltarType.Custom;

    [Tooltip("Ordered list of power-ups this altar grants, one per interaction.")]
    public List<PowerUpDefinition> stages = new List<PowerUpDefinition>();

    public int StageCount => stages?.Count ?? 0;

    public PowerUpDefinition GetStage(int index)
    {
        if (stages == null || index < 0 || index >= stages.Count) return null;
        return stages[index];
    }
}
