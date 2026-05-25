using UnityEngine;

using Unity.Cinemachine;
public class CameraShakeController : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shake()
    {
        impulseSource.GenerateImpulse();
    }
}