using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLevelLoadingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup blackFadePanel;
    public GameObject loadingSpinner;  // Optional

    [Header("Settings")]
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        // Start hidden
        blackFadePanel.alpha = 0f;
        blackFadePanel.interactable = false;
        blackFadePanel.blocksRaycasts = false;
        if (loadingSpinner != null)
            loadingSpinner.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(FadeCanvasGroup(blackFadePanel, 0f, 1f, fadeDuration));
        if (loadingSpinner != null)
            loadingSpinner.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade out loading UI
        if (loadingSpinner != null)
            loadingSpinner.SetActive(false);
        yield return StartCoroutine(FadeCanvasGroup(blackFadePanel, 1f, 0f, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        cg.alpha = end;

        bool visible = end > 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}
