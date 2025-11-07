using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Gameplay
}

public class GameManager : MonoBehaviour
{
    public static event Action<GameState> OnGameStateChanged;
    
    [field: SerializeField] public GameState CurrentState { get; private set; }

    [SerializeField] private SceneField mainMenuScene;
    [SerializeField] private CanvasManager canvasManager; 

    private void Start()
    {
        if (canvasManager == null)
            canvasManager = FindFirstObjectByType<CanvasManager>();

        if (canvasManager != null)
            canvasManager.FadeOut(PanelType.BlackScreen);
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
        if (scene.name == mainMenuScene.SceneName)
            SetState(GameState.MainMenu);
        else
            SetState(GameState.Gameplay);
    }

    private void SetState(GameState newState)
    {
        if (newState == CurrentState)
            return;

        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}
