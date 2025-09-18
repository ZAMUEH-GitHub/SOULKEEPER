using UnityEngine;

public class ParentingColliderController : MonoBehaviour
{
    public GameObject chandelierObject;

    void Update()
    {
        transform.position = chandelierObject.transform.position;
    }
}
