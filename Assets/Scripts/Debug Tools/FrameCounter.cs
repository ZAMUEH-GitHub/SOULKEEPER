using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.UIElements;

public class FPScounter : MonoBehaviour
{

    public float fpsText;
    private float hudRefresh;
    public float timer;

    void Update()
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            fpsText = fps;
            timer = Time.unscaledTime + hudRefresh;

        }
    }
}