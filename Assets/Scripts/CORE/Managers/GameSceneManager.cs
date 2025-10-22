using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private SceneDoorManager sceneDoorManager;
    private int doorName;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(SceneField scene, int doorName)
    {
        SceneManager.LoadScene(scene);
        this.doorName = doorName;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneDoorManager = GameObject.FindGameObjectWithTag("Scene Door Manager").GetComponent<SceneDoorManager>();
        sceneDoorManager.ChooseDoor(doorName);
    }
}
