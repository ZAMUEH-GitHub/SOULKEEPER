using UnityEngine;

public class SingletonManager : Singleton<SingletonManager>
{
    [Header("Singleton References")]
    [SerializeField] private GameObject _PlayerRootPrefab;
    [SerializeField] private Transform _PlayerRootParent;

    [Header("UI & Manager References")]
    public GameObject _GameManager;
    public GameObject _GameSceneManager;
    public GameObject _SceneDoorManager;
    public GameObject _AudioManager;
    public GameObject _TimeManager;
    public GameObject _SaveSlotManager;
    public GameObject _CanvasGroup;
    public GameObject _GlobalCanvas;
    public GameObject _MainMenuCanvas;
    public GameObject _GameplayCanvas;
    public GameObject _EventSystem;

    [Header("Scene References")]
    public SceneField mainMenuScene;

    private GameManager gameManager;
    private CanvasManager canvasManager;
    private GameObject currentPlayerRoot;

    #region Unity Lifecycle
    private void Start()
    {
        gameManager = _GameManager != null ? _GameManager.GetComponent<GameManager>() : FindFirstObjectByType<GameManager>();
        canvasManager = _CanvasGroup != null ? _CanvasGroup.GetComponent<CanvasManager>() : FindFirstObjectByType<CanvasManager>();

        if (gameManager != null)
            HandleGameStateChanged(gameManager.CurrentState);

        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }
    #endregion

    #region GameState Reaction
    private void HandleGameStateChanged(GameState state)
    {
        bool isMainMenu = state == GameState.MainMenu;

        SafeSetActive(_MainMenuCanvas, isMainMenu);
        SafeSetActive(_GameplayCanvas, !isMainMenu);
        SafeSetActive(_GlobalCanvas, true);

        if (isMainMenu)
        {
            DestroyPlayerRoot();
        }
        else
        {
            SpawnPlayerRoot();
        }
    }
    #endregion

    #region PlayerRoot Management
    private void SpawnPlayerRoot()
    {
        if (_PlayerRootPrefab == null)
        {
            Debug.LogError("[SingletonManager] PlayerRoot prefab is missing!");
            return;
        }

        if (currentPlayerRoot != null)
        {
            Debug.LogWarning("[SingletonManager] PlayerRoot already exists — skipping spawn.");
            return;
        }

        currentPlayerRoot = Instantiate(_PlayerRootPrefab, _PlayerRootParent);
        currentPlayerRoot.name = "PLAYER ROOT";

        Debug.Log("[SingletonManager] Spawned new PlayerRoot prefab.");
    }

    private void DestroyPlayerRoot()
    {
        if (currentPlayerRoot == null) return;

        Destroy(currentPlayerRoot);
        currentPlayerRoot = null;

        Debug.Log("[SingletonManager] Destroyed PlayerRoot prefab.");
    }
    #endregion

    #region Utility
    private void SafeSetActive(GameObject obj, bool active)
    {
        if (obj == null) return;
        if (obj.activeSelf == active) return;
        obj.SetActive(active);
    }
    #endregion
}
