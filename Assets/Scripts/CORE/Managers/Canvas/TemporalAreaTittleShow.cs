using UnityEngine;

public class TemporalAreaTittleShow : Singleton<TemporalAreaTittleShow>
{
    protected override bool IsPersistent => false;
    public Animator titleAnimator; 

    public void showTitle(string title)
    {
        titleAnimator.SetTrigger(title);
    }
}
