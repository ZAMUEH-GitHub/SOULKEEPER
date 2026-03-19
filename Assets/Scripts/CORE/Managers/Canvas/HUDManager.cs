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
            ForceUpdateUI();
        }
    }

    private void Update()
    {
        if (stats == null)
        {
            var session = SessionManager.Instance;
            if (session != null && session.HasActiveSession)
            {
                stats = session.RuntimeStats;
                ForceUpdateUI();
            }

            if (stats == null) return;
        }

        UpdateHealthUI();
        UpdateScoreUI();
        UpdatePowerUpsUI();
    }

    private void ForceUpdateUI()
    {
        UpdateAllHealthIcons();
        UpdateScoreUI(true);
        UpdatePowerUpsUI(true);
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
    private void UpdateScoreUI(bool force = false)
    {
        if (scoreText == null) return;
        if (!force && stats.score == lastScore) return;

        lastScore = stats.score;
        scoreText.text = $"{stats.score:N0}";
    }
    #endregion

    #region PowerUps
    private void UpdatePowerUpsUI(bool force = false)
    {
        if (stats == null) return;

        if (jumpIcon != null && (force || stats.jumpUnlocked != lastJump))
            jumpIcon.SetActive(stats.jumpUnlocked);

        if (dashIcon != null && (force || stats.dashUnlocked != lastDash))
            dashIcon.SetActive(stats.dashUnlocked);

        if (attackIcon != null && (force || stats.attackUnlocked != lastAttack))
            attackIcon.SetActive(stats.attackUnlocked);

        if (wallSlideIcon != null && (force || stats.wallSlideUnlocked != lastWallSlide))
            wallSlideIcon.SetActive(stats.wallSlideUnlocked);

        if (wallJumpIcon != null && (force || stats.wallJumpUnlocked != lastWallJump))
            wallJumpIcon.SetActive(stats.wallJumpUnlocked);

        lastJump = stats.jumpUnlocked;
        lastDash = stats.dashUnlocked;
        lastAttack = stats.attackUnlocked;
        lastWallSlide = stats.wallSlideUnlocked;
        lastWallJump = stats.wallJumpUnlocked;
    }
    #endregion
}