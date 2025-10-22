using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour, IInteractable
{
    [Header("Load Scene")]
    [SerializeField] private SceneField loadScene;

    [Header("Door IDs")]
    public string doorID;
    public string targetDoorID;

    private GameSceneManager gameSceneManager;

    void Start()
    {
        gameSceneManager = GameObject.FindGameObjectWithTag("Game Scene Manager").GetComponent<GameSceneManager>();
    }

    public void Interact()
    {
        gameSceneManager.LoadScene(loadScene, targetDoorID);
    }
}
