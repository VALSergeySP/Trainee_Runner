using UnityEngine;

public class Singleton : MonoBehaviour
{
    private Singleton() { }
    public static Singleton Instance { get; private set; }

    [SerializeField] private LevelGenerationManager _levelGenerationManager;
    public LevelGenerationManager LevelGenerationManagerInstance => _levelGenerationManager;

    [SerializeField] private PoolManager _poolManager;
    public PoolManager PoolManagerInstance => _poolManager;

    [SerializeField] private ObstaclesGenerationManager _obstaclesGenerationManager;
    public ObstaclesGenerationManager ObstaclesGenerationManagerInstance => _obstaclesGenerationManager;

    [SerializeField] private SwipeManager _swipeManager;
    public SwipeManager SwipeManagerInstance => _swipeManager;

    [SerializeField] private ScorePointsManager _scorePointsManager;
    public ScorePointsManager ScorePointsManagerInstance => _scorePointsManager;


    [SerializeField] private UIManager _uiManager;
    public UIManager UIManagerInstance => _uiManager;
    
    [SerializeField] private PlayerCollisionController _playerCollisionController;
    public PlayerCollisionController PlayerCollisionControllerInstance => _playerCollisionController;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
