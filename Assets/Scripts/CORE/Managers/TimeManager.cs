using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public void ResetTime()
    {
        Time.timeScale = 1.0f;
    }   

    public void FreezeTime()
    {
        Time.timeScale = 0.0f;
    }

    public void SlowTime(float slowMultiplier)
    {
        Time.timeScale = slowMultiplier;
    }
}
