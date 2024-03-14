using DG.Tweening;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] RectTransform _mainMenu;
    [SerializeField] RectTransform _deathMenu;
    [SerializeField] RectTransform _loadingMenu;
    [SerializeField] RectTransform _leaderboardMenu;
    Vector2 _movementOffset;
    [SerializeField] float _movementTime = 0.25f;

    void Start()
    {
        _movementOffset = new Vector2(_mainMenu.rect.width + 100f, _mainMenu.rect.height + 100f);

        _loadingMenu.DOAnchorPos(new Vector2(0, -_movementOffset.y), _movementTime);
        _mainMenu.DOAnchorPos(Vector2.zero, 0);
        _deathMenu.DOAnchorPos(new Vector2(0, _movementOffset.y), 0);
        _leaderboardMenu.DOAnchorPos(new Vector2(-_movementOffset.x, 0), 0);

        Singleton.Instance.LevelGenerationManagerInstance.StartLevelEvent += OnGameStart;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += OnGameReset;
        Singleton.Instance.PlayerCollisionControllerInstance.OnPlayerDeathEvent += OnPlayerDeath;
        Singleton.Instance.LevelGenerationManagerInstance.ContinueLevelEvent += OnGameContinue;
    }

    void OnGameContinue()
    {
        _deathMenu.DOAnchorPos(new Vector2(0, _movementOffset.y), _movementTime);
    }

    void OnGameStart()
    {
        _mainMenu.DOAnchorPos(new Vector2(0, -_movementOffset.y), _movementTime);
    }

    void OnPlayerDeath()
    {
        _deathMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    public void OnLeadearboardButton()
    {
        _leaderboardMenu.DOAnchorPos(Vector2.zero, _movementTime);
        _mainMenu.DOAnchorPos(new Vector2(_movementOffset.x, 0), _movementTime);
    }

    public void OnBackToMainMenuButton()
    {
        _leaderboardMenu.DOAnchorPos(new Vector2(-_movementOffset.x, 0), _movementTime);
        _mainMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    void OnGameReset()
    {
        _deathMenu.DOAnchorPos(new Vector2(0, _movementOffset.y), _movementTime);
        _mainMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    public void LogOutButton()
    {
        _loadingMenu.DOAnchorPos(Vector2.zero, _movementTime);
        FirebaseAuth.DefaultInstance.SignOut();
        Invoke(nameof(LoadAuthMenuScene), _movementTime);
    }

    void LoadAuthMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitFromGameButton()
    {
#if UNITY_EDITOR
        Debug.Log("Game exit!");
#endif
        Application.Quit();
    }
}
