using UnityEngine;

public class MouseParallaxTarget : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float sensitivity = 1.5f;
    public float maxOffset = 2f;
    public float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Vector2 offset = (mousePos - screenCenter) / screenCenter;
        offset = Vector2.ClampMagnitude(offset, 1f);

        targetPosition = new Vector3(offset.x, offset.y, 0f) * maxOffset * sensitivity;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, smoothTime);
    }
}