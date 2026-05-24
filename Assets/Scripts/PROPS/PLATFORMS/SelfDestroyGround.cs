using System.Collections;
using UnityEngine;

public class SelfDestroyGround : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private ParticleSystem groundDestroyParticles;
    [SerializeField] private GameObject groundDestroyObject;
    [SerializeField] private string requiredItemFlag;

    [Header("Timings")]
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private float shakeDuration = 0.4f;
    [SerializeField] private float destroyDelay = 0.3f;

    private bool destructionStarted = false;

    private void OnEnable()
    {
        GameEvents.OnFlagUnlocked += OnFlagUnlocked;
    }

    private void OnDisable()
    {
        GameEvents.OnFlagUnlocked -= OnFlagUnlocked;
    }

    private void Start()
    {
        // Handles already unlocked flags (save/load or testing)
        if (SessionManager.Instance != null &&
            SessionManager.Instance.UnlockedFlags.Contains(requiredItemFlag))
        {
            destructionStarted = true;
            StartCoroutine(DestroyGroundRoutine());
        }
    }

    private void OnFlagUnlocked(string unlockedFlag)
    {
        if (destructionStarted) return;

        if (unlockedFlag == requiredItemFlag)
        {
            destructionStarted = true;
            StartCoroutine(DestroyGroundRoutine());
        }
    }

    private IEnumerator DestroyGroundRoutine()
    {
        Debug.Log("[SelfDestroyGround] Flag detected. Starting destruction sequence.");

        yield return new WaitForSeconds(startDelay);

        // CAMERA SHAKE PLACEHOLDER
        Debug.Log("[SelfDestroyGround] CAMERA SHAKE");

        yield return new WaitForSeconds(shakeDuration);

        if (groundDestroyParticles != null)
        {
            groundDestroyParticles.Play();
        }

        yield return new WaitForSeconds(destroyDelay);

        OnDestroyGround();
    }

    private void OnDestroyGround()
    {
        Debug.Log("[SelfDestroyGround] Ground destroyed.");

        if (groundDestroyObject != null)
        {
            groundDestroyObject.SetActive(false);
        }
    }
}