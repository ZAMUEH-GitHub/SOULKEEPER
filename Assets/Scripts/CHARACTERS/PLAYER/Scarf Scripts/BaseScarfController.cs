using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class BaseScarfController : MonoBehaviour
{
    [Header("States")]
    public bool isIdle;
    public bool isAttacking; 

    [Header("Scarf Settings")]
    public int segmentAmount;
    public float segmentDistance;
    public float maxDistance;
    public Vector3 segment0; 

    [Header("Speed Settings")]
    public float speedTransition;
    public float maxSpeed;
    public float minSpeed;
    private float smoothSpeed;

    private Vector3 attackPosition; 
    private Vector3[] segmentPoses;
    private Vector3[] segmentV;

    [Header ("Objects")]
    public Transform targetDirection;
    public PlayerController playerController;
    public LineRenderer lineRend;
    public Transform scarfObjective;

    private void Start()
    {
        #region onStart

        transform.position = targetDirection.position;
        lineRend.positionCount = segmentAmount;

        segmentPoses = new Vector3[segmentAmount];
        segmentV = new Vector3[segmentAmount];

        isAttacking = false;
        isIdle = true; 
        #endregion
    }

    private void Update()
    {
        segmentPoses[0] = targetDirection.position;
        segment0 = segmentPoses[0];

        if (isAttacking)
        {
            attackPosition = scarfObjective.transform.position;            
            segmentPoses[segmentPoses.Length - 1] = attackPosition;

            for (int i = segmentPoses.Length - 2; i >= 0; i--)
            {
                Vector3 targetPos = segmentPoses[i + 1] - (targetDirection.right * segmentDistance);
                segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentV[i], smoothSpeed);
            }
        }

        if (isIdle)
        {
            ScarfIdle();
        }

        lineRend.SetPositions(segmentPoses);
    }

    private void ScarfIdle()
    {
        Flip();

        segmentPoses[0] = targetDirection.position;
        float targetSmoothSpeed = (Vector3.Distance(segmentPoses[1], segmentPoses[0]) > segmentDistance + maxDistance) ? maxSpeed : minSpeed;
        smoothSpeed = Mathf.Lerp(smoothSpeed, targetSmoothSpeed, Time.deltaTime * speedTransition);

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + targetDirection.right * segmentDistance, ref segmentV[i], smoothSpeed);
        }

        lineRend.SetPositions(segmentPoses);
    }

    public void ScarfAttack(bool attackInput, Vector2 moveVector)
    {
        isIdle = false;
        isAttacking = true;
        StartCoroutine(EndAttack());
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
 
    public IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        isIdle = true;
    }
}
