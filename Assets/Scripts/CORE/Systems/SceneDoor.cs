using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class SceneDoor : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public string doorID;
    public SceneField targetScene;
    public string targetDoorID;

    [Header("Interaction UI")]
    [SerializeField] private TextMeshPro interactTextMesh;
    [SerializeField] private InputActionReference interactActionRef;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float interactionCooldown = 1.0f;

    private bool playerInRange = false;
    private float lastInteractionTime = -999f;

    #region Unity Lifecycle
    private void Awake()
    {
        SetTextInstantAlpha(interactTextMesh, 0f);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !gameObject.activeInHierarchy) return;

        playerInRange = true;
        ShowInteractText();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !gameObject.activeInHierarchy) return;

        playerInRange = false;
        HideInteractText();
    }
    #endregion

    #region Interaction System
    public void Interact()
    {
        if (!CanInteract()) return;

        var manager = FindFirstObjectByType<SceneDoorManager>();
        if (manager != null)
            manager.RegisterDoorUse(doorID);

        var sceneManager = FindFirstObjectByType<GameSceneManager>();
        if (sceneManager != null)
            sceneManager.LoadSceneFromDoor(targetScene, targetDoorID);

        lastInteractionTime = Time.time;
        HideInteractText();
    }

    private bool CanInteract()
    {
        return Time.time - lastInteractionTime >= interactionCooldown;
    }

        #region Text Logic
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

            #region Fade Logic
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
            #endregion

        private void SetTextInstantAlpha(TextMeshPro text, float alpha)
        {
            if (text == null) return;
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
            return $"{GetInteractionKeyName()} Enter Door";
        }
        #endregion

    #endregion
}
