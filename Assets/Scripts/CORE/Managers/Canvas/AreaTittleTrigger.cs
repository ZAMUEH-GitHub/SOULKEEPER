using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AreaTittleTrigger : MonoBehaviour
{
    public string AnimationToShow;

    [Header("Trigger Behaviors")]
    [SerializeField] private bool isOneTimePlay = true;
    [SerializeField] private bool freezesPlayer = false;
    [Tooltip("How long the player remains frozen before getting control back.")]
    [SerializeField] private float freezeDuration = 3f;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isOneTimePlay && hasTriggered) return;

            if (TemporalAreaTittleShow.Instance != null)
            {
                TemporalAreaTittleShow.Instance.showTitle(AnimationToShow);
            }
            else
            {
                Debug.LogWarning("TemporalAreaTittleShow Singleton is missing in the scene!");
            }

            if (freezesPlayer)
            {
                StartCoroutine(FreezePlayerRoutine());
            }

            if (isOneTimePlay)
            {
                hasTriggered = true;
            }
        }
    }

    private IEnumerator FreezePlayerRoutine()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.FreezeAllInputs();
            yield return new WaitForSeconds(freezeDuration);
            PlayerController.Instance.UnfreezeAllInputs();
        }
    }
}