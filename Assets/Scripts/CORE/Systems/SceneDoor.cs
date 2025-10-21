using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour, IInteractable
{
    [Header("Load Scene")]
    [SerializeField] private SceneField loadScene;

    public void Interact()
    {
        SceneManager.LoadScene(loadScene);
    }
}
