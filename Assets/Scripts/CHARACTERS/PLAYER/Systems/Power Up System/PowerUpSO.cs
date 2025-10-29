using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PowerUps/Definition")]
public class PowerUpDefinition : ScriptableObject
{
    public string displayName;
    public List<PowerUpEffect> effects = new List<PowerUpEffect>();
}
