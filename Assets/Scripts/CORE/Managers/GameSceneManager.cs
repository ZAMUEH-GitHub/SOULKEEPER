using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    private SceneDoorManager sceneDoorManager;
    private string targetDoorID;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(SceneField scene, string targetDoorID)
    {
        this.targetDoorID = targetDoorID;
        SceneManager.LoadScene(scene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(WaitAndTeleport());
    }

    private IEnumerator WaitAndTeleport()
    {
        yield return null;

        sceneDoorManager = GameObject.FindGameObjectWithTag("Scene Door Manager").GetComponent<SceneDoorManager>();
        sceneDoorManager.ChooseDoor(targetDoorID);
    }
}
