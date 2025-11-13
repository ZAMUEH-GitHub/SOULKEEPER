using UnityEngine;

public class PlayerRoot : Singleton<PlayerRoot>
{
    protected override bool IsPersistent => true;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.MainMenu)
        {
            DestroyInstance();
        }
    }

    public static void DestroyInstance()
    {
        if (_instance == null) return;

        if (_instance.gameObject != null)
            Destroy(_instance.gameObject);

        _instance = null;
    }
}
