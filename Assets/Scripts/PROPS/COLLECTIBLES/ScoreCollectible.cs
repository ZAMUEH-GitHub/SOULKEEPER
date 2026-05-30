using UnityEngine;

public class ScoreCollectible : MonoBehaviour
{
    [Header("Score Collectible Stats")]
    [SerializeField] private int scoreAmount;

    private Animator scoreAnim;

    private void Start()
    {
        scoreAnim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
                player.playerRuntimeStats.score += scoreAmount;

            scoreAnim.SetTrigger("collected");
        }
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}