using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class TriggerBossFight : MonoBehaviour, IInteractable
{
    [Header("Boss References")]
    [Tooltip("Drag the BossStateManager here to start the encounter.")]
    public BossStateManager bossStateManager;

    [Header("Interaction UI")]
    [SerializeField] private TextMeshPro interactTextMesh;
    [SerializeField] private InputActionReference interactActionRef;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float interactionCooldown = 1f;

    private bool playerInRange = false;
    private float lastInteractionTime = -999f;
    private Coroutine fadeRoutine;
    private bool hasTriggered = false;

    #region Unity Lifecycle
    private void Awake()
    {
        SetTextInstantAlpha(interactTextMesh, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !gameObject.activeInHierarchy || hasTriggered)
            return;

        playerInRange = true;
        ShowInteractText();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !gameObject.activeInHierarchy)
            return;

        playerInRange = false;
        HideInteractText();
    }

    private void OnDisable()
    {
        playerInRange = false;
        SetTextInstantAlpha(interactTextMesh, 0f);
    }
    #endregion

    #region Interaction System
    public void Interact()
    {
        if (!CanInteract() || hasTriggered)
            return;

        if (bossStateManager != null)
        {
            hasTriggered = true;
            lastInteractionTime = Time.time;
            HideInteractText();

            bossStateManager.StartBossFight();

            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            Debug.LogWarning("[TriggerBossFight] Missing BossStateManager reference!");
        }
    }

    private bool CanInteract()
    {
        return Time.time - lastInteractionTime >= interactionCooldown;
    }
    #endregion

    #region UI Logic
    private void ShowInteractText()
    {
        if (!interactTextMesh) return;

        interactTextMesh.text = GetInteractionText();
        StartFade(1f, false);
    }

    private void HideInteractText()
    {
        StartFade(0f, true);
    }

    private void StartFade(float targetAlpha, bool disableAfter)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeText(interactTextMesh, targetAlpha, disableAfter));
    }

    private IEnumerator FadeText(TextMeshPro text, float targetAlpha, bool disableAfter)
    {
        if (!text) yield break;

        text.gameObject.SetActive(true);

        Color color = text.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            if (!gameObject.activeInHierarchy)
                yield break;

            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / fadeDuration);

            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            text.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        text.color = color;

        if (disableAfter && Mathf.Approximately(targetAlpha, 0f))
        {
            text.gameObject.SetActive(false);
        }

        fadeRoutine = null;
    }

    private void SetTextInstantAlpha(TextMeshPro text, float alpha)
    {
        if (!text) return;

        Color c = text.color;
        c.a = alpha;
        text.color = c;

        text.gameObject.SetActive(alpha > 0f);
    }

    private string GetInteractionKeyName()
    {
        if (interactActionRef == null || interactActionRef.action == null)
            return "(E)";

        try
        {
            return $"({interactActionRef.action.GetBindingDisplayString()})";
        }
        catch
        {
            return "(E)";
        }
    }

    public string GetInteractionText()
    {
        return $"{GetInteractionKeyName()} Challenge";
    }
    #endregion
}