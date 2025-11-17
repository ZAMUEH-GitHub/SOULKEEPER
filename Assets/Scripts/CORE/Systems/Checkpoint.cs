using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour, IInteractable
{
    [Header("Checkpoint Settings")]
    public string checkpointID;
    public bool isActiveCheckpoint;

    [Header("Interaction UI")]
    [SerializeField] private TextMeshPro interactTextMesh;
    [SerializeField] private InputActionReference interactActionRef;
    [SerializeField] private float fadeDuration = 0.5f;

    private SaveSlotManager saveSlotManager;
    private bool isPlayerInRange;

    #region Unity Lifecycle
    private void Awake()
    {
        saveSlotManager = SaveSlotManager.Instance;
        isActiveCheckpoint = false;

        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.isTrigger = true;

        SetTextInstantAlpha(interactTextMesh, 0f);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInRange = true;
        if (!isActiveCheckpoint)
            ShowInteractText();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInRange = false;
        HideInteractText();
    }
    #endregion

    #region Interaction System
    public void Interact()
    {
        if (!isPlayerInRange) return;

        ActivateCheckpoint();
        HideInteractText();
    }

    public string GetInteractionText()
    {
        return isActiveCheckpoint ? "" : $"{GetInteractionKeyName()} Save Progress";
    }

    private string GetInteractionKeyName()
    {
        if (interactActionRef == null || interactActionRef.action == null)
            return "(E)";

        try
        {
            string displayString = interactActionRef.action.GetBindingDisplayString();
            return $"({displayString})";
        }
        catch
        {
            return "(E)";
        }
    }
    #endregion

    #region Activation Logic
    private void ActivateCheckpoint()
    {
        isActiveCheckpoint = true;

        int activeSlot = saveSlotManager != null ? saveSlotManager.ActiveSlotIndex : 1;
        var runtimeStats = SessionManager.Instance.RuntimeStats;

        if (runtimeStats != null)
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Debug.Log($"[Checkpoint] Saving checkpoint '{checkpointID}' in scene '{currentScene}' (Slot {activeSlot})");
            SaveSystem.Save(activeSlot, runtimeStats, null, checkpointID);
        }
        else
        {
            Debug.LogWarning("[Checkpoint] Could not find runtime stats for saving!");
        }

        if (SessionManager.Instance != null)
            SessionManager.Instance.CurrentCheckpointID = checkpointID;

        ToastPanelManager.Instance.ShowToast("Progress Saved", 2f);
    }
    #endregion

    #region Text Fade Logic
    private void ShowInteractText()
    {
        if (!interactTextMesh) return;
        interactTextMesh.text = GetInteractionText();
        TryStartFade(interactTextMesh, 1f, false);
    }

    private void HideInteractText()
    {
        TryStartFade(interactTextMesh, 0f, true);
    }

    private void TryStartFade(TextMeshPro text, float targetAlpha, bool disableAfterFade)
    {
        if (!isActiveAndEnabled || !gameObject.activeInHierarchy || text == null) return;
        StartCoroutine(FadeText(text, targetAlpha, disableAfterFade));
    }

    private IEnumerator FadeText(TextMeshPro text, float targetAlpha, bool disableAfterFade)
    {
        text.gameObject.SetActive(true);
        Color color = text.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) yield break;

            float t = time / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            text.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a = targetAlpha;
        text.color = color;

        if (disableAfterFade && targetAlpha == 0f)
            text.gameObject.SetActive(false);
    }

    private void SetTextInstantAlpha(TextMeshPro text, float alpha)
    {
        if (text == null) return;
        Color c = text.color;
        c.a = alpha;
        text.color = c;
        text.gameObject.SetActive(alpha > 0f);
    }
    #endregion
}
