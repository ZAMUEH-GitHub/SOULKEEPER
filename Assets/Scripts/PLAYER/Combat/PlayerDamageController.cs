using UnityEngine;
using System.Collections;

public class PlayerDamageController : MonoBehaviour, IKnockbackable, IDamageable
{
    [SerializeField] private PlayerStatsSO playerStats;

    [Header("Damage Settings")]
    public int playerHealth => playerStats.health;
    public bool isTakingDamage;
    public float damageRate => playerStats.damageRate;
    private float nextDamage;
    public bool isKnockedBack;

    [Header("Damage Particles")]
    public ParticleSystem damageParticles;

    #region Script and Component References
    private PlayerDeathController deathController;
    private PlayerController playerController;
    private PlayerDashController dashController;
    private Coroutine takeDamageCoroutine;

    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRB;
    #endregion

    private void Awake()
    {
        #region Script, Component and Variable Suscriptions

        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
        }

        playerController = GetComponent<PlayerController>();
        deathController = GetComponent<PlayerDeathController>();
        dashController = GetComponent<PlayerDashController>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerRB = GetComponent<Rigidbody2D>();
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
        playerStats.health -= damage;
        nextDamage = damageRate;
        playerSprite.color = Color.red;

        yield return new WaitForSeconds(0.25f);

        playerSprite.color = Color.white;
        isTakingDamage = false;

        if (playerHealth <= 0)
        {
            //animator.SetTrigger("PlayerDeath");
            playerController.FreezeAllInputs();

            deathController.Die();      // Later called by Unity Animation Event
        }
    }
    #endregion
}