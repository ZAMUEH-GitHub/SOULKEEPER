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
    public string currentSceneID;
    public string currentCheckpointID;
    public string lastDoorID;

    [Header("Story Progress")]
    public int currentStoryState;
    public List<string> unlockedFlags = new List<string>();

    [Header("Player Position")]
    public Vector2 lastPlayerPosition;

    public GameSaveData()
    {
        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
