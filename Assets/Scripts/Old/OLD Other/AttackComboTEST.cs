using UnityEngine;
using UnityEngine.InputSystem;

public class AttackComboTEST : MonoBehaviour
{
    public float attackRate = 0.05f;
    public float nextAttack = 0.0f;

    public float attackRangeTime = 0.5f;
    public float attackTimer = 0.0f;
    public float attackStep = 0.0f;

    public void TESTAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlayerSideAttack();
        }


    }

    public void PlayerSideAttack()
    {
        if (attackStep == 0 && nextAttack == 0)
        {
            Attack1();
        }

        if (attackStep == 1 && attackTimer > 0 && nextAttack == 0)
        {
            Attack2();
        }

        if (attackStep == 2 && attackTimer > 0 && nextAttack == 0)
        {
            Attack3();
        }
    }

    public void Attack1()
    {
        Debug.Log("ATTACK 1");
        attackStep = 1;
        attackTimer = attackRangeTime;
        nextAttack = attackRate;
    }
    public void Attack2()
    {
        Debug.Log("ATTACK 2");
        attackStep = 2;
        attackTimer = attackRangeTime;
        nextAttack = attackRate;
    }

    public void Attack3()
    {
        Debug.Log("ATTACK 3");
        attackStep = 0;
        nextAttack = attackRate;
    }

    void Update()
    {
        if (attackTimer == 0)
        {
            attackStep = 0;
        }

        nextAttack = Mathf.Max(0, nextAttack - Time.deltaTime);
        attackTimer = Mathf.Max(0, attackTimer - Time.deltaTime);
    }
}
