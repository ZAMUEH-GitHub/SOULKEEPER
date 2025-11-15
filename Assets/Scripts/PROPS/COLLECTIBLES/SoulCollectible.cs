using UnityEngine;

public class SoulCollectible : MonoBehaviour
{
    [Header("Souls Settings")]
    [SerializeField] private int collectibleScore;
    [SerializeField] private float initialSoulSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float smoothTime = 0.3f;

    private GameObject playerObject;
    private float currentSpeed;
    private Vector2 velocity = Vector2.zero;

    void Start()
    {
        playerObject = PlayerController.Instance.gameObject;
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
            Destroy(gameObject);
        }
    }
}
