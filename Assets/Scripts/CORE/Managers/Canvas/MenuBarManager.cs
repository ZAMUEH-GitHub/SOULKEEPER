using UnityEngine;

public class MenuBarManager : MonoBehaviour
{
    [Header("Animation Durations")]
    [Tooltip("Match this to your Close animation length")]
    [SerializeField] private float closeDuration = 0.3f;
    [Tooltip("Match this to your Open animation length")]
    [SerializeField] private float openDuration = 0.3f;

    [Header("Component References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AnimationEventRelay eventRelay;

    private readonly int OpenTrigger = Animator.StringToHash("Open");
    private readonly int CloseTrigger = Animator.StringToHash("Close");

    public float CloseDuration => closeDuration;
    public float OpenDuration => openDuration;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>(true);
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (eventRelay == null) eventRelay = GetComponentInChildren<AnimationEventRelay>(true);
    }

    public void CloseBar()
    {
        if (animator != null) animator.SetTrigger(CloseTrigger);
    }

    public void OpenBar()
    {
        if (animator != null) animator.SetTrigger(OpenTrigger);
    }

    public void SetVisible(bool isVisible)
    {
        if (canvasGroup != null) canvasGroup.alpha = isVisible ? 1f : 0f;
    }
}