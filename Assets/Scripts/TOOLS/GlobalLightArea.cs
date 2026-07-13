using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class GlobalLightArea : MonoBehaviour
{
    [Header("Global Light Reference")]
    [Tooltip("Drag the Global Light 2D from your scene hierarchy here.")]
    [SerializeField] private Light2D globalLight;

    [Header("Target Settings")]
    [Tooltip("The intensity the global light will transition to when the player enters.")]
    public float targetIntensity = 0.2f;

    [Header("Transition Settings")]
    [Tooltip("How long (in seconds) the intensity transition takes.")]
    public float transitionDuration = 1.5f;

    private float originalIntensity;
    private Coroutine currentTransition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (globalLight == null)
            {
                Debug.LogWarning($"Global Light reference is missing on {gameObject.name}!");
                return;
            }

            originalIntensity = globalLight.intensity;

            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }

            currentTransition = StartCoroutine(LerpGlobalIntensity(targetIntensity, transitionDuration));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (globalLight == null) return;

            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }

            currentTransition = StartCoroutine(LerpGlobalIntensity(originalIntensity, transitionDuration));
        }
    }

    private IEnumerator LerpGlobalIntensity(float targetInt, float duration)
    {
        float startIntensity = globalLight.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            globalLight.intensity = Mathf.Lerp(startIntensity, targetInt, smoothT);

            yield return null;
        }

        globalLight.intensity = targetInt;
    }
}