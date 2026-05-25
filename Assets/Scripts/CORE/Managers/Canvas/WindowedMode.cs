using UnityEngine;

public class WindowedMode : MonoBehaviour
{
    public void FullScreen()
    {
        Debug.Log("fullscreen0");
       // Screen.SetResolution(1920, 1080, true);

    }
    public void Windowed()
    {
        Debug.Log("Windowed");
       // Screen.SetResolution(810, 540, false);

    }
}
