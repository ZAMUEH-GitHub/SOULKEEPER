/*using UnityEngine;

public class SceneDoor : MonoBehaviour
{
    [Tooltip("Name of the scene to load when player enters this door")]
    public string sceneToLoad;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameSceneManager.Instance.LoadScene(sceneToLoad);
        }
    }
}
*/