using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    private SceneDoorManager sceneDoorManager;
    private string targetDoorID;

    [SerializeField] private CanvasManager canvasManager;

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
        StartCoroutine(LoadSceneRoutine(scene, targetDoorID));
    }

    private IEnumerator LoadSceneRoutine(SceneField scene, string targetDoorID)
    {
        this.targetDoorID = targetDoorID;

        canvasManager.FadeIn(PanelType.BlackScreen);

        float fadeDuration = canvasManager.GetFadeDuration(PanelType.BlackScreen);
        yield return new WaitForSeconds(fadeDuration);

        SceneManager.LoadScene(scene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(WaitAndTeleport());
        FindSceneDoorManager();
    }

    private IEnumerator WaitAndTeleport()
    {
        yield return null;

        FindSceneDoorManager();

        if (sceneDoorManager != null)
            sceneDoorManager.ChooseDoor(targetDoorID);

        canvasManager.FadeOut(PanelType.BlackScreen);
    }

    private void FindSceneDoorManager()
    {
        GameObject sceneDoorObj = GameObject.FindGameObjectWithTag("Scene Door Manager");
        if (sceneDoorObj != null)
            sceneDoorManager = sceneDoorObj.GetComponent<SceneDoorManager>();
        else
            sceneDoorManager = null;
    }
}
