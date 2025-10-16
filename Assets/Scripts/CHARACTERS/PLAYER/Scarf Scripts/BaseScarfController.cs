using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class BaseScarfController : MonoBehaviour
{
    [Header("States")]
    public bool isIdle;
    public bool isAttacking; 
    public int facing; 
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

    [Header("Vectors")]
    private Vector3 attackPosition; 
    private Vector3[] segmentPoses;
    private Vector3[] segmentV;
    private Vector2 playerOrientation;

    [Header ("Objects")]
    public Transform targetDirection;
    public PlayerController playerController;
    public LineRenderer lineRend;
    private PlayerJumpController jumpController;
    public Transform scarfObjective;
    public Transform leftScarfObjective; 
    private void Start()
    {
        transform.position = targetDirection.position;
        lineRend.positionCount = segmentAmount;
        segmentPoses = new Vector3[segmentAmount];
        segmentV = new Vector3[segmentAmount];
        isAttacking = false;
        isIdle = true; 

        jumpController = GetComponentInParent<PlayerJumpController>();
        
    }

    private void Update()
    {
        segmentPoses[0] = targetDirection.position;
        segment0 = segmentPoses[0];

        if (isAttacking == true)
        {
           
            if (playerOrientation.y == 0 && facing == 1)
            {
                attackPosition = scarfObjective.transform.position;
            }
            else if (playerOrientation.y == 0 && facing == -1)
            {
                attackPosition = leftScarfObjective.transform.position;
            }


            else if (playerOrientation.y > 0)
            {
                attackPosition = scarfObjective.position;
            }
            else if (playerOrientation.y < 0 /*&& !jumpController.isGrounded*/)
            {
                attackPosition = scarfObjective.position;
            }


                segmentPoses[segmentPoses.Length - 1] = attackPosition;
            for (int i = segmentPoses.Length - 2; i >= 0; i--)
            {
                Vector3 targetPos = segmentPoses[i + 1] - (targetDirection.right * segmentDistance);
                segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentV[i], smoothSpeed);

                /* segmentPoses[segmentPoses.Length - 1] = Vector3.SmoothDamp(segmentPoses[segmentPoses.Length - 1], attackPosition, ref segmentV[segmentPoses.Length - 1],smoothSpeed);*/
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
    public void ScarfAttack(bool attackInput, Vector2 moveVector)
    {
        if (attackInput == true)
        {
            playerOrientation = moveVector; 
            isIdle = false;
            isAttacking = true;
            StartCoroutine(ScarfCooldown());
        }
    }
    public IEnumerator ScarfCooldown()
    {
        lineRend.SetPositions(segmentPoses);
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        isIdle = true;
    }

    private void Flip()
    {
        if (playerController.moveVector.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
            facing = 1; 
        }
        if (playerController.moveVector.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
            facing = -1;
        }
    }
}
