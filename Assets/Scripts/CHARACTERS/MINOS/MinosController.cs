using UnityEngine;
using System.Collections;

public class MinosController : MonoBehaviour
{
    [Header("General Boss Stats")]
    public int bossHealth;
    public bool isAlive;
    public bool isTakingDamage;
    public float damageRate;
    private float damageTimer = 0;

    public enum BossPhase { Phase0, Phase1, Phase2 };
    public BossPhase currentBossPhase = BossPhase.Phase0;

    private Animator bossAnimator;
    private GameObject[] enemyBodyParts;
    private SpriteRenderer[] bossSprites;
    private Phase1Controller phase1Controller;
    private Phase2Controller phase2Controller;

    void Start()
    {
        isAlive = true;

        bossAnimator = GetComponentInChildren<Animator>();
        phase1Controller = GetComponent<Phase1Controller>();
        phase2Controller = GetComponent<Phase2Controller>();

        enemyBodyParts = GameObject.FindGameObjectsWithTag("King Minos Body Part");
        bossSprites = new SpriteRenderer[enemyBodyParts.Length];

        for (int i = 0; i < enemyBodyParts.Length; i++)
        {
            bossSprites[i] = enemyBodyParts[i].GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (!isAlive) return;

        switch (currentBossPhase)
        {
            case BossPhase.Phase0:
                break;
            case BossPhase.Phase1:
                phase1Controller.RunPhase1();
                break;
            case BossPhase.Phase2:
                phase2Controller.RunPhase2();
                break;
        }

        damageTimer = Mathf.Max(0, damageTimer - Time.deltaTime);
    }

    public void AdvancePhase()
    {
        if (currentBossPhase == BossPhase.Phase0)
        {
            currentBossPhase = BossPhase.Phase1;
            SessionManager.Instance.UnlockProgressFlag("Minos_Phase1_Started");
            Debug.Log("BOSS PHASE 1!!");
        }
        else if (currentBossPhase == BossPhase.Phase1)
        {
            currentBossPhase = BossPhase.Phase2;
            SessionManager.Instance.UnlockProgressFlag("Minos_Phase2_Started");
            Debug.Log("BOSS PHASE 2!!");
        }
    }

    public IEnumerator TakeDamage(int damage)
    {
        if (currentBossPhase != BossPhase.Phase2) yield break;

        if (damageTimer <= 0)
        {
            bossHealth -= damage;
            isTakingDamage = true;
            foreach (var renderer in bossSprites)
                renderer.color = Color.red;

            yield return new WaitForSeconds(0.25f);
            foreach (var renderer in bossSprites)
                renderer.color = Color.white;

            if (bossHealth <= 0)
            {
                Die();
            }

            isTakingDamage = false;
            damageTimer = damageRate;
        }
    }

    private void Die()
    {
        isAlive = false;
        if (bossAnimator != null) bossAnimator.SetTrigger("Death");

        SessionManager.Instance.UnlockProgressFlag("Minos_Defeated");
    }
}