/*using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : Singleton<GameSceneManager>
{
    [Header("Fade UI")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 2f;

    // Flag indicating if gameplay is active
    public bool IsGameActive { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("Fade Canvas Group not assigned in GameSceneManager.");
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(Fade(1));

        // Extra wait time to keep screen black longer before loading
        yield return new WaitForSeconds(1.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        yield return null;

        // Fade back from black
        yield return StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    protected override void OnSceneChanged(bool isGameplayScene)
    {
        IsGameActive = isGameplayScene;

        Debug.Log($"GameSceneManager: IsGameActive set to {IsGameActive}");

        // You could enable/disable UI elements here, or notify other managers if needed.
    }
}
*/