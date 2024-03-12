using System.Collections;
using TMPro;
using UnityEngine;

public class ScorePointsManager : MonoBehaviour
{


    int _currentScore = 0;
    public int CurrentScore => _currentScore;
    [SerializeField] int _scorePointsPerSecond = 20;
    [SerializeField] TMP_Text _scoreTextField; // Только для теста (добавить UI контроллер позже)

    private void Start()
    {
        Singleton.Instance.LevelGenerationManagerInstance.StartLevelEvent += StartScorePointsManager;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += ResetScorePointsManager;
        Singleton.Instance.LevelGenerationManagerInstance.ContinueLevelEvent += ContinueScorePointManager;
        Singleton.Instance.PlayerCollisionControllerInstance.OnPlayerDeathEvent += StopScorePointsManager;
    }

    void StartScorePointsManager()
    {
        _currentScore = 0;

        StartCoroutine(ScorePointsRoutine(_scorePointsPerSecond));
    }

    IEnumerator ScorePointsRoutine(int scorePointsPerSecond)
    {
        float timeToWait = 1f / (float)scorePointsPerSecond;

        while(true)
        {
            yield return new WaitForSeconds(timeToWait);
            _currentScore++;
            _scoreTextField.text = _currentScore.ToString();
        }
    }

    void ResetScorePointsManager()
    {
        _currentScore = 0;
        _scoreTextField.text = _currentScore.ToString();
    }

    void StopScorePointsManager()
    {
        StopAllCoroutines();
        Debug.Log($"Your score: {_currentScore}");
        _scoreTextField.text = _currentScore.ToString();
    }

    void ContinueScorePointManager()
    {
        StartCoroutine(ScorePointsRoutine(_scorePointsPerSecond));
    }

    private void OnDisable()
    {
        Singleton.Instance.LevelGenerationManagerInstance.StartLevelEvent -= StartScorePointsManager;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent -= ResetScorePointsManager;
        Singleton.Instance.LevelGenerationManagerInstance.ContinueLevelEvent -= ContinueScorePointManager;
        Singleton.Instance.PlayerCollisionControllerInstance.OnPlayerDeathEvent -= StopScorePointsManager;
    }
}
