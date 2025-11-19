using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class AreaTitleTrigger : MonoBehaviour
{
    [Header("Area Title")]
    [SerializeField] private string titleMessage;
    [SerializeField] private string subtitleMessage;
    [SerializeField] private float messageDuration;

    private Collider2D cL;

    private IEnumerator Start()
    {
        cL = GetComponent<Collider2D>();
        cL.enabled = false;

        yield return new WaitForSeconds(0.5f);

        cL.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AreaTitlePanelManager.Instance.ShowAreaTitle(titleMessage, subtitleMessage, messageDuration);
        }
    }
}
