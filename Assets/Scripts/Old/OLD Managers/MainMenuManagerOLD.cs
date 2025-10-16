/*using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManagerOLD : MonoBehaviour
{
    [Header("Panels")]
    public CanvasGroup panelMainMenu;
    public CanvasGroup panelSettings;
    public CanvasGroup panelKeybindings;
    public CanvasGroup panelCredits;

    [Header("Fade Loading")]
    public CanvasGroup fadePanel;   // Assign your black fullscreen panel here

    private bool isMuted = false;
    private float fadeDuration = 1f;

    // Coroutine references for fades to avoid overlapping
    private Coroutine fadeMainMenu;
    private Coroutine fadeSettings;
    private Coroutine fadeKeybindings;
    private Coroutine fadeCredits;

    private void Start()
    {
        ShowPanel(panelMainMenu);
        HidePanel(panelSettings);
        HidePanel(panelKeybindings);
        HidePanel(panelCredits);

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }

        // Initialize sound volume and mute from SoundManager if exists
        if (SoundManager.Instance != null)
        {
            // Assuming SoundManager has these methods, else add them:
            SoundManager.Instance.SetMute(isMuted);
            SoundManager.Instance.SetVolume(PlayerPrefs.GetFloat("Volume", 1f));
        }
    }

    #region Panel Navigation

    public void OpenSettings()
    {
        HidePanel(panelMainMenu);
        ShowPanel(panelSettings);
    }

    public void OpenKeybindings()
    {
        HidePanel(panelSettings);
        ShowPanel(panelKeybindings);
    }

    public void OpenCredits()
    {
        HidePanel(panelMainMenu);
        ShowPanel(panelCredits);
    }

    public void BackToMainMenu()
    {
        HidePanel(panelSettings);
        HidePanel(panelKeybindings);
        HidePanel(panelCredits);
        ShowPanel(panelMainMenu);
    }

    public void BackToSettings()
    {
        HidePanel(panelKeybindings);
        ShowPanel(panelSettings);
    }

    #endregion

    #region Panel Helpers

    public void ShowPanel(CanvasGroup panel)
    {
        StopFadeCoroutine(panel);
        StartFadeCoroutine(panel, 1f, true);
    }

    public void HidePanel(CanvasGroup panel)
    {
        StopFadeCoroutine(panel);
        StartFadeCoroutine(panel, 0f, false);
    }

    private void StopFadeCoroutine(CanvasGroup panel)
    {
        if (panel == panelMainMenu && fadeMainMenu != null)
        {
            StopCoroutine(fadeMainMenu);
            fadeMainMenu = null;
        }
        else if (panel == panelSettings && fadeSettings != null)
        {
            StopCoroutine(fadeSettings);
            fadeSettings = null;
        }
        else if (panel == panelKeybindings && fadeKeybindings != null)
        {
            StopCoroutine(fadeKeybindings);
            fadeKeybindings = null;
        }
        else if (panel == panelCredits && fadeCredits != null)
        {
            StopCoroutine(fadeCredits);
            fadeCredits = null;
        }
    }

    private void StartFadeCoroutine(CanvasGroup panel, float targetAlpha, bool interactable)
    {
        if (panel == panelMainMenu)
            fadeMainMenu = StartCoroutine(FadeCanvasGroup(panel, targetAlpha, interactable));
        else if (panel == panelSettings)
            fadeSettings = StartCoroutine(FadeCanvasGroup(panel, targetAlpha, interactable));
        else if (panel == panelKeybindings)
            fadeKeybindings = StartCoroutine(FadeCanvasGroup(panel, targetAlpha, interactable));
        else if (panel == panelCredits)
            fadeCredits = StartCoroutine(FadeCanvasGroup(panel, targetAlpha, interactable));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha, bool interactable)
    {
        float duration = 0.5f;
        float startAlpha = cg.alpha;
        float elapsed = 0f;

        if (targetAlpha > 0f)
            cg.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        cg.alpha = targetAlpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;

        if (targetAlpha == 0f)
            cg.gameObject.SetActive(false);

        // Clear coroutine references
        if (cg == panelMainMenu) fadeMainMenu = null;
        else if (cg == panelSettings) fadeSettings = null;
        else if (cg == panelKeybindings) fadeKeybindings = null;
        else if (cg == panelCredits) fadeCredits = null;
    }

    #endregion

    #region Button Actions

    public void StartGame()
    {
        StartCoroutine(FadeOutAndLoadScene("TheCathedralOfTheLost"));
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (fadePanel == null)
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        fadePanel.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 1f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
            yield return null;

        asyncLoad.allowSceneActivation = true;
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion

    #region Settings Controls

    public void ToggleMute(bool mute)
    {
        isMuted = mute;
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMute(mute);
    }

    public void AdjustVolume(float volume)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolume(volume);
            isMuted = volume <= 0f;
        }
        // Save volume persistently
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    #endregion
}
*/