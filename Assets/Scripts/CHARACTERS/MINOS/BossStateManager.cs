using System.Collections;
using UnityEngine;

public class BossStateManager : MonoBehaviour
{
    public enum BossPhase
    {
        Phase0_Intro,
        Phase1_Waves,
        Phase2_Brawler
    }

    [Header("Current State")]
    public BossPhase currentPhase = BossPhase.Phase0_Intro;

    [Header("Component References")]
    public Animator bossAnimator;
    public Phase1Controller waveSpawner;         
    public Phase2Controller combatController;

    [Header("Phase 2 Animation Settings")]
    [Tooltip("The index of the Animator Layer used for Phase 2 attacks.")]
    public int phase2LayerIndex = 1;
    [Tooltip("How long it takes to blend the Animator Layer weight from 0 to 1.")]
    public float layerTransitionDuration = 1.0f;

    private void Start()
    {
        currentPhase = BossPhase.Phase0_Intro;

        if (bossAnimator != null)
        {
            bossAnimator.SetLayerWeight(phase2LayerIndex, 0f);
        }
    }

    public void StartBossFight()
    {
        if (currentPhase != BossPhase.Phase0_Intro) return;

        currentPhase = BossPhase.Phase1_Waves;

        bossAnimator.SetTrigger("StartPhase1");

        waveSpawner.StartWaves(); 
    }

    public void TransitionToPhase2()
    {
        if (currentPhase != BossPhase.Phase1_Waves) return;

        currentPhase = BossPhase.Phase2_Brawler;

        bossAnimator.SetTrigger("StartPhase2");

        StartCoroutine(LerpAnimatorLayerWeight(phase2LayerIndex, 1f, layerTransitionDuration));

        combatController.EnableCombat();
    }

    private IEnumerator LerpAnimatorLayerWeight(int layerIndex, float targetWeight, float duration)
    {
        float startWeight = bossAnimator.GetLayerWeight(layerIndex);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newWeight = Mathf.Lerp(startWeight, targetWeight, elapsedTime / duration);
            bossAnimator.SetLayerWeight(layerIndex, newWeight);

            yield return null;
        }

        bossAnimator.SetLayerWeight(layerIndex, targetWeight);
    }
}