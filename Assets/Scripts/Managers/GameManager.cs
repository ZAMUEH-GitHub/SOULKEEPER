using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region Player Stats
    public bool IsPaused { get; private set; }

    [Header("Player Stats")]
    public int playerScore = 0;
    public int playerHealth = 5;
    public GameObject[] healthSprites;
    private PlayerController playerController;
    #endregion

    #region Power-Up Tracking

    [Header("Power Ups")]
    public bool dashUnlocked;
    public bool dashUpgraded;
    public bool doubleJumpUnlocked;
    public bool doubleJumpUpgraded;
    public bool wallJumpUnlocked;
    public bool wallJumpUpgraded;
    public bool attackSpeedUpgraded;
    public bool attackDamageUpgraded;

    #endregion

    #region PowerUp HUD Sprites

    [Header("PowerUp HUD Sprites")]
    public GameObject dashUnlockedSprite;
    public GameObject dashUpgradedSprite;

    public GameObject doubleJumpUnlockedSprite;
    public GameObject doubleJumpUpgradedSprite;

    public GameObject wallJumpUnlockedSprite;
    public GameObject wallJumpUpgradedSprite;

    public GameObject attackSpeedUnlockedSprite;
    public GameObject attackDamageUpgradedSprite;

    #endregion

    #region UI Elements

    [Header("Text Boxes")]
    public TextMeshProUGUI scoreTextBox;

    [Header("Canvas Groups")]
    public CanvasGroup panelHUD;
    public CanvasGroup panelPauseMenu;
    public CanvasGroup panelSettings;
    public CanvasGroup panelKeybindings;

    #endregion

    // Added for volume control persistence
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float savedVolume = 1f;

    private void Start()
    {
        playerController = GameObject.Find("PLAYER").GetComponent<PlayerController>();

        // Load saved volume on start
        savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = savedVolume;
    }

    #region Pause Menu Controls

    public void OpenPauseMenu()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        ShowPanel(panelPauseMenu);
        HidePanel(panelHUD);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        HidePanel(panelPauseMenu);
        ShowPanel(panelHUD);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("Main Menu");
    }

    public void OpenSettingsMenu()
    {
        HidePanel(panelPauseMenu);
        ShowPanel(panelSettings);
    }

    public void BackToPauseMenu()
    {
        HidePanel(panelSettings);
        HidePanel(panelKeybindings);
        ShowPanel(panelPauseMenu);
    }

    public void ShowKeybindingsPanel()
    {
        HidePanel(panelSettings);
        ShowPanel(panelKeybindings);
    }

    public void BackToSettingsMenu()
    {
        HidePanel(panelKeybindings);
        ShowPanel(panelSettings);
    }

    #endregion

    #region Settings Menu Controls

    private bool isMuted = false;

    public enum DifficultyLevel { Easy, Normal, Hard }
    public DifficultyLevel currentDifficulty = DifficultyLevel.Normal;

    public void ToggleMute(bool mute)
    {
        isMuted = mute;
        AudioListener.volume = mute ? 0f : savedVolume;
    }

    public void AdjustVolume(float volume)
    {
        savedVolume = volume;
        if (!isMuted)
            AudioListener.volume = volume;

        // Save volume persistently
        PlayerPrefs.SetFloat("Volume", savedVolume);
        PlayerPrefs.Save();
    }

    public void SetDifficulty(int difficultyIndex)
    {
        currentDifficulty = (DifficultyLevel)difficultyIndex;
    }

    #endregion

    #region UI Helpers

    public void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 1f;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    public void HidePanel(CanvasGroup panel)
    {
        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    #endregion

    #region UI Updaters

    public void HealthUpdater(int currentHealth)
    {
        for (int i = 0; i < healthSprites.Length; i++)
        {
            healthSprites[i].SetActive(i < currentHealth);
        }

        playerHealth = playerController.playerHealth;
    }

    public void ScoreUpdater(int score)
    {
        playerScore += score;
        scoreTextBox.text = " " + playerScore;
    }
    public void SpendScore(int amount)
    {
        playerScore -= amount;
        ScoreUpdater(0);
    }

    public void DashUpdater(float nextDash)
    {
        // Reserved for dash UI logic
    }

    #endregion

    #region PowerUp HUD Update

    public void UpdatePowerUpHUD()
    {
        // Dash
        dashUnlockedSprite.SetActive(dashUnlocked && !dashUpgraded);
        dashUpgradedSprite.SetActive(dashUpgraded);

        // Double Jump
        doubleJumpUnlockedSprite.SetActive(doubleJumpUnlocked && !doubleJumpUpgraded);
        doubleJumpUpgradedSprite.SetActive(doubleJumpUpgraded);

        // Wall Jump
        wallJumpUnlockedSprite.SetActive(wallJumpUnlocked && !wallJumpUpgraded);
        wallJumpUpgradedSprite.SetActive(wallJumpUpgraded);

        // Attack PowerUps
        attackSpeedUnlockedSprite.SetActive(attackSpeedUpgraded && !attackDamageUpgraded);
        attackDamageUpgradedSprite.SetActive(attackDamageUpgraded);
    }

    #endregion
    public void ResetPlayerStats()
    {
        playerHealth = 5;
        HealthUpdater(playerHealth);
        playerScore = 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TheCathedralOfTheLost")
        {
            ResetPlayerStats();
        }
    }
}
