using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    [Header("Game Data")]
    public PlayerSaveData playerData;
    public List<AltarSaveData> altarData = new List<AltarSaveData>();
    public float totalPlaytime;
    public string timestamp;
    public int version = 1;

    [Header("Scene Transition Data")]
    public string currentScene;
    public string lastDoorID;

    public GameSaveData()
    {
        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
