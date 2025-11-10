using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Gameplay
}

public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnGameStateChanged;

    [field: SerializeField] public GameState CurrentState { get; private set; }

    [Header("Scene References")]
    [SerializeField] private SceneField mainMenuScene;
    [SerializeField] private CanvasManager canvasManager;

    [Header("Player Data Management")]
    [SerializeField] private PlayerStatsSO basePlayerStats;
    public static PlayerStatsSO RuntimePlayerStats { get; private set; }

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        canvasManager ??= CanvasManager.Instance;

        if (canvasManager != null)
            canvasManager.FadeOut(PanelType.BlackScreen);

        OnGameStateChanged += HandleGameStateChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnGameStateChanged -= HandleGameStateChanged;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuScene.SceneName)
            SetState(GameState.MainMenu);
        else
            SetState(GameState.Gameplay);
    }

    public void SetState(GameState newState)
    {
        if (newState == CurrentState)
            return;

        CurrentState = newState;
        Debug.Log($"[GameManager] State changed to {newState}");
        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                if (RuntimePlayerStats != null)
                {
                    RuntimePlayerStats = null;
                    Debug.Log("[GameManager] Cleared runtime PlayerStatsSO (MainMenu)");
                }
                break;

            case GameState.Gameplay:
                if (RuntimePlayerStats == null && basePlayerStats != null)
                {
                    RuntimePlayerStats = basePlayerStats.Clone();
                    Debug.Log("[GameManager] Created runtime PlayerStatsSO clone for Gameplay");
                }
                break;
        }
    }

    public void StartNewGame()
    {
        if (basePlayerStats == null)
        {
            Debug.LogError("[GameManager] Missing base PlayerStatsSO reference!");
            return;
        }

        RuntimePlayerStats = basePlayerStats.Clone();
        Debug.Log("[GameManager] New Game started — fresh PlayerStats clone created.");
    }

    public void ReturnToMainMenu()
    {
        SetState(GameState.MainMenu);
    }

    public void EnterGameplay()
    {
        SetState(GameState.Gameplay);
    }
}
