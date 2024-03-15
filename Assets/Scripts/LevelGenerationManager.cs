using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerationManager : MonoBehaviour
{
    public delegate void LevelDelegate();
    public event LevelDelegate ResetLevelEvent;
    public event LevelDelegate StartLevelEvent;
    public event LevelDelegate ContinueLevelEvent;
    public event LevelDelegate PauseLevelEvent;

    [SerializeField] Vector3 PART_OFFSET = new(0, 0, 10); // Офсет части уровня (длина одной части)
    [SerializeField] GameObject[] _levelPartPrefabs; // Части для генерации окружения
    List<Transform> _levelParts = new();

    [SerializeField] float _maxMovementSpeed = 10f;
    [SerializeField] float _startSpeed = 3f;
    [SerializeField] float _timeToMaxSpeedInSec = 120f; // Время, через которое скорость будет максимальной
    float _currentSpeed = 0f;
    float _stopSpeed;
    public float CurrentSpeed=> _currentSpeed;
    float _speedChangeStep;
    [SerializeField] int _maxLevelParts = 5; // Максимальное число видимых частей уровня

    AudioSource _audioSource;

    bool _isFirstStart = true;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();


        ResetLevel();

        Singleton.Instance.PlayerCollisionControllerInstance.OnPlayerDeathEvent += StopLevel;
    }

    void Update()
    {
        if (_currentSpeed == 0f) { return; }
        
        foreach (var levelPart in _levelParts)
        {
            levelPart.position -= new Vector3(0, 0, _currentSpeed * Time.deltaTime);
        }

        if (_levelParts.Count > 0)
        {
            if (_levelParts[0].position.z < -PART_OFFSET.z)
            {
                Singleton.Instance.PoolManagerInstance.Despawn(_levelParts[0].gameObject);
                _levelParts.RemoveAt(0);

                CreateNewLevelPart();
            }
        }
    }

    public void StopLevel() // Пауза уровня
    {
        _stopSpeed = _currentSpeed;
        _currentSpeed = 0f;

        PauseLevelEvent?.Invoke();

        StopAllCoroutines();
    }

    public void ContinueLevel() // Продолжить после паузы
    {
        _currentSpeed = _stopSpeed;
        StartCoroutine(SpeedChangeRoutine());

        ContinueLevelEvent?.Invoke();
    }

    public void ResetLevel() // Сброс уровня к начальному состоянию
    {
        _speedChangeStep = (_maxMovementSpeed - _startSpeed) / _timeToMaxSpeedInSec;
        _currentSpeed = 0f;

        StopAllCoroutines();

        while (_levelParts.Count > 0)
        {
            Singleton.Instance.PoolManagerInstance.Despawn(_levelParts[0].gameObject);
            _levelParts.RemoveAt(0);
        }

        for (int i = 0; i < _maxLevelParts; i++)
        {
            CreateNewLevelPart();
        }

        ResetLevelEvent?.Invoke();
        if (Singleton.Instance.BackgroundSoundManagerInstance != null && !_isFirstStart)
        {
            Singleton.Instance.BackgroundSoundManagerInstance.StopGameBackgroundMusic();
        }

        _isFirstStart = false;
    }

    public void StartLevel() // Начало забега
    {
        _currentSpeed = _startSpeed;
        StartLevelEvent?.Invoke();
        StartCoroutine(SpeedChangeRoutine());
        _audioSource.Play();
        if (Singleton.Instance.BackgroundSoundManagerInstance != null)
        {
            Singleton.Instance.BackgroundSoundManagerInstance.StartGameBackgroundMusic();
        }    
    }

    IEnumerator SpeedChangeRoutine()
    {
        while(_currentSpeed < _maxMovementSpeed)
        {
            yield return new WaitForSeconds(1f);

            _currentSpeed += _speedChangeStep;
        }

        _currentSpeed = _maxMovementSpeed;
    }

    private void CreateNewLevelPart()
    {
        Vector3 position = Vector3.zero;
        if(_levelParts.Count > 0) { position = _levelParts[^1].position + PART_OFFSET; }

        int num = Random.Range(0, _levelPartPrefabs.Length);

        Transform newPart = Singleton.Instance.PoolManagerInstance.Spawn(_levelPartPrefabs[num], position, Quaternion.identity).transform;
        newPart.SetParent(transform);
        _levelParts.Add(newPart);
    }
}
