using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAttachmentController : MonoBehaviour
{
    [SerializeField] private bool attached;
    private Vector2 lastPlatformPos;

    private Rigidbody2D rb;
    private Rigidbody2D platformRb;
    private Transform platformTf;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public void AttachToPlatform(Collider2D col)
    {
        platformRb = col.attachedRigidbody;
        platformTf = col.transform;
        lastPlatformPos = platformTf.position;
        attached = true;
    }

    public void DetachFromPlatform()
    {
        attached = false;
        platformRb = null;
        platformTf = null;
    }

    private void FixedUpdate()
    {
        if (!attached || platformTf == null) return;

        Vector2 platformPos = platformTf.position;
        Vector2 delta = platformPos - lastPlatformPos;

        rb.position += delta;
        lastPlatformPos = platformPos;
    }
}
