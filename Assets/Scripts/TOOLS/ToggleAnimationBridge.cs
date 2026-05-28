using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleAnimationBridge : MonoBehaviour
{
    [Header("Animator Reference")]
    [SerializeField] private Animator toggleAnimator;

    private Toggle toggle;
    private readonly int IsOnHash = Animator.StringToHash("IsOn");

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnEnable()
    {
        if (toggleAnimator != null)
        {
            toggleAnimator.SetBool(IsOnHash, toggle.isOn);

            if (toggle.isOn)
                toggleAnimator.Play("Checkbox_Active", 0, 1f);
            else
                toggleAnimator.Play("Checkbox_Idle", 0, 1f);
        }
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (toggleAnimator != null)
        {
            toggleAnimator.SetBool(IsOnHash, isOn);
        }
    }
}