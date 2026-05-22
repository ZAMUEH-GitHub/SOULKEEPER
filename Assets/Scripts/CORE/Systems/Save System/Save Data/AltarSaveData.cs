using System;
using UnityEngine;

[Serializable]
public class AltarSaveData
{
    [Header("Altar Data")]
    public string altarID;
    public bool completed;
    public int currentStage;
    public bool isActivated;

    public AltarSaveData() { }

    public AltarSaveData(string id, bool completed, int stage, bool isActivated)
    {
        altarID = id;
        this.completed = completed;
        currentStage = stage;
        this.isActivated = isActivated;
    }
}