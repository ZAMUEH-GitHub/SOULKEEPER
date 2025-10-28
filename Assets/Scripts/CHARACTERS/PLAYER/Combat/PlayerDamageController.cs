using UnityEngine;
using System.Collections;

public class PlayerDamageController : MonoBehaviour, IKnockbackable, IDamageable
{
    public PlayerStatsSO playerStats;

    [Header("Damage Settings")]
    public int playerHealth;
    public bool isTakingDamage;
    public float damageRate;
    private float nextDamage;
    public bool isKnockedBack;

    public GameObject soulObject;
    public ParticleSystem damageParticles;
    public ParticleSystem deathParticles;

    #region Script and Component References

    private PlayerController playerController;
    private PlayerDashController dashController;
    private Coroutine takeDamageCoroutine;

    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRB;
    #endregion

    private void Awake()
    {
        #region Script, Component and Variable Suscriptions

        playerController = GetComponent<PlayerController>();
        dashController = GetComponent<PlayerDashController>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerRB = GetComponent<Rigidbody2D>();

        playerHealth = playerStats.health;
        damageRate = playerStats.damageRate;
        #endregion
    }

    private void Update()
    {
        nextDamage = Mathf.Max(0, nextDamage - Time.deltaTime);
    }

    #region Knockback System

    public void Knockback(Vector2 knockbackVector, float knockbackForce, float knockbackDuration)
    {
        playerRB.AddForce(knockbackVector * knockbackForce, ForceMode2D.Impulse);
        playerRB.linearVelocity = (new Vector2(knockbackVector.x, knockbackVector.y + 1) * knockbackForce);
        isKnockedBack = true;
        StartCoroutine(CancelKnockback(knockbackDuration));
    }

    public IEnumerator CancelKnockback(float knockbackDuration)
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    #endregion

    #region Damage System

    public void TakeDamage(int damage, Vector2 damageVector)
    {
        Quaternion particleRotation = Quaternion.FromToRotation(Vector2.up, damageVector);

        if (!dashController.isDashing && !isTakingDamage && nextDamage <= 0)
        {
            if (takeDamageCoroutine != null)
            {
                StopCoroutine(takeDamageCoroutine);
            }
            takeDamageCoroutine = StartCoroutine(ExecuteTakeDamage(damage));
            Instantiate(damageParticles, transform.position, particleRotation);
        }
    }


    private IEnumerator ExecuteTakeDamage(int damage)
    {
        isTakingDamage = true;
        playerHealth -= damage;
        nextDamage = damageRate;
        playerSprite.color = Color.red;

        yield return new WaitForSeconds(0.25f);

        playerSprite.color = Color.white;
        isTakingDamage = false;

        if (playerHealth <= 0)
        {
            Die();
        }
    }

    #endregion

    public void Die()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity);

        for (int i = playerController.playerScore / 2; i > 0; i--)
        {
            GameObject soul = Instantiate(soulObject, transform.position, Quaternion.identity);
            soul.transform.position = new Vector2(soul.transform.position.x + Random.Range(-2f, 2f), soul.transform.position.y + Random.Range(-1.5f, 2f));
        }
    }
}