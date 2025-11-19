using System.Collections;
using UnityEngine;

public class BaseScarfController : MonoBehaviour
{
    [Header("States")]
    public bool isIdle;
    public bool isAttacking;

    [Header("Scarf Settings")]
    public int segmentAmount = 15;
    public float segmentDistance = 0.15f;
    public float maxDistance = 0.05f;
    public Vector3 segment0;

    [Header("Speed Settings")]
    public float speedTransition = 25f;
    public float maxSpeed = 0.01f;
    public float minSpeed = 0.15f;
    private float smoothSpeed;

    private Vector3 attackPosition;
    private Vector3[] segmentPoses;
    private Vector3[] segmentV;

    [Header("References")]
    public Transform targetDirection;
    public PlayerController playerController;
    public LineRenderer lineRend;
    public Transform scarfObjective;

    private bool initialized;

    private void Start()
    {
        InitializeScarf();
    }

    private void InitializeScarf()
    {
        lineRend.useWorldSpace = true;
        lineRend.positionCount = segmentAmount;

        segmentPoses = new Vector3[segmentAmount];
        segmentV = new Vector3[segmentAmount];

        Vector3 startPos = targetDirection.position;
        for (int i = 0; i < segmentAmount; i++)
        {
            segmentPoses[i] = startPos;
        }

        lineRend.SetPositions(segmentPoses);

        isIdle = true;
        isAttacking = false;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
            InitializeScarf();

        if (Vector3.Distance(segmentPoses[0], targetDirection.position) > 2f)
            InitializeScarf();

        segmentPoses[0] = targetDirection.position;
        segment0 = segmentPoses[0];

        if (isAttacking)
            ScarfAttack();
        else if (isIdle)
            ScarfIdle();

        lineRend.SetPositions(segmentPoses);
    }

    public void SetAttackInput(bool attackInput, Vector2 moveVector)
    {
        if (!isAttacking)
        {
            isIdle = false;
            isAttacking = true;
            StartCoroutine(EndAttack());
        }
    }

    private void ScarfIdle()
    {
        Flip();

        segmentPoses[0] = targetDirection.position;

        float targetSmoothSpeed = (Vector3.Distance(segmentPoses[1], segmentPoses[0]) > segmentDistance + maxDistance)
            ? maxSpeed
            : minSpeed;

        smoothSpeed = Mathf.Lerp(smoothSpeed, targetSmoothSpeed, Time.deltaTime * speedTransition);

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            Vector3 targetPos = segmentPoses[i - 1] + (targetDirection.right * segmentDistance);
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentV[i], smoothSpeed);
        }
    }

    private void ScarfAttack()
    {
        attackPosition = scarfObjective.transform.position;
        segmentPoses[segmentPoses.Length - 1] = attackPosition;

        for (int i = segmentPoses.Length - 2; i >= 0; i--)
        {
            Vector3 targetPos = segmentPoses[i + 1] - (targetDirection.right * segmentDistance);
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentV[i], smoothSpeed);
        }
    }

    private void Flip()
    {
        if (playerController.moveVector.x > 0)
            transform.localScale = new Vector2(1, 1);
        else if (playerController.moveVector.x < 0)
            transform.localScale = new Vector2(-1, 1);
    }

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        isIdle = true;
    }
}
