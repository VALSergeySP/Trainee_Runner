using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIAuthManager : MonoBehaviour
{
    [SerializeField] RectTransform _authMenu;
    [SerializeField] RectTransform _registrationMenu;
    [SerializeField] RectTransform _loadingMenu;
    [SerializeField] Vector2 _transformOffset;
    [SerializeField] float _movementTime;

    private void Start()
    {
        // Добавить проверку на первый запуск игры и открытие меню регистрации

        _loadingMenu.DOAnchorPos(new Vector2(0, 2000), _movementTime);
        _authMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    public void OnAuthorization()
    {
        _registrationMenu.DOAnchorPos(_transformOffset, _movementTime);
        _authMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    public void OnRegistration()
    {
        _authMenu.DOAnchorPos(new Vector2(-_transformOffset.x, _transformOffset.y), _movementTime);
        _registrationMenu.DOAnchorPos(Vector2.zero, _movementTime);
    }

    public void OnSuccessLogin()
    {
        _loadingMenu.DOAnchorPos(Vector2.zero, _movementTime);

        Invoke(nameof(LoadMainMenuScene), _movementTime);
    }

    void LoadMainMenuScene()
    {
        SceneManager.LoadScene(1);
    }
}
