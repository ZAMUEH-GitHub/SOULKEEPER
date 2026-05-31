using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    private PlayerStatsSO playerStats;

    [Header("Death Particles and Souls")]
    public ParticleSystem deathParticles;
    public AudioClip deathSound;
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
            playerController.stateMachine.ChangeState(playerController.deathState);
        }

        int corpseLayer = LayerMask.NameToLayer("Corpse");
        if (corpseLayer != -1)
        {
            gameObject.layer = corpseLayer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = corpseLayer;
            }
        }
    }

    public void TriggerDeathEffects()
    {
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (soulObject != null)
        {
            for (int i = playerStats.score / 2; i > 0; i--)
            {
                GameObject soul = Instantiate(soulObject, transform.position, Quaternion.identity);
                soul.transform.position = new Vector2(soul.transform.position.x + Random.Range(-2f, 2f), soul.transform.position.y + Random.Range(-1.5f, 2f));
            }
        }

        ExecuteDeathSequence();
    }

    private async void ExecuteDeathSequence()
    {
        await Task.Delay(500);

        int slotIndex = 1;
        if (SaveSlotManager.Instance != null)
            slotIndex = SaveSlotManager.Instance.ActiveSlotIndex;

        if (SessionManager.Instance != null && SaveSystem.SaveExists(slotIndex))
        {
            await SaveSystem.LoadAsync(slotIndex, SessionManager.Instance.RuntimeStats);
        }

        if (SaveSystem.HasValidPlayerPosition)
        {
            transform.position = SaveSystem.LastLoadedPlayerPosition;
        }

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

        playerController.animController.SetBool("isDead", false);

        if (colliders != null)
        {
            foreach (var col in colliders)
                col.enabled = true;
        }

        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer != -1)
        {
            gameObject.layer = playerLayer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = playerLayer;
            }
        }

        playerController.stateMachine.ChangeState(playerController.respawnState);
    }
}