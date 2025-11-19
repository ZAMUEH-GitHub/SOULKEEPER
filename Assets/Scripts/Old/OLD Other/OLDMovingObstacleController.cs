using UnityEngine;

public class OLDMovingObstacleController : MonoBehaviour
{
    public float obstacleSpeed;

    public float xLimit;
    private float xPositiveLimit;
    private float xNegativeLimit;

    [HideInInspector] public Vector2 obstacleOrientation;

    void Start()
    {
        obstacleOrientation = Vector2.right;

        xPositiveLimit = transform.position.x + xLimit;
        xNegativeLimit = transform.position.x - xLimit;
    }


    void Update()
    {
        transform.Translate(obstacleOrientation * obstacleSpeed * Time.deltaTime);

        if (transform.position.x > xPositiveLimit)
        {
            obstacleOrientation = Vector2.left;
        }
        if (transform.position.x < xNegativeLimit)
        {
            obstacleOrientation = Vector2.right;
        }
    }
}
