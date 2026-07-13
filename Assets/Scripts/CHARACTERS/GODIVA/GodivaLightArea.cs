using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class GodivaLightArea : MonoBehaviour
{
    [Header("Target Light Settings")]
    [Tooltip("The intensity the light will transition to.")]
    public float targetIntensity = 1.5f;
    [Tooltip("The outer radius the light will transition to.")]
    public float targetRadius = 5f;

    [Header("Transition Settings")]
    [Tooltip("How long (in seconds) the transition takes.")]
    public float transitionDuration = 1f;

    private float originalIntensity;
    private float originalRadius;

    private Coroutine currentTransition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GodivaMovement godiva = collision.GetComponent<GodivaMovement>();

        if (collision.CompareTag("Godiva"))
        {
            Light2D godivaLight = godiva.godivaLight;
            if (godivaLight == null) return;

            originalIntensity = godivaLight.intensity;
            originalRadius = godivaLight.pointLightOuterRadius;

            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }

            currentTransition = StartCoroutine(LerpLight(godivaLight, targetIntensity, targetRadius, transitionDuration));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GodivaMovement godiva = collision.GetComponent<GodivaMovement>();

        if (collision.CompareTag("Godiva"))
        {
            Light2D godivaLight = godiva.godivaLight;
            if (godivaLight == null) return;

            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }

            currentTransition = StartCoroutine(LerpLight(godivaLight, originalIntensity, originalRadius, transitionDuration));
        }
    }

    private IEnumerator LerpLight(Light2D lightToChange, float targetInt, float targetRad, float duration)
    {
        float startIntensity = lightToChange.intensity;
        float startRadius = lightToChange.pointLightOuterRadius;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            lightToChange.intensity = Mathf.Lerp(startIntensity, targetInt, smoothT);
            lightToChange.pointLightOuterRadius = Mathf.Lerp(startRadius, targetRad, smoothT);

            yield return null;
        }

        lightToChange.intensity = targetInt;
        lightToChange.pointLightOuterRadius = targetRad;
    }
}