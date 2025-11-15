using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PowerUps/Definition")]
public class PowerUpDefinition : ScriptableObject
{
    public string displayName;
    public List<PowerUp> effects = new List<PowerUp>();
}
