using UnityEngine;

public class SoulCollectible : MonoBehaviour
{
    public int collectibleScore;
    public float initialSoulSpeed;
    public float acceleration;
    public float smoothTime = 0.3f;
    private GameObject playerObject;
    private GameManager gameManager;
    private float currentSpeed;
    private Vector2 velocity = Vector2.zero;

    void Start()
    {
        gameManager = GameObject.Find("GAME MANAGER").GetComponent<GameManager>();
        playerObject = GameObject.Find("PLAYER");
        currentSpeed = initialSoulSpeed;
    }

    private void Update()
    {
        Vector2 targetPosition = playerObject.transform.position;

        transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, currentSpeed, Time.deltaTime);

        currentSpeed += acceleration * Time.deltaTime;
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
