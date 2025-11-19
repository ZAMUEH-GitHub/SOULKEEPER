using System;
using UnityEngine;

[Serializable]
public class AltarSaveData
{
    [Header("Altar Data")]
    public string altarID;
    public bool completed;
    public int currentStage;

    public AltarSaveData() { }

    public AltarSaveData(string id, bool completed, int stage)
    {
        altarID = id;
        this.completed = completed;
        currentStage = stage;
    }
}
