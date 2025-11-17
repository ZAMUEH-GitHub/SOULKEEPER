using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;
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

    public static event Action<string> OnCheckpointActivated;

    #region Unity Lifecycle
    private void Awake()
    {
        saveSlotManager = SaveSlotManager.Instance;

        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.isTrigger = true;

        SetTextInstantAlpha(interactTextMesh, 0f);
    }

    private void OnEnable()
    {
        OnCheckpointActivated += HandleCheckpointActivated;
    }

    private void OnDisable()
    {
        OnCheckpointActivated -= HandleCheckpointActivated;
        StopAllCoroutines();
    }

    private void Start()
    {
        if (SessionManager.Instance != null)
        {
            string currentID = SessionManager.Instance.CurrentCheckpointID;
            isActiveCheckpoint = (checkpointID == currentID);
        }
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
        return $"{GetInteractionKeyName()} Save Progress";
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
        int activeSlot = saveSlotManager != null ? saveSlotManager.ActiveSlotIndex : 1;
        var runtimeStats = SessionManager.Instance.RuntimeStats;

        if (runtimeStats != null)
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            SaveSystem.Save(activeSlot, runtimeStats, null, checkpointID);
            Debug.Log($"[Checkpoint] Saved at '{checkpointID}' (Scene '{currentScene}', Slot {activeSlot})");
        }

        if (SessionManager.Instance != null)
            SessionManager.Instance.CurrentCheckpointID = checkpointID;

        OnCheckpointActivated?.Invoke(checkpointID);

        ToastPanelManager.Instance.ShowToast("Progress Saved", 2f);
    }

    private void HandleCheckpointActivated(string activeID)
    {
        bool wasActive = isActiveCheckpoint;
        isActiveCheckpoint = (checkpointID == activeID);

        if (wasActive != isActiveCheckpoint)
            Debug.Log($"[Checkpoint] {checkpointID} active state = {isActiveCheckpoint}");
    }

    public static void BroadcastActivation(string checkpointID)
    {
        OnCheckpointActivated?.Invoke(checkpointID);
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
