using DG.Tweening;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] RectTransform _mainMenu;
    [SerializeField] RectTransform _gameMenu;
    [SerializeField] RectTransform _deathMenu;
    [SerializeField] RectTransform _loadingMenu;
    [SerializeField] RectTransform _leaderboardMenu;
    [SerializeField] RectTransform _pauseMenu;
    [SerializeField] RectTransform _optionsMenu;
    Vector2 _movementOffset;
    [SerializeField] float _movementTime = 0.25f;

    AudioSource _audioSource;
    [SerializeField] AudioClip _buttonClickSound;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        _audioSource = GetComponent<AudioSource>();

        _movementOffset = new Vector2(_mainMenu.rect.width + 100f, _mainMenu.rect.height + 100f);

        _loadingMenu.DOAnchorPos(new Vector2(0, -_movementOffset.y), _movementTime);
        _mainMenu.DOAnchorPos(Vector2.zero, 0);
        _deathMenu.DOAnchorPos(new Vector2(0, _movementOffset.y), 0);
        _leaderboardMenu.DOAnchorPos(new Vector2(-_movementOffset.x, 0), 0);
        _pauseMenu.DOAnchorPos(new Vector2(_movementOffset.x, 0), 0);
        _gameMenu.DOAnchorPos(new Vector2(0, _movementOffset.y), 0);

        Singleton.Instance.LevelGenerationManagerInstance.StartLevelEvent += OnGameStart;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += OnGameReset;
        Singleton.Instance.PlayerCollisionControllerInstance.OnPlayerDeathEvent += OnPlayerDeath;
        Singleton.Instance.LevelGenerationManagerInstance.ContinueLevelEvent += OnGameContinue;
    }

    public void PlayButtonSound()
    {
        _audioSource.PlayOneShot(_buttonClickSound);
    }

    public void OnOptionsButton() // Переход в меню настроек
    {
        _mainMenu.DOAnchorPos(new Vector2(-_movementOffset.x, 0), _movementTime);
        _optionsMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    public void OnReturnFromOptionsButton() // Переход из меню опций в главное меню
    {
        _mainMenu.DOAnchorPos(Vector2.zero, _movementTime);
        _optionsMenu.DOAnchorPos(new Vector2(_movementOffset.x, 0), _movementTime);
    }

    void OnGameContinue() // Переход обратно к игре после просмотра рекламы
    {
        _deathMenu.DOAnchorPos(new Vector2(0, _movementOffset.y), _movementTime);
    }

    void OnGameStart() // При начале игры
    {
        _mainMenu.DOAnchorPos(new Vector2(0, -_movementOffset.y), _movementTime);
        _gameMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    void OnPlayerDeath() // При смерти игрока
    {
        _deathMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    public void OnLeadearboardButton() // Откырть таблицу лидеров с главного меню
    {
        _leaderboardMenu.DOAnchorPos(Vector2.zero, _movementTime);
        _mainMenu.DOAnchorPos(new Vector2(_movementOffset.x, 0), _movementTime);
    }
    public void OnBackToMainMenuPauseButton() // Возврат в главное меню из меню паузы
    {
        Time.timeScale = 1f;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevel();
    }
    public void OnPauseButton() // При нажатии паузы
    {
        _pauseMenu.DOAnchorPos(Vector2.zero, 0);
        Time.timeScale = 0f;
        _gameMenu.DOAnchorPos(new Vector2(-_movementOffset.x, 0), 0);
    }
    public void OnResumeButton() // При нажатии продолжить из меню паузы
    {
        Time.timeScale = 1f;
        _pauseMenu.DOAnchorPos(new Vector2(_movementOffset.x, 0), _movementTime);
        _gameMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    public void OnBackToMainMenuButton() // Возврат в главное меню из таблицы лидеров
    {
        _leaderboardMenu.DOAnchorPos(new Vector2(-_movementOffset.x, 0), _movementTime);
        _mainMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    void OnGameReset()
    {
        _deathMenu.DOAnchorPos(new Vector2(0, _movementOffset.y), _movementTime);
        _mainMenu.DOAnchorPos(Vector2.zero, _movementTime);
        _gameMenu.DOAnchorPos(new Vector2(0, _movementOffset.y), _movementTime);
        _pauseMenu.DOAnchorPos(new Vector2(_movementOffset.x, 0), _movementTime);
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
