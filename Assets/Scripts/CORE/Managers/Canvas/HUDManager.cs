using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Image[] healthIcons;
    [SerializeField] private Animator[] healthAnimators;

    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Power-Up Icons")]
    [SerializeField] private Animator decorLineAnimator;
    [SerializeField] private GameObject attackIcon;
    [SerializeField] private GameObject jumpIcon;
    [SerializeField] private GameObject dashIcon;
    [SerializeField] private GameObject wallJumpIcon;

    private PlayerStatsSO stats;
    private int lastHealth = -1;
    private int lastMaxHealth = -1;
    private int lastScore = -1;

    private int lastUnlockCount = -1;
    private bool lastJump, lastDash, lastAttack, lastWallJump;

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

            if (stats == null && PlayerController.Instance != null && PlayerController.Instance.playerRuntimeStats != null)
            {
                stats = PlayerController.Instance.playerRuntimeStats;
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
        lastHealth = stats.health;
        lastMaxHealth = stats.maxHealth;

        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (i < stats.maxHealth)
            {
                healthIcons[i].gameObject.SetActive(true);

                if (i < healthAnimators.Length && healthAnimators[i] != null)
                {
                    healthAnimators[i].ResetTrigger("Damage");
                    healthAnimators[i].ResetTrigger("Heal");

                    if (i < stats.health)
                    {
                        healthAnimators[i].Play("Health_Idle");
                    }
                    else
                    {
                        healthAnimators[i].Play("Health_Empty");
                    }
                }
            }
            else
            {
                healthIcons[i].gameObject.SetActive(false);
            }
        }

        UpdateScoreUI(true);
        UpdatePowerUpsUI(true);
    }

    #region Health
    private void UpdateHealthUI()
    {
        if (stats.health == lastHealth && stats.maxHealth == lastMaxHealth)
            return;

        if (stats.maxHealth != lastMaxHealth)
        {
            ForceUpdateUI();
            return;
        }

        if (lastHealth != -1 && healthAnimators != null)
        {
            if (stats.health < lastHealth)
            {
                for (int i = stats.health; i < lastHealth; i++)
                    if (i < healthAnimators.Length && healthAnimators[i] != null)
                        healthAnimators[i].SetTrigger("Damage");
            }
            else if (stats.health > lastHealth)
            {
                for (int i = lastHealth; i < stats.health; i++)
                    if (i < healthAnimators.Length && healthAnimators[i] != null)
                        healthAnimators[i].SetTrigger("Heal");
            }
        }

        lastHealth = stats.health;
        lastMaxHealth = stats.maxHealth;
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

        if (attackIcon != null && (force || stats.attackUnlocked != lastAttack))
            attackIcon.SetActive(stats.attackUnlocked);

        if (jumpIcon != null && (force || stats.jumpUnlocked != lastJump))
            jumpIcon.SetActive(stats.jumpUnlocked);

        if (dashIcon != null && (force || stats.dashUnlocked != lastDash))
            dashIcon.SetActive(stats.dashUnlocked);

        if (wallJumpIcon != null && (force || stats.wallJumpUnlocked != lastWallJump))
            wallJumpIcon.SetActive(stats.wallJumpUnlocked);

        lastAttack = stats.attackUnlocked;
        lastJump = stats.jumpUnlocked;
        lastDash = stats.dashUnlocked;
        lastWallJump = stats.wallJumpUnlocked;

        int currentUnlocks = 0;
        if (stats.attackUnlocked) currentUnlocks++;
        if (stats.jumpUnlocked) currentUnlocks++;
        if (stats.dashUnlocked) currentUnlocks++;
        if (stats.wallJumpUnlocked) currentUnlocks++;

        if (force || lastUnlockCount == -1)
        {
            if (decorLineAnimator != null)
            {
                if (currentUnlocks == 0) decorLineAnimator.Play("HUD_Bar_Idle");
                else if (currentUnlocks == 1) decorLineAnimator.Play("HUD_Bar_PowerUp1");
                else if (currentUnlocks == 2) decorLineAnimator.Play("HUD_Bar_PowerUp2");
                else if (currentUnlocks == 3) decorLineAnimator.Play("HUD_Bar_PowerUp3");
                else if (currentUnlocks >= 4) decorLineAnimator.Play("HUD_Bar_PowerUp4");
            }
        }
        else if (currentUnlocks > lastUnlockCount && decorLineAnimator != null)
        {
            int gained = currentUnlocks - lastUnlockCount;
            for (int i = 0; i < gained; i++)
            {
                decorLineAnimator.SetTrigger("NextUnlock");
            }
        }

        lastUnlockCount = currentUnlocks;
    }
    #endregion
}