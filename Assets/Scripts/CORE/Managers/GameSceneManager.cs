using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    private SceneDoorManager sceneDoorManager;
    [SerializeField] private string targetDoorID;
    [SerializeField] private bool isLoadingScene;
    [Space(5)]
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
        if (isLoadingScene) return;
        StartCoroutine(LoadSceneRoutine(scene, targetDoorID));
    }

    private IEnumerator LoadSceneRoutine(SceneField scene, string targetDoorID)
    {
        isLoadingScene = true;

        this.targetDoorID = targetDoorID;

        if (canvasManager != null)
            canvasManager.FadeIn(PanelType.BlackScreen);

        yield return new WaitForSeconds(canvasManager.GetFadeDuration(PanelType.BlackScreen));

        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        isLoadingScene = false;
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

        if (canvasManager != null)
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
