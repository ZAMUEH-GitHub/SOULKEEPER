using System.Collections;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;

    [Header("Death Particles and Souls")]
    public ParticleSystem deathParticles;
    public GameObject soulObject;

    private Collider2D[] colliders;
    private bool isDead;
    private PlayerController playerController;

    private void Awake()
    {
        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
            playerController = controller;
        }

        colliders = GetComponents<Collider2D>();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        playerController.isAlive = false;

        if (playerController != null)
        {
            playerController.FreezeAllInputs();
        }
        /*
        if (colliders != null)
        {
            foreach (var col in colliders)
                col.enabled = false;
        }
        */
        Instantiate(deathParticles, transform.position, Quaternion.identity);

        for (int i = playerStats.score / 2; i > 0; i--)
        {
            GameObject soul = Instantiate(soulObject, transform.position, Quaternion.identity);
            soul.transform.position = new Vector2(soul.transform.position.x + Random.Range(-2f, 2f), soul.transform.position.y + Random.Range(-1.5f, 2f));
        }

        int corpseLayer = LayerMask.NameToLayer("Corpse");
        if (corpseLayer != -1)
            gameObject.layer = corpseLayer;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        int slotIndex = 1;
        if (SaveSlotManager.Instance != null)
            slotIndex = SaveSlotManager.Instance.ActiveSlotIndex;

        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadSceneFromCheckpointSlot(slotIndex);
        }
        else
        {
            Debug.LogError("[PlayerDeathController] GameSceneManager.Instance not found!");
        }
    }

    public void ResetAfterRespawn()
    {
        isDead = false;
        playerController.isAlive = true;

        if (colliders != null)
        {
            foreach (var col in colliders)
                col.enabled = true;
        }

        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer != -1)
            gameObject.layer = playerLayer;
    }
}
