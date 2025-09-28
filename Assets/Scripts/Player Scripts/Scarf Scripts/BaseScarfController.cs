using UnityEngine;

public class BaseScarfController : MonoBehaviour
{
    [Header("Scarf Settings")]
    public int segmentAmount;
    public float segmentDistance;
    public float maxDistance;

    [Header("Speed Settings")]
    public float speedTransition;
    public float maxSpeed;
    public float minSpeed;
    private float smoothSpeed;

    private Vector3[] segmentPoses;
    private Vector3[] segmentV;

    public Transform targetDirection;
    public PlayerController playerController;
    public LineRenderer lineRend;
    private void Start()
    {
        transform.position = targetDirection.position;
        lineRend.positionCount = segmentAmount;
        segmentPoses = new Vector3[segmentAmount];
        segmentV = new Vector3[segmentAmount];
    }

    private void Update()
    {
        segmentPoses[0] = targetDirection.position;
        Flip();

        float targetSmoothSpeed = (Vector3.Distance(segmentPoses[1], segmentPoses[0]) > segmentDistance + maxDistance) ? maxSpeed : minSpeed;

        smoothSpeed = Mathf.Lerp(smoothSpeed, targetSmoothSpeed, Time.deltaTime * speedTransition); 

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + targetDirection.right * segmentDistance, ref segmentV[i], smoothSpeed);
        }

        lineRend.SetPositions(segmentPoses);
    }

    private void Flip()
    {
        if (playerController.moveVector.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
        if (playerController.moveVector.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }
}
