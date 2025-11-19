using UnityEngine;

public class Chandelier : MonoBehaviour
{
    private new HingeJoint2D hingeJoint;
    public float returnSpeed = 10f;
    public float maxReturnTorque = 100f;

    private float initialRotation; 

    void Start()
    {
        if (hingeJoint == null)
        {
            hingeJoint = GetComponent<HingeJoint2D>();
        }

        initialRotation = transform.rotation.eulerAngles.z;
    }

    void Update()
    {
        // Calculate the Difference between Current Rotation and Initial rotation
        float rotationDifference = transform.rotation.eulerAngles.z - initialRotation;

        // Normalize the rotation difference to handle both directions (-180 to 180 degrees)
        if (rotationDifference > 180f)
            rotationDifference -= 360f;
        else if (rotationDifference < -180f)
            rotationDifference += 360f;

        // Apply torque to gradually bring it back to the starting position
        float returnTorque = Mathf.Sign(-rotationDifference) * Mathf.Min(Mathf.Abs(rotationDifference) * returnSpeed, maxReturnTorque);

        // Apply the return torque to the RigidBody2D
        hingeJoint.attachedRigidbody.AddTorque(returnTorque);
    }
}