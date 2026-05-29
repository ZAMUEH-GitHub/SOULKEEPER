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
}