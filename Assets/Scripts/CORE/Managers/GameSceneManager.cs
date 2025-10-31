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
        sceneDoorManager = GameObject.FindGameObjectWithTag("Scene Door Manager").GetComponent<SceneDoorManager>();
    }

    private IEnumerator WaitAndTeleport()
    {
        yield return null;

        sceneDoorManager = GameObject.FindGameObjectWithTag("Scene Door Manager").GetComponent<SceneDoorManager>();

        if (sceneDoorManager != null)
            sceneDoorManager.ChooseDoor(targetDoorID);

        canvasManager.FadeOut(PanelType.BlackScreen);
    }
}
