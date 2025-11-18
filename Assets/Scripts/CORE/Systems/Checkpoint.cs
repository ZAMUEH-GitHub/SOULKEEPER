using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Threading.Tasks;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour, IInteractable
{
    [Header("Checkpoint Settings")]
    public string checkpointID;
    public GameObject activeObject;
    public bool isActiveCheckpoint;

    [Header("Interaction UI")]
    [SerializeField] private TextMeshPro interactTextMesh;
    [SerializeField] private InputActionReference interactActionRef;
    [SerializeField] private float fadeDuration = 0.5f;

    private SaveSlotManager saveSlotManager;
    private bool isPlayerInRange;
    private static bool isSaving;

    #region Unity Lifecycle
    private void Awake()
    {
        saveSlotManager = SaveSlotManager.Instance;

        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.isTrigger = true;

        SetTextInstantAlpha(interactTextMesh, 0f);
    }

    private void Start()
    {
        var global = CheckpointManager.Instance;
        isActiveCheckpoint = global.IsCheckpointActive(checkpointID) && global.IsSceneCurrent(gameObject.scene.name);
        if (activeObject) activeObject.SetActive(isActiveCheckpoint);
    }

    private void Update()
    {
        if (activeObject) activeObject.SetActive(isActiveCheckpoint);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInRange = true;
        if (!isActiveCheckpoint && !SessionManager.IsLoadingFromSave)
            ShowInteractText();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInRange = false;
        HideInteractText();
    }
    #endregion

    #region Interaction
    public async void Interact()
    {
        if (!isPlayerInRange || isSaving || isActiveCheckpoint) return;
        if (SessionManager.IsLoadingFromSave) return;

        isSaving = true;
        try
        {
            await ActivateCheckpointAsync();
        }
        finally
        {
            isSaving = false;
        }

        HideInteractText();
    }

    public string GetInteractionText()
    {
        return $"{GetInteractionKeyName()} Save Progress";
    }

    private async Task ActivateCheckpointAsync()
    {
        int slot = saveSlotManager != null ? saveSlotManager.ActiveSlotIndex : 1;
        var runtimeStats = SessionManager.Instance?.RuntimeStats;
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (runtimeStats == null)
        {
            Debug.LogWarning("[Checkpoint] No runtime stats available, aborting save.");
            return;
        }

        await SaveSystem.SaveAsync(slot, runtimeStats, null, checkpointID);

        FindFirstObjectByType<SceneCheckpointManager>()?.NotifyCheckpointActivated(this);
        ToastPanelManager.Instance?.ShowToast("Progress Saved", 3f);

        Debug.Log($"[Checkpoint] Activated '{checkpointID}' in scene '{sceneName}'.");
    }

    public void SetActiveState(bool active)
    {
        isActiveCheckpoint = active;
        if (activeObject) activeObject.SetActive(active);
    }
    #endregion

    #region UI Helpers
    private void ShowInteractText()
    {
        if (!gameObject.activeInHierarchy || interactTextMesh == null)
            return;

        if (!interactTextMesh) return;
        interactTextMesh.text = GetInteractionText();
        StartCoroutine(FadeText(interactTextMesh, 1f, false));
    }

    private void HideInteractText()
    {
        if (!gameObject.activeInHierarchy || interactTextMesh == null)
            return;

        StartCoroutine(FadeText(interactTextMesh, 0f, true));
    }

    private string GetInteractionKeyName()
    {
        if (interactActionRef == null || interactActionRef.action == null)
            return "(E)";
        try { return $"({interactActionRef.action.GetBindingDisplayString()})"; }
        catch { return "(E)"; }
    }

    private IEnumerator FadeText(TextMeshPro text, float targetAlpha, bool disableAfter)
    {
        if (!text) yield break;
        text.gameObject.SetActive(true);
        Color c = text.color;
        float start = c.a, t = 0f;

        while (t < fadeDuration)
        {
            c.a = Mathf.Lerp(start, targetAlpha, t / fadeDuration);
            text.color = c;
            t += Time.deltaTime;
            yield return null;
        }

        c.a = targetAlpha;
        text.color = c;
        if (disableAfter && targetAlpha == 0f)
            text.gameObject.SetActive(false);
    }

    private void SetTextInstantAlpha(TextMeshPro text, float alpha)
    {
        if (!text) return;
        Color c = text.color;
        c.a = alpha;
        text.color = c;
        text.gameObject.SetActive(alpha > 0f);
    }
    #endregion
}
