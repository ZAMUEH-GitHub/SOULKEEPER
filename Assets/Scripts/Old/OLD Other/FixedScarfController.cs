/*using UnityEngine;

public class FixedScarfController : MonoBehaviour
{
    public int length;
    public LineRenderer lineRend;
    public Vector3[] segmentPoses;
    private Vector3[] segmentV;

    public Transform targetDirection;
    public Transform wiggleDirection;

    public float targetDistance;
    public float smoothSpeed;

    public float wiggleSpeed;
    public float wiggleMagnitude;

    public PlayerController playerController;

    private void Start()
    {
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
    }

    private void Update()
    {
        segmentPoses[0] = targetDirection.position;

        Flip();

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            Vector3 targetPos = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i - 1]).normalized * targetDistance;
            segmentPoses[i] = Vector3.SmoothDamp((segmentPoses[i]), targetPos, ref segmentV[i], smoothSpeed);
        }
        lineRend.SetPositions(segmentPoses);
    }

    private void Flip()
    {
        if (playerController.playerOrientation.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
        if (playerController.playerOrientation.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }
}
*/