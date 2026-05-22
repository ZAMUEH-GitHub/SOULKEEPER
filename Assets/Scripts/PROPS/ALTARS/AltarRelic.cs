using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AltarRelic : MonoBehaviour
{
    [Header("Relic Settings")]
    [Tooltip("This must exactly match the 'Required Item Flag' on the corresponding AltarController (e.g., 'FoundDashRelic').")]
    public string relicFlag;
    /*
    [Header("Visuals & Audio")]
    [SerializeField] private GameObject pickupEffectPrefab;
    [SerializeField] private AudioClip pickupSound; 
    */
    private bool isCollected = false;

    private void Start()
    {
        if (SessionManager.Instance != null && SessionManager.Instance.UnlockedFlags.Contains(relicFlag))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected || !gameObject.activeInHierarchy) return;

        if (collision.CompareTag("Player"))
        {
            CollectRelic();
        }
    }

    private void CollectRelic()
    {
        isCollected = true;

        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.UnlockedFlags.Add(relicFlag);
            Debug.Log($"[AltarRelic] Collected relic: {relicFlag}. The corresponding Altar can now be activated!");

            ToastPanelManager.Instance?.ShowToast("Relic Found");
        }
        else
        {
            Debug.LogWarning("[AltarRelic] SessionManager instance not found! Flag not saved.");
        }
        /*
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }
        */
        gameObject.SetActive(false);
    }
}