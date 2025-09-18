using UnityEngine;

public class ScoreCollectible : MonoBehaviour
{
    public int collectibleScore;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GAME MANAGER").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.ScoreUpdater(collectibleScore);
            Destroy(gameObject);
        }
    }
}
