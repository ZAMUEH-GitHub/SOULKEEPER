using UnityEngine;
using System.Collections;

public class PlayerDamageController : MonoBehaviour, IKnockbackable, IDamageable
{
    private PlayerStatsSO playerStats;

    [Header("Damage Settings")]
    public int playerHealth => playerStats.health;
    public bool isTakingDamage;
    public float damageRate => playerStats.damageRate;
    private float nextDamage;
    public bool isKnockedBack;
    [HideInInspector] public float currentKnockbackDuration;

    [Header("VFX Settings")]
    [SerializeField] private ParticleSystem damageParticles;
    [SerializeField] private SpriteRenderer[] playerSprite;

    #region Script and Component References
    private PlayerController playerController;
    private PlayerDashController dashController;
    private PlayerDeathController deathController;
    private PlayerAnimationController animController;
    private Coroutine takeDamageCoroutine;

    private Rigidbody2D playerRB;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        #region Script, Component and Variable Suscriptions

        var controller = PlayerController.Instance;
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
        }

        playerController = PlayerController.Instance;
        animController = PlayerController.Instance.animController;
        deathController = PlayerController.Instance.deathController;
        dashController = PlayerController.Instance.dashController;

        playerRB = GetComponent<Rigidbody2D>();
        #endregion
    }

    private void Update()
    {
        nextDamage = Mathf.Max(0, nextDamage - Time.deltaTime);

        animController.SetBool("isTakingDamage", isTakingDamage);
    }
    #endregion

    #region Knockback System
    public void Knockback(Vector2 knockbackVector, float knockbackForce, float knockbackDuration)
    {
        playerRB.bodyType = RigidbodyType2D.Dynamic;

        playerRB.gravityScale = 1;

        playerRB.AddForce(knockbackVector * knockbackForce, ForceMode2D.Impulse);
        playerRB.linearVelocity = (new Vector2(knockbackVector.x, knockbackVector.y + 1) * knockbackForce);
        isKnockedBack = true;

        currentKnockbackDuration = knockbackDuration;

        playerController.stateMachine.ChangeState(playerController.knockbackState);
    }

    public void EndKnockback()
    {
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

        for (int i = 0; i < playerSprite.Length; i++)
        {
            if (playerSprite[i] != null) playerSprite[i].color = Color.red;
        }

        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < playerSprite.Length; i++)
        {
            if (playerSprite[i] != null) playerSprite[i].color = Color.white;
        }

        isTakingDamage = false;

        if (playerHealth <= 0)
        {
            deathController.Die();
        }
    }
    #endregion
}