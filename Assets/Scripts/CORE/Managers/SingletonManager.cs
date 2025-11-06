using UnityEngine;

public class SingletonManager : MonoBehaviour
{
    [Header("Singleton References")]
    public GameObject _Player;
    public GameObject _Camera;
    public GameObject _GameManager;
    public GameObject _GameSceneManager;
    public GameObject _AudioManager;
    public GameObject _TimeManager;
    public GameObject _CanvasGroup;
    public GameObject _MainMenuCanvas;
    public GameObject _GameplayCanvas;
    public GameObject _EventSystem;

    [Header("Scene References")]
    public SceneField mainMenuScene;

    private GameManager gameManager;
    private CanvasManager canvasManager;

    private void Start()
    {
        gameManager = _GameManager.GetComponent<GameManager>();
        canvasManager = _CanvasGroup.GetComponent<CanvasManager>();

        if (_GameManager != null)
            gameManager = _GameManager.GetComponent<GameManager>();
        else
            gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
            HandleGameStateChanged(gameManager.CurrentState);

        GameManager.OnGameStateChanged += HandleGameStateChanged;

        if (_CanvasGroup != null)
            canvasManager = _CanvasGroup.GetComponent<CanvasManager>();
        else
            canvasManager = FindFirstObjectByType<CanvasManager>();
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        bool isMainMenu = state == GameState.MainMenu;

        SafeSetActive(_Player, !isMainMenu);
        SafeSetActive(_Camera, !isMainMenu);
        SafeSetActive(_MainMenuCanvas, isMainMenu);

        canvasManager.ToggleCanvasInteractivity(_MainMenuCanvas, isMainMenu);
        canvasManager.ToggleCanvasInteractivity(_GameplayCanvas, !isMainMenu);
    }

    private void SafeSetActive(GameObject obj, bool active)
    {
        if (obj == null) return;
        if (obj.activeSelf == active) return;
        obj.SetActive(active);
    }
}
