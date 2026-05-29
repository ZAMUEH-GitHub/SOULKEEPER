using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class AltarRelic : MonoBehaviour, IInteractable
{
    [Header("Relic Settings")]
    [Tooltip("Must match the altar required flag.")]
    [SerializeField] private string relicFlag;

    [Header("Interaction UI")]
    [SerializeField] private TextMeshPro interactTextMesh;
    [SerializeField] private InputActionReference interactActionRef;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float interactionCooldown = 1f;

    [SerializeField] private string unlockedPowrUp; 

    private bool playerInRange = false;
    private bool isCollected = false;
    private float lastInteractionTime = -999f;
    private Coroutine fadeRoutine;

    private Animator relicAnim;

    #region Unity Lifecycle

    private void Awake()
    {
        SetTextInstantAlpha(interactTextMesh, 0f);
    }

    private void Start()
    {
        relicAnim = GetComponent<Animator>();
        if (SessionManager.Instance != null &&
            SessionManager.Instance.UnlockedFlags.Contains(relicFlag))
        {
            isCollected = true;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || isCollected || !gameObject.activeInHierarchy)
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

    #endregion

    #region Interaction System

    public void Interact()
    {
        if (isCollected || !CanInteract())
            return;

        if (unlockedPowrUp == "Dash" )
        {
            PlayerController.Instance.playerRuntimeStats.dashUnlocked = true;
        }
        if (unlockedPowrUp == "WallJump")
        {
            PlayerController.Instance.playerRuntimeStats.wallJumpUnlocked = true;
        }
        if (unlockedPowrUp == "DoubleJump")
        {
            PlayerController.Instance.playerRuntimeStats.maxJumpCount = 2; 
        }

        CollectRelic();
    }

    private bool CanInteract()
    {
        return Time.time - lastInteractionTime >= interactionCooldown;
    }

    private void CollectRelic()
    {
        relicAnim.SetTrigger("relicUnlocked");
        isCollected = true;
        lastInteractionTime = Time.time;

        HideInteractText();

        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.UnlockProgressFlag(relicFlag);

            Debug.Log($"[AltarRelic] Collected relic: {relicFlag}");

            ToastPanelManager.Instance?.ShowToast("Relic Found");
        }
        else
        {
            Debug.LogWarning("[AltarRelic] SessionManager instance not found!");
        }

        // FX placeholder
        // Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

      
    }
    public void OnDestroy()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region UI Logic

    private void ShowInteractText()
    {
        relicAnim.SetTrigger("playerInZone");
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

        fadeRoutine = StartCoroutine(
            FadeText(interactTextMesh, targetAlpha, disableAfter));
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
        return isCollected
            ? "Collected"
            : $"{GetInteractionKeyName()} Collect Relic";
    }

    #endregion
}