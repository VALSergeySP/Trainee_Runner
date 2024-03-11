using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerationManager : MonoBehaviour
{
    [SerializeField] Vector3 PART_OFFSET = new(0, 0, 10);
    [SerializeField] GameObject _levelPartPrefab;
    List<Transform> _levelParts = new();

    [SerializeField] float _maxMovementSpeed = 10f;
    [SerializeField] float _startSpeed = 3f;
    float _currentSpeed = 0f;
    [SerializeField] int _maxLevelParts = 5;

    void Start()
    {
        ResetLevel();
        InitializeLevel();
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
            if (_levelParts[0].position.z < -PART_OFFSET.z) // TO DO: Заменить на пул позже
            {
                Destroy(_levelParts[0].gameObject);
                _levelParts.RemoveAt(0);

                CreateNewLevelPart();
            }
        }
    }

    public void ResetLevel()
    {
        _currentSpeed = 0f;
        while(_levelParts.Count > 0) // TO DO: Заменить на пул позже
        {
            Destroy(_levelParts[0].gameObject);
            _levelParts.RemoveAt(0);
        }

        for (int i = 0; i < _maxLevelParts; i++)
        {
            CreateNewLevelPart();
        }
    }

    public void InitializeLevel()
    {
        _currentSpeed = _startSpeed;
    }

    private void CreateNewLevelPart()
    {
        Vector3 position = Vector3.zero;
        if(_levelParts.Count > 0) { position = _levelParts[^1].position + PART_OFFSET; }

        Transform newPart = Instantiate(_levelPartPrefab, position, Quaternion.identity).transform;
        newPart.SetParent(transform);
        _levelParts.Add(newPart);
    }
}
