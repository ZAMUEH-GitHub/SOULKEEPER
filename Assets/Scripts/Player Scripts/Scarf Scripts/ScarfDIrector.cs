using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class ScarfDIrector : MonoBehaviour
{
    public PlayerController playerController;

    public void Update()
    {
        Flip();
    }
    private void Flip()
    {
        if (playerController.moveVector.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
        if (playerController.moveVector.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }
}
