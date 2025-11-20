using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Image[] healthIcons;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Power-Up Icons")]
    [SerializeField] private GameObject jumpIcon;
    [SerializeField] private GameObject dashIcon;
    [SerializeField] private GameObject attackIcon;
    [SerializeField] private GameObject wallSlideIcon;
    [SerializeField] private GameObject wallJumpIcon;

    private PlayerStatsSO stats;
    private int lastHealth = -1;
    private int lastMaxHealth = -1;
    private int lastScore = -1;

    private bool lastJump, lastDash, lastAttack, lastWallSlide, lastWallJump;

    private void OnEnable()
    {
        var session = SessionManager.Instance;
        if (session != null && session.HasActiveSession)
        {
            stats = session.RuntimeStats;
            UpdateAllHealthIcons();
        }
    }

    private void Update()
    {
        if (stats == null) return;

        UpdateHealthUI();
        UpdateScoreUI();
        UpdatePowerUpsUI();
    }

    #region Health
    private void UpdateHealthUI()
    {
        if (stats.health == lastHealth && stats.maxHealth == lastMaxHealth)
            return;

        UpdateAllHealthIcons();
    }

    private void UpdateAllHealthIcons()
    {
        lastHealth = stats.health;
        lastMaxHealth = stats.maxHealth;

        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (i < stats.maxHealth)
            {
                healthIcons[i].enabled = true;
                healthIcons[i].sprite = (i < stats.health) ? fullHeartSprite : emptyHeartSprite;
            }
            else
            {
                healthIcons[i].enabled = false;
            }
        }
    }
    #endregion

    #region Score
    private void UpdateScoreUI()
    {
        if (scoreText == null) return;
        if (stats.score == lastScore) return;

        lastScore = stats.score;
        scoreText.text = $"{stats.score:N0}";
    }
    #endregion

    #region PowerUps
    private void UpdatePowerUpsUI()
    {
        if (stats == null) return;

        if (jumpIcon != null && stats.jumpUnlocked != lastJump)
            jumpIcon.SetActive(stats.jumpUnlocked);

        if (dashIcon != null && stats.dashUnlocked != lastDash)
            dashIcon.SetActive(stats.dashUnlocked);

        if (attackIcon != null && stats.attackUnlocked != lastAttack)
            attackIcon.SetActive(stats.attackUnlocked);

        if (wallSlideIcon != null && stats.wallSlideUnlocked != lastWallSlide)
            wallSlideIcon.SetActive(stats.wallSlideUnlocked);

        if (wallJumpIcon != null && stats.wallJumpUnlocked != lastWallJump)
            wallJumpIcon.SetActive(stats.wallJumpUnlocked);

        lastJump = stats.jumpUnlocked;
        lastDash = stats.dashUnlocked;
        lastAttack = stats.attackUnlocked;
        lastWallSlide = stats.wallSlideUnlocked;
        lastWallJump = stats.wallJumpUnlocked;
    }
    #endregion
}
