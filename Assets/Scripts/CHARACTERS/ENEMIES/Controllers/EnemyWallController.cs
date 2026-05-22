using UnityEngine;

public class EnemyWallController : MonoBehaviour
{
    public bool isWalled;

    [Header("Wall Check (OverlapCircle)")]
    public Transform wallCheckPoint;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;

    private void Update()
    {
        isWalled = Physics2D.OverlapCircle(wallCheckPoint.position, wallCheckRadius, wallLayer);
    }
    private void OnDrawGizmosSelected()
    {
        if (wallCheckPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallCheckPoint.position, wallCheckRadius);
    }
}
