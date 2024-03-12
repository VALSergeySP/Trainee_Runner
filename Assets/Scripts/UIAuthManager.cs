using UnityEngine;
using DG.Tweening;

public class UIAuthManager : MonoBehaviour
{
    [SerializeField] RectTransform _authMenu;
    [SerializeField] RectTransform _registrationMenu;
    [SerializeField] Vector2 _transformOffset;
    [SerializeField] float _movementTime;

    private void Start()
    {
        // �������� �������� �� ������ ������ ���� � �������� ���� �����������
        // �������� �������� �� ��� �������������� ������������ � ��������� ����
        _authMenu.DOAnchorPos(Vector2.zero, 0f);
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
}
