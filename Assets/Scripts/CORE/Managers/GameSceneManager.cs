using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    private SceneDoorManager sceneDoorManager;

    [SerializeField] private string targetDoorID;
    [SerializeField] private bool isLoadingScene;

    [Header("References")]
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

        SceneManager.LoadScene(scene);
        isLoadingScene = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(WaitAndTeleport());
        FindSceneDoorManager();
    }

    private IEnumerator WaitAndTeleport()
    {
        yield return new WaitForEndOfFrame();

        if (canvasManager == null)
            canvasManager = FindFirstObjectByType<CanvasManager>();

        FindSceneDoorManager();

        if (sceneDoorManager != null && !string.IsNullOrEmpty(targetDoorID))
        {
            sceneDoorManager.ChooseDoor(targetDoorID);
        }

        if (canvasManager != null)
        {
            canvasManager.FadeOut(PanelType.BlackScreen);
            yield return new WaitForSeconds(canvasManager.GetFadeDuration(PanelType.BlackScreen));
        }
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
