using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SceneCheckpointManager : MonoBehaviour
{
    public List<Checkpoint> checkpoints = new();


    private void Awake()
    {
        checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None).ToList();
    }


    private IEnumerator Start()
    {
        yield return null;
        RefreshActiveCheckpoint();
    }


    public void NotifyCheckpointActivated(Checkpoint checkpoint)
    {
        CheckpointManager.Instance.RegisterActivation(checkpoint.checkpointID);


        foreach (var cp in checkpoints)
            cp.SetActiveState(cp == checkpoint);
    }


    public void RefreshActiveCheckpoint()
    {
        var global = CheckpointManager.Instance;
        foreach (var cp in checkpoints)
            cp.SetActiveState(global.IsCheckpointActive(cp.checkpointID) && global.IsSceneCurrent(gameObject.scene.name));
    }


    public Checkpoint GetCheckpoint(string id)
    {
        return checkpoints.FirstOrDefault(c => c.checkpointID == id);
    }
}