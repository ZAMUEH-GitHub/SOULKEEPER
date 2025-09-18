using UnityEngine;
using Unity.Cinemachine;

public class CinemachineAutoTarget : MonoBehaviour
{
    private CinemachineCamera vCam;

    void Start()
    {
        vCam = GetComponent<CinemachineCamera>();

        if (PlayerController.Instance != null)
        {
            vCam.Follow = PlayerController.Instance.transform;
        }
    }
}
