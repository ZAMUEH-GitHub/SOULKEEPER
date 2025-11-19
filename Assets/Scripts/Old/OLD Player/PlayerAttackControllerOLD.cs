/*using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackControllerOLD : MonoBehaviour
{
    [Header("AttackSettings")]
    public int playerDamage;
    public bool isAttacking;
    [Space(5f)]
    public float attackRate;
    private float nextAttack;
    [Space(5f)]
    public float attackRangeTime;
    private float attackTimer;

    public enum AttackState { Attack1, Attack2, Attack3 }
    public AttackState currentAttackState = AttackState.Attack1;

    [Header("Knockback Settings")]
    public Vector2 knockbackVector;
    public float knockbackForce;
    public float knockbackDuration;

    [Header("Particle Settings")]
    public ParticleSystem playerAttackParticles;
    public Vector2 attackOrientation;
    public Quaternion particleRotation;

    public GameObject player;
    public PlayerController playerController;
    public PlayerPowerUpController powerUpController;
    public Animator playerAnimator;

    void Update()
    {
        if (attackTimer == 0)
        {
            currentAttackState = AttackState.Attack1;
            isAttacking = false;
        }

        nextAttack = Mathf.Max(0, nextAttack - Time.deltaTime);
        attackTimer = Mathf.Max(0, attackTimer - Time.deltaTime);
        knockbackDuration = playerController.knockbackDuration;
    }

    public void PlayerInputAttack(InputAction.CallbackContext context)
    {

        if (!playerController.isDashing && !playerController.isWallSliding)
        {
            if (context.performed && playerController.playerOrientation.y == 0)
            {
                attackOrientation = (player.transform.localScale.x > 0) ? Vector2.right : Vector2.left;
                isAttacking = true;
                PlayerSideAttack();
            }   

            if (context.performed && playerController.playerOrientation.y > 0)
            {
                attackOrientation = Vector2.up;
                isAttacking = true;
                PlayerUpAttack();
            }

            if (context.performed && playerController.playerOrientation.y < 0 && !playerController.isGrounded)
            {
                attackOrientation = Vector2.down;
                isAttacking = true;
                PlayerDownAttack();
            }
        }
    }

    private void PlayerSideAttack()
    {
        if (currentAttackState == AttackState.Attack1 && nextAttack == 0)
        {
            SideAttack1();
        }

        if (currentAttackState == AttackState.Attack2 && attackTimer > 0 && nextAttack == 0)
        {
            SideAttack2();
        }

        if (currentAttackState == AttackState.Attack3 && attackTimer > 0 && nextAttack == 0)
        {
            SideAttack3();
        }
    }

    private void SideAttack1()
    {
        playerAnimator.SetTrigger("PlayerSideAttack1");
        
        particleRotation = Quaternion.FromToRotation(Vector2.up, attackOrientation);
        Instantiate(playerAttackParticles, transform.position, particleRotation);

        currentAttackState = AttackState.Attack2;
        attackTimer = attackRate + attackRangeTime;
        nextAttack = attackRate;
    }

    private void SideAttack2()
    {
        playerAnimator.SetTrigger("PlayerSideAttack2");

        particleRotation = Quaternion.FromToRotation(Vector2.up, attackOrientation);
        Instantiate(playerAttackParticles, transform.position, particleRotation);

        currentAttackState = AttackState.Attack3;
        attackTimer = attackRate + attackRangeTime;
        nextAttack = attackRate;
    }

    private void SideAttack3()
    {
        playerAnimator.SetTrigger("PlayerSideAttack3");

        particleRotation = Quaternion.FromToRotation(Vector2.up, attackOrientation);
        Instantiate(playerAttackParticles, transform.position, particleRotation);
        currentAttackState = AttackState.Attack1;
        attackTimer = 0.3f;
        nextAttack = attackRate;
    }

private void PlayerUpAttack()
{
    if (nextAttack == 0)
    {
        playerAnimator.SetTrigger("PlayerUpAttack");

        particleRotation = Quaternion.FromToRotation(Vector2.up, attackOrientation);
        Instantiate(playerAttackParticles, transform.position, particleRotation);
        nextAttack = attackRate;
    }
}

private void PlayerDownAttack()
{
    if (nextAttack == 0)
    {
        playerAnimator.SetTrigger("PlayerDownAttack");

        particleRotation = Quaternion.FromToRotation(Vector2.up, attackOrientation);
        Instantiate(playerAttackParticles, transform.position, particleRotation);
        nextAttack = attackRate;
    }
}
    //skibidi 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IEnemy enemy = collision.GetComponent<IEnemy>();

            if (enemy != null)
            {
                knockbackVector = (collision.transform.position - player.transform.position).normalized;

                playerController.PlayerKnockback(-knockbackVector, knockbackForce / 3, knockbackDuration);

                if (player.transform.position.y > collision.transform.position.y + 1)
                {
                    playerController.PlayerKnockback(-knockbackVector, knockbackForce * 2, knockbackDuration);
                }

                enemy.EnemyKnockback(knockbackVector, knockbackForce);
                StartCoroutine(enemy.TakeDamage(playerDamage, knockbackVector));
            }
        }

        if (collision.CompareTag("Spike"))
        {
            knockbackVector = (player.transform.position - collision.transform.position).normalized;
            playerController.PlayerKnockback(knockbackVector, knockbackForce, knockbackDuration);
            
            if (player.transform.position.y > collision.transform.position.y + 1)
            {
                playerController.PlayerKnockback(knockbackVector, knockbackForce * 2, knockbackDuration);
            }
        }

        if (collision.CompareTag("Bulb"))
        {
            knockbackVector = (collision.transform.position - player.transform.position).normalized;
            playerController.PlayerKnockback(-knockbackVector, knockbackForce, knockbackDuration);
            playerAnimator.SetTrigger("PlayerJump");

            if (player.transform.position.y > collision.transform.position.y + 1)
            {
                playerController.PlayerKnockback(-knockbackVector, knockbackForce * 3, knockbackDuration);
            }
        }

        if (collision.CompareTag("Minos Take Damage"))
        {
            MinosController minos = collision.GetComponentInParent<MinosController>();
            
            if (minos != null) 
            { 
                knockbackVector = (collision.transform.position - player.transform.position).normalized;
                playerController.PlayerKnockback(-knockbackVector, knockbackForce / 3, knockbackDuration);
            
                if (player.transform.position.y > collision.transform.position.y + 1)
                {
                    playerController.PlayerKnockback(-knockbackVector, knockbackForce * 2, knockbackDuration);
                }

                StartCoroutine(minos.TakeDamage(playerDamage));
            }
        }
    }
}*/