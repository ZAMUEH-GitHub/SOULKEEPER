using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour, IInteractable
{
    [Header("Load Scene")]
    [SerializeField] private SceneField loadScene;
    public int doorName;

    private GameSceneManager gameSceneManager;

    void Start()
    {
        gameSceneManager = GameObject.FindGameObjectWithTag("Game Scene Manager").GetComponent<GameSceneManager>();
    }

    public void Interact()
    {
        gameSceneManager.LoadScene(loadScene, doorName);
    }
}
