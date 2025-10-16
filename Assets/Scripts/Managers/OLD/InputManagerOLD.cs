/*using UnityEngine;

public class InputManagerOLD : Singleton<InputManagerOLD>
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.panelKeybindings.alpha > 0)
            {
                GameManager.Instance.HidePanel(GameManager.Instance.panelKeybindings);
                GameManager.Instance.ShowPanel(GameManager.Instance.panelSettings);
                return;
            }

            if (GameManager.Instance.panelSettings.alpha > 0)
            {
                GameManager.Instance.BackToPauseMenu();
                return;
            }

            if (GameManager.Instance.IsPaused)
            {
                GameManager.Instance.ResumeGame();
            }
            else
            {
                GameManager.Instance.OpenPauseMenu();
            }
        }
    }
}*/
