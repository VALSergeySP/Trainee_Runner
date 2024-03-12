using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] RectTransform _mainMenu;
    [SerializeField] RectTransform _deathMenu;
    [SerializeField] Vector2 _movementOffset;
    [SerializeField] float _movementTime = 0.25f;

    void Start()
    {
        _mainMenu.DOAnchorPos(Vector2.zero, 0f);

        Singleton.Instance.LevelGenerationManagerInstance.StartLevelEvent += OnGameStart;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += OnGameReset;
        Singleton.Instance.PlayerCollisionControllerInstance.OnPlayerDeathEvent += OnPlayerDeath;
    }


    void OnGameStart()
    {
        _mainMenu.DOAnchorPos(new Vector2(_movementOffset.x, -_movementOffset.y), _movementTime);
    }

    void OnPlayerDeath()
    {
        _deathMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    void OnGameReset()
    {
        _deathMenu.DOAnchorPos(_movementOffset, _movementTime);
        _mainMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    public void OnRespawnButton() // Test button
    {
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevel();
    }

    public void ExitFromGameButton()
    {
#if UNITY_EDITOR
        Debug.Log("Game exit!");
#endif
        Application.Quit();
    }
}
