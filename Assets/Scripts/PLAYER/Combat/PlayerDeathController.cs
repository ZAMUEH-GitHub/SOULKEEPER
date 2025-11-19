using System.Collections;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;

    [Header("Death Particles and Souls")]
    public ParticleSystem deathParticles;
    public GameObject soulObject;

    private const string CORPSE_LAYER = "Corpse";

    private PlayerController playerController;

    private void Awake()
    {
        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
            playerController = controller;
        }
    }

    public void Die()
    {
        playerController.isAlive = false;
        
        Instantiate(deathParticles, transform.position, Quaternion.identity);

        for (int i = playerStats.score / 2; i > 0; i--)
        {
            GameObject soul = Instantiate(soulObject, transform.position, Quaternion.identity);
            soul.transform.position = new Vector2(soul.transform.position.x + Random.Range(-2f, 2f), soul.transform.position.y + Random.Range(-1.5f, 2f));
        }

        if (LayerMask.NameToLayer(CORPSE_LAYER) != -1)
            gameObject.layer = LayerMask.NameToLayer(CORPSE_LAYER);

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return null;
        // (¿) GameSceneManager.Instance.LoadSceneFromCheckpoint(); (?)
    }
}
