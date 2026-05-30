using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimatorRebind : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }
}