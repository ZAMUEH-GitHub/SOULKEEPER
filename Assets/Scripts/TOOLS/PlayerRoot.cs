using UnityEngine;

public class PlayerRoot : Singleton<PlayerRoot>
{
    protected override bool IsPersistent => true;

    [Header("Root's Game Objects")]
    public GameObject _Player;
    public GameObject _Camera;

    private PlayerController _PlayerController;

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        _PlayerController = GetComponentInChildren<PlayerController>();
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }
    #endregion

    #region Root GameState Logic
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
    #endregion

    #region Player Enabling/Disabling
    public void DisablePlayerObject()
    {
        _Player.SetActive(false);
    }

    public void EnablePlayerObject()
    {
        _Player.SetActive(true);
    }

    #endregion
}
