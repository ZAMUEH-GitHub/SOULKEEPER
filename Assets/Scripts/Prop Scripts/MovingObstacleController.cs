using UnityEngine;

public class MovingObstacleController : MonoBehaviour
{
    [Header("Moving Obstacle Attributes")]
    public float movingObstacleSpeed = 1f;
    public float movingObstacleLimit = 3f;

    [Header("Moving Obstacle Direction")]
    [Range(-1, 1)] public int vectorUpDown;
    [Range(-1, 1)] public int vectorLeftRight;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Vector2 movementDirection;

    void Start()
    {
        startPosition = transform.position;
        movementDirection = new Vector2(vectorLeftRight, vectorUpDown).normalized;
        targetPosition = startPosition + movementDirection * movingObstacleLimit;
    }

    void Update()
    {
        // Use a sine wave to ease in and out smoothly (sin wave goes from 0 to 1 and back)
        float t = (Mathf.Sin(Time.time * movingObstacleSpeed * Mathf.PI * 2 - Mathf.PI / 2) + 1f) / 2f;
        
        // Lerp smoothly between start and target positions using t
        Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, t);
        transform.position = newPosition;
    }
}
