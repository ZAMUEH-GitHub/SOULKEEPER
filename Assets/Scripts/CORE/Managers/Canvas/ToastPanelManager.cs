using System.Collections;
using UnityEngine;
using TMPro;

public class ToastPanelManager : Singleton<ToastPanelManager>
{
    protected override bool IsPersistent => false;

    [Header("UI Reference")]
    [SerializeField] private TMP_Text toastText;

    public void ShowToast(string message, float duration = 1.5f)
    {
        if (toastText == null) return;
        toastText.text = message;

        CanvasManager.Instance.FadeIn(PanelType.ToastPanel);
        StartCoroutine(HideToastAfterDelay(duration));
    }

    private IEnumerator HideToastAfterDelay(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        CanvasManager.Instance.FadeOut(PanelType.ToastPanel);
    }
}
