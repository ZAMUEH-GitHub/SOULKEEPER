using UnityEngine;

public class WiggleController : MonoBehaviour
{
    public float wiggleSpeed;
    public float wiggleMagnitude;

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);
    }
}
