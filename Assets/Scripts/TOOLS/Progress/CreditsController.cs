using UnityEngine;

public class CreditsController : MonoBehaviour
{
    public void OnCreditsFinished()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            Debug.LogWarning("[CreditsController] GameManager instance not found!");
        }
    }
}