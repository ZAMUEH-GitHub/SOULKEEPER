using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DistanceJoint2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class SpiderEnemyController : MonoBehaviour, IDamageable, IKnockbackable
{
    [SerializeField] private HazardState currentState = HazardState.Idle;
   
    [Header("Web Settings")]
    [SerializeField] private Transform ceilingAnchor;
    [SerializeField] private float dropDistance = 5f;
    [SerializeField] private float dropSpeed = 10f;
    [SerializeField] private float retractSpeed = 2f;

    [Header("Detection")]
    [SerializeField] private float detectionDistanceX = 2f;
    [SerializeField] private float detectionDistanceY = 7f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Health & Death")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private ParticleSystem damageParticles;
    [SerializeField] private SpriteRenderer[] enemySprites;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private AudioClip deathSound;

    private int currentHealth;
    private Animator anim;
    private const string CORPSE_LAYER = "Corpse";

    #region Cached Variables & References
    private DistanceJoint2D distanceJoint;
    private LineRenderer lineRenderer;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private Vector2 originalPosition;
    private float originalJointDistance;
    private float originalGravity;
    private float currentSlack = 0f;
    private bool isTriggered = false;

    private Coroutine takeDamageCoroutine;
    private bool isTakingDamage;

    private AnimationEventRelay animRelay;
    #endregion

    private enum HazardState { Idle, Dropping, Attacking, Retracting, Dead }

    private void Awake()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        enemySprites = GetComponentsInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        originalPosition = transform.position;
        originalJointDistance = distanceJoint.distance;
        originalGravity = rb.gravityScale;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentState == HazardState.Dead) return;

        UpdateWebVisuals();

        switch (currentState)
        {
            case HazardState.Idle:
                DetectPlayer();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (currentState == HazardState.Dead) return;

        if (currentState == HazardState.Dropping)
        {
            HandleDropping();
        }
        else if (currentState == HazardState.Retracting)
        {
            HandleRetracting();
        }
    }

    private void DetectPlayer()
    {
        Vector2 boxCenter = originalPosition - new Vector2(0, detectionDistanceY / 2);
        Vector2 boxSize = new Vector2(detectionDistanceX * 2, detectionDistanceY);

        Collider2D player = Physics2D.OverlapBox(boxCenter, boxSize, 0f, playerLayer);

        if (player != null && !isTriggered)
        {
            isTriggered = true;

            distanceJoint.enabled = false;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;

            if (anim != null) anim.SetBool("IsDropping", true);

            currentState = HazardState.Dropping;
        }
    }

    private void HandleDropping()
    {
        currentSlack = Mathf.MoveTowards(currentSlack, dropDistance, dropSpeed * Time.fixedDeltaTime);

        Vector2 targetPosition = originalPosition - new Vector2(0, currentSlack);
        rb.MovePosition(targetPosition);

        if (currentSlack >= dropDistance)
        {
            currentState = HazardState.Attacking;
            RestorePhysicsJoint();

            if (anim != null)
            {
                anim.SetBool("IsDropping", false);
                anim.SetBool("IsAttacking", true);
            }
        }
    }

    public void EndAttack()
    {
        if (currentState == HazardState.Dead) return;

        currentState = HazardState.Retracting;
        if (anim != null)
        {
            anim.SetBool("IsAttacking", false);
            anim.SetBool("IsRetracting", true);
        }
    }

    private void HandleRetracting()
    {
        RestorePhysicsJoint();
        rb.WakeUp();

        currentSlack = Mathf.MoveTowards(currentSlack, 0f, retractSpeed * Time.fixedDeltaTime);
        distanceJoint.distance = originalJointDistance + currentSlack;

        if (currentSlack <= 0f)
        {
            currentSlack = 0f;
            distanceJoint.distance = originalJointDistance;

            transform.position = originalPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            isTriggered = false;
            currentState = HazardState.Idle;

            if (anim != null) anim.SetBool("IsRetracting", false);
        }
    }

    private void RestorePhysicsJoint()
    {
        if (!distanceJoint.enabled)
        {
            distanceJoint.enabled = true;
            rb.gravityScale = originalGravity;
            distanceJoint.distance = originalJointDistance + currentSlack;
        }
    }

    private void UpdateWebVisuals()
    {
        if (ceilingAnchor != null)
        {
            lineRenderer.SetPosition(0, ceilingAnchor.position);
            lineRenderer.SetPosition(1, transform.position);
        }
    }

    public void TakeDamage(int damage, Vector2 damageVector)
    {
        if (currentState == HazardState.Dead) return;

        if (damageParticles != null)
        {
            Quaternion particleRotation = Quaternion.FromToRotation(Vector2.up, damageVector);
            Instantiate(damageParticles, transform.position, particleRotation);
        }

        if (!isTakingDamage)
        {
            if (takeDamageCoroutine != null) StopCoroutine(takeDamageCoroutine);
            takeDamageCoroutine = StartCoroutine(ExecuteTakeDamage(damage));
        }
    }

    private IEnumerator ExecuteTakeDamage(int damage)
    {
        isTakingDamage = true;
        currentHealth -= damage;

        if (anim != null) anim.SetTrigger("Hit");

        foreach (SpriteRenderer sr in enemySprites) sr.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        foreach (SpriteRenderer sr in enemySprites) sr.color = Color.white;

        isTakingDamage = false;

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        currentState = HazardState.Dead;
        StopAllCoroutines();

        if (LayerMask.NameToLayer(CORPSE_LAYER) != -1)
            gameObject.layer = LayerMask.NameToLayer(CORPSE_LAYER);
        
        if (anim != null) anim.SetBool("IsDropping", false);
        if (anim != null) anim.SetBool("IsRetracting", false);
        if (anim != null) anim.SetBool("IsAttacking", false);
        if (anim != null) anim.SetBool("IsDead", true);

        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        rb.gravityScale = originalGravity;
    }

    public void EndDeath()
    {
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        foreach (SpriteRenderer sr in enemySprites)
        {
            sr.enabled = false;
        }

        Destroy(gameObject, 5f);
    }

    public void Knockback(Vector2 knockbackVector, float knockbackForce, float knockbackDuration)
    {
        if (currentState == HazardState.Dead) return;

        if (currentState == HazardState.Dropping)
        {
            RestorePhysicsJoint();
            currentState = HazardState.Attacking;

            if (anim != null)
            {
                anim.SetBool("IsDropping", false);
                anim.SetBool("IsAttacking", true);
            }
        }

        rb.AddForce(knockbackVector.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 startPos = Application.isPlaying ? originalPosition : (Vector2)transform.position;

        Vector2 endPos = startPos - new Vector2(0, dropDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPos, endPos);

        Gizmos.color = Color.red;
        Vector2 boxCenter = startPos - new Vector2(0, detectionDistanceY / 2);
        Vector2 boxSize = new Vector2(detectionDistanceX * 2, detectionDistanceY);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}