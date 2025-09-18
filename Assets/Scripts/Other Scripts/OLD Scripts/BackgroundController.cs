using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public GameObject mainCamera;
    private float transformY;

    void Start()
    {

    }

    void Update()
    {
        transformY = mainCamera.transform.position.y;
        transform.position = new Vector3(transform.position.x, transformY, transform.position.z);
    }
}
