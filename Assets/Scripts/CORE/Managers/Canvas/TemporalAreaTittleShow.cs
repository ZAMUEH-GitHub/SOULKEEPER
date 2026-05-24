using UnityEngine;

public class TemporalAreaTittleShow : Singleton<TemporalAreaTittleShow>
{
    // You'll need to define this since your Singleton base class requires it
    protected override bool IsPersistent => false;
    public Animator tittleAnimator; 

    public void showTitle(string title)
    {
        tittleAnimator.SetTrigger(title);
    }
}
