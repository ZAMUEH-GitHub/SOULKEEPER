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
    public enum BossPhase { Phase0, Phase1, Phase2, Phase3 };
    public BossPhase currentBossPhase = BossPhase.Phase0;


    private Animator bossAnimator;
    private GameObject[] enemyBodyParts;
    private SpriteRenderer[] bossSprites;
    private Phase1Controller phase1Controller;
    private Phase2Controller phase2Controller;

    private float[] currentLayerWeights = new float[5];
    private float lerpSpeed = 2f;

    void Start()
    {
        isAlive = true;

        bossAnimator = GetComponent<Animator>();
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
        switch (currentBossPhase)
        {
            case BossPhase.Phase0:
                SmoothSetLayerWeights(0, 1, 0, 0, 0);
                break;
            case BossPhase.Phase1:
                phase1Controller.RunPhase1();
                SmoothSetLayerWeights(0, 0, 1, 0, 0);
                break;
            case BossPhase.Phase2:
                phase2Controller.RunPhase2();
                SmoothSetLayerWeights(0, 0, 0, 1, 0);
                break;
            case BossPhase.Phase3:
                // phase3Controller.RunPhase3();
                SmoothSetLayerWeights(0, 0, 0, 0, 1);
                break;
        }

        ApplyLayerWeights();
        damageTimer = Mathf.Max(0, damageTimer - Time.deltaTime);
    }

    private void SmoothSetLayerWeights(float target0, float target1, float target2, float target3, float target4)
    {
        float[] targets = new float[] { target0, target1, target2, target3, target4 };
        for (int i = 0; i < currentLayerWeights.Length; i++)
        {
            currentLayerWeights[i] = Mathf.Lerp(currentLayerWeights[i], targets[i], Time.deltaTime * lerpSpeed);
        }
    }

    private void ApplyLayerWeights()
    {
        for (int i = 0; i < currentLayerWeights.Length; i++)
        {
            bossAnimator.SetLayerWeight(i, currentLayerWeights[i]);
        }
    }

    public void AdvancePhase()
    {
        if (currentBossPhase == BossPhase.Phase0)
        {
            currentBossPhase = BossPhase.Phase1;
            Debug.Log("BOSS PHASE 1!!");
        }
        else if (currentBossPhase == BossPhase.Phase1)
        {
            currentBossPhase = BossPhase.Phase2;
            Debug.Log("BOSS PHASE 2!!");
        }
        else if (currentBossPhase == BossPhase.Phase2)
        {
            currentBossPhase = BossPhase.Phase3;
            Debug.Log("BOSS PHASE 3!!");
        }
    }

    public IEnumerator TakeDamage(int damage)
    {
        if (damageTimer <= 0)
        {
            bossHealth -= damage;
            isTakingDamage = true;
            foreach (var renderer in bossSprites)
                renderer.color = Color.red;

            if (bossHealth <= 50 && currentBossPhase == BossPhase.Phase2)
            {
                AdvancePhase();
            }

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
    }
}
