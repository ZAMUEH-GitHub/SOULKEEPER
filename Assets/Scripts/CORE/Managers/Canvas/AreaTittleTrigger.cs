using UnityEngine;
using UnityEngine.UI;

public class AreaTittleTrigger : MonoBehaviour
{
    public string AnimationToShow;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Access the Singleton directly when the player enters the trigger
            if (TemporalAreaTittleShow.Instance != null)
            {
                TemporalAreaTittleShow.Instance.showTitle(AnimationToShow);
            }
            else
            {
                Debug.LogWarning("TemporalAreaTittleShow Singleton is missing in the scene!");
            }
        }
    }
}