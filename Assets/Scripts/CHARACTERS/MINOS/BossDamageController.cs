using System.Collections;
using UnityEngine;

public class BossDamageController : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHealth = 50;
    public int currentHealth;
    [Tooltip("Invincibility duration after taking a hit.")]
    public float damageRate = 0.25f;

    private float nextDamage;
    private bool isTakingDamage;

    [Header("Visual Effects")]
    [Tooltip("Assign all sprite renderers that make up the boss here to flash them red.")]
    public SpriteRenderer[] bossSprites;
    public ParticleSystem damageParticles;

    [Header("State References")]
    public BossStateManager stateManager;

    [Header("Death Scene Transition")]
    public SceneField targetSceneOnDeath;
    public Vector2 targetSpawnPosition;
    [Tooltip("The ID used to record this save event (e.g., MinosBoss_Defeat)")]
    public string bossSaveID = "MinosBoss_Defeat";

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (nextDamage > 0)
        {
            nextDamage -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage, Vector2 damageVector)
    {
        if (currentHealth <= 0) return;
        if (stateManager != null && stateManager.currentPhase != BossStateManager.BossPhase.Phase2_Brawler) return;

        if (!isTakingDamage && nextDamage <= 0)
        {
            StartCoroutine(ExecuteTakeDamage(damage, damageVector));
        }
    }

    private IEnumerator ExecuteTakeDamage(int damage, Vector2 damageVector)
    {
        isTakingDamage = true;
        currentHealth -= damage;
        nextDamage = damageRate;

        if (damageParticles != null)
        {
            Quaternion particleRotation = Quaternion.FromToRotation(Vector2.up, damageVector);
            Instantiate(damageParticles, transform.position, particleRotation);
        }

        foreach (SpriteRenderer sr in bossSprites)
        {
            if (sr != null) sr.color = Color.red;
        }

        yield return new WaitForSeconds(0.15f);

        foreach (SpriteRenderer sr in bossSprites)
        {
            if (sr != null) sr.color = Color.white;
        }

        isTakingDamage = false;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (stateManager != null && stateManager.combatController != null)
        {
            stateManager.combatController.enabled = false;
        }

        if (stateManager != null && stateManager.bossAnimator != null)
        {
            stateManager.bossAnimator.SetTrigger("Death");
        }

        Debug.Log("[BossDamageController] The Boss has been defeated!");
    }

    public async void EndDeath()
    {
        Debug.Log("[BossDamageController] Death Animation Complete. Performing soft save...");

        var runtimeStats = SessionManager.Instance?.RuntimeStats;
        int slot = SaveSlotManager.Instance != null ? SaveSlotManager.Instance.ActiveSlotIndex : 1;

        if (runtimeStats != null)
        {
            await SaveSystem.SaveAsync(slot, runtimeStats, bossSaveID, null, false);
            Debug.Log($"[BossDamageController] Soft Save completed for '{bossSaveID}'");
        }

        Debug.Log("[BossDamageController] Loading next scene directly.");

        if (GameSceneManager.Instance != null && targetSceneOnDeath != null)
        {
            GameSceneManager.Instance.LoadSceneDirect(targetSceneOnDeath, targetSpawnPosition);
        }
    }
}