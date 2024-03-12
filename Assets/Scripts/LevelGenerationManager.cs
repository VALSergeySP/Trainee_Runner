using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerationManager : MonoBehaviour
{
    public delegate void ResetLevelDelegate();
    public event ResetLevelDelegate ResetLevelEvent;

    public delegate void StartLevelDelegate();
    public event StartLevelDelegate StartLevelEvent;

    [SerializeField] Vector3 PART_OFFSET = new(0, 0, 10);
    [SerializeField] GameObject _levelPartPrefab;
    List<Transform> _levelParts = new();

    [SerializeField] float _maxMovementSpeed = 10f;
    [SerializeField] float _startSpeed = 3f;
    [SerializeField] float _timeToMaxSpeedInSec = 120f;
    float _currentSpeed = 0f;
    public float CurrentSpeed=> _currentSpeed;
    float _speedChangeStep;
    [SerializeField] int _maxLevelParts = 5;

    void Start()
    {
        ResetLevel();
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

    public void ResetLevel()
    {
        _speedChangeStep = (_maxMovementSpeed - _startSpeed) / _timeToMaxSpeedInSec;
        _currentSpeed = 0f;

        StopAllCoroutines();

        while(_levelParts.Count > 0)
        {
            Singleton.Instance.PoolManagerInstance.Despawn(_levelParts[0].gameObject);
            _levelParts.RemoveAt(0);
        }

        for (int i = 0; i < _maxLevelParts; i++)
        {
            CreateNewLevelPart();
        }

        ResetLevelEvent?.Invoke();
    }

    public void StartLevel()
    {
        _currentSpeed = _startSpeed;
        StartLevelEvent?.Invoke();
        StartCoroutine(SpeedChangeRoutine());
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

        Transform newPart = Singleton.Instance.PoolManagerInstance.Spawn(_levelPartPrefab, position, Quaternion.identity).transform;
        newPart.SetParent(transform);
        _levelParts.Add(newPart);
    }
}
