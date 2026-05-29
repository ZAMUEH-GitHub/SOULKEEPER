using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRoot : Singleton<PlayerRoot>
{
    protected override bool IsPersistent => true;

    [Header("Root's Game Objects")]
    public GameObject _Player;
    public GameObject _Camera;
    public GameObject _Godiva;

    private PlayerController _PlayerController;

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        _PlayerController = GetComponentInChildren<PlayerController>();
    }

    private void Start()
    {
        // Check the scene immediately upon spawning
        CheckGodivaState(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Scene Logic
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckGodivaState(scene.name);
    }

    private void CheckGodivaState(string sceneName)
    {
        if (sceneName == "1_Tutorial")
        {
            DisableGodivaObject();
        }
        else
        {
            EnableGodivaObject();
        }
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

    public void DisableGodivaObject()
    {
        if (_Godiva != null) _Godiva.SetActive(false);
    }

    public void EnableGodivaObject()
    {
        if (_Godiva != null) _Godiva.SetActive(true);
    }
    #endregion
}