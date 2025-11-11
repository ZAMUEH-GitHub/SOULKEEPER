using UnityEngine;


public class TimeManager : Singleton<TimeManager>
{
    protected override bool IsPersistent => false;

    [Header("Time Settings")]
    [Range(0f, 2f)][SerializeField] private float defaultTimeScale = 1f;

    private float _currentTimeScale = 1f;
    private float sessionStartTime;
    private float accumulatedPlaytime;

    public float CurrentTimeScale => _currentTimeScale;
    public bool IsPaused => Mathf.Approximately(_currentTimeScale, 0f);


    protected override void Awake()
    {
        base.Awake();
        ResetTime();
        ResetPlaytime();
    }


    #region Time Scale Controls

    public void ResetTime()
    {
        _currentTimeScale = defaultTimeScale;
        Time.timeScale = _currentTimeScale;
    }

    public void FreezeTime()
    {
        _currentTimeScale = 0f;
        Time.timeScale = 0f;
    }

    public void SlowTime(float slowMultiplier)
    {
        slowMultiplier = Mathf.Clamp(slowMultiplier, 0f, 1f);
        _currentTimeScale = slowMultiplier;
        Time.timeScale = _currentTimeScale;
    }

    public void SetCustomTime(float scale)
    {
        _currentTimeScale = Mathf.Clamp(scale, 0f, 2f);
        Time.timeScale = _currentTimeScale;
    }
    #endregion


    #region Playtime Tracking

    public void StartSessionTimer()
    {
        sessionStartTime = Time.realtimeSinceStartup;
    }

    public void StopSessionTimer()
    {
        if (sessionStartTime > 0f)
        {
            accumulatedPlaytime += Time.realtimeSinceStartup - sessionStartTime;
            sessionStartTime = 0f;
        }
    }

    public float GetTotalPlaytime()
    {
        if (sessionStartTime <= 0f)
            return accumulatedPlaytime;


        return accumulatedPlaytime + (Time.realtimeSinceStartup - sessionStartTime);
    }

    public void ResetPlaytime()
    {
        accumulatedPlaytime = 0f;
        sessionStartTime = Time.realtimeSinceStartup;
    }
    #endregion
}