using System.Collections;
using UnityEngine;

public class AreaTittleTrigger : MonoBehaviour
{
    public string AnimationToShow;
    [Tooltip("Leave at 0 to use the Manager's default duration.")]
    public float DisplayDuration = 0f;

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

            if (AreaTitlePanelManager.Instance != null)
            {
                AreaTitlePanelManager.Instance.ShowAreaTitle(AnimationToShow, DisplayDuration);
            }
            else
            {
                Debug.LogWarning("AreaTitlePanelManager Singleton is missing in the scene!");
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