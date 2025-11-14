using UnityEngine;
using System;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class AltarController : MonoBehaviour, IInteractable
{
    [Header("Altar Setup")]
    public AltarStatsSO altarSO;
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private bool completed = false;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionCooldown = 1.0f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private InputActionReference interactActionRef;
    [SerializeField] private TextMeshPro interactTextMesh;
    [SerializeField] private TextMeshPro nameTextMesh;
    [SerializeField] private TextMeshPro statusTextMesh;

    private float lastInteractionTime = -999f;
    private bool playerInRange = false;

    public static event Action<PowerUpDefinition> OnPowerUpUnlocked;

    public bool IsUsed => completed;
    public bool IsCompleted => completed;
    public int CurrentStage => currentStageIndex;
    public int TotalStages => altarSO ? altarSO.StageCount : 0;

    #region Unity Lifecycle
    private void Awake()
    {
        if (altarSO == null)
        {
            Debug.LogWarning($"[AltarController] Missing AltarStatsSO on {name}");
            completed = true;
        }

        HideAllTextsInstant();
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (!completed && interactTextMesh != null)
            interactTextMesh.gameObject.SetActive(CanInteract());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!gameObject.activeInHierarchy) return;

        playerInRange = true;
        ShowTexts();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!gameObject.activeInHierarchy) return;

        playerInRange = false;
        HideAllTexts();
    }
    #endregion

    #region Interaction System
    public void Interact()
    {
        if (!CanInteract())
            return;

        if (completed || altarSO == null)
            return;

        Debug.Log($"[AltarController] Player interacted with {altarSO.displayName}");
        UnlockNextPowerUpStage();

        lastInteractionTime = Time.time;

        if (interactTextMesh != null)
            TryStartFade(interactTextMesh, 0f, true);

        UpdateStatusText();
    }

    private bool CanInteract()
    {
        return !completed && Time.time - lastInteractionTime >= interactionCooldown;
    }

    private void UnlockNextPowerUpStage()
    {
        var def = altarSO.GetStage(currentStageIndex);
        if (def == null)
        {
            Debug.LogWarning($"[AltarController] Stage {currentStageIndex} missing definition.");
            return;
        }

        OnPowerUpUnlocked?.Invoke(def);
        Debug.Log($"[AltarController] Unlocked PowerUp: {def.name}");

        currentStageIndex++;

        if (currentStageIndex >= altarSO.StageCount)
            CompleteAltar();
    }

    private void CompleteAltar()
    {
        completed = true;
        Debug.Log($"[AltarController] {altarSO.displayName} completed!");

        if (interactTextMesh != null)
            TryStartFade(interactTextMesh, 0f, true);

        UpdateStatusText();
    }

        #region Text Logic
        private void ShowTexts()
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;

            if (nameTextMesh)
            {
                nameTextMesh.text = altarSO ? altarSO.displayName : "Unknown Altar";
                TryStartFade(nameTextMesh, 1f, false);
            }

            if (statusTextMesh)
            {
                statusTextMesh.text = completed ? "Completed" : $"Stage {currentStageIndex + 1}/{altarSO.StageCount}";
                TryStartFade(statusTextMesh, 1f, false);
            }

            if (interactTextMesh)
            {
                interactTextMesh.text = $"{GetInteractionKeyName()} Interact";
                bool canShow = !completed && CanInteract();
                if (canShow)
                    TryStartFade(interactTextMesh, 1f, false);
            }
        }

        private void HideAllTexts()
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;

            TryStartFade(nameTextMesh, 0f, true);
            TryStartFade(statusTextMesh, 0f, true);
            TryStartFade(interactTextMesh, 0f, true);
        }

        private void HideAllTextsInstant()
        {
            SetTextInstantAlpha(nameTextMesh, 0f);
            SetTextInstantAlpha(statusTextMesh, 0f);
            SetTextInstantAlpha(interactTextMesh, 0f);
        }

        private void SetTextInstantAlpha(TextMeshPro text, float alpha)
        {
            if (text == null) return;

            Color c = text.color;
            c.a = alpha;
            text.color = c;

            text.gameObject.SetActive(alpha > 0f);
        }

            #region Fade Logic
            private void TryStartFade(TextMeshPro text, float targetAlpha, bool disableAfterFade)
            {
                if (text == null) return;
                if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;

                StartCoroutine(FadeText(text, targetAlpha, disableAfterFade));
            }

            private IEnumerator FadeText(TextMeshPro text, float targetAlpha, bool disableAfterFade)
            {
                if (!text) yield break;

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
            #endregion

        private void UpdateStatusText()
        {
            if (statusTextMesh != null)
                statusTextMesh.text = completed ? "Completed" : $"Stage {currentStageIndex + 1}/{altarSO.StageCount}";
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

        public string GetInteractionText()
        {
            return $"{GetInteractionKeyName()} Interact";
        }

        public AltarSaveData ToSaveData() => new AltarSaveData(
            altarSO ? altarSO.displayName : "UnknownAltar",
            completed,
            currentStageIndex
        );

        public void FromSaveData(AltarSaveData data)
        {
            if (data == null || altarSO == null) return;

            completed = data.completed;
            currentStageIndex = data.currentStage;

            HideAllTextsInstant();
        }
    #endregion

    #endregion
}
