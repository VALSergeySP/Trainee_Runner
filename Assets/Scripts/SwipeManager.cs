using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public delegate void MovementDelegate(bool[] swipes);
    public event MovementDelegate Movement;

    public enum Direction { Left, Right, Up, Down};
    bool[] _swipeData = new bool[4];

    Vector2 _startTouchPosition;
    Vector2 _swipeMovementDelta;

    bool _touchMoved;

    const float MIN_SWIPE_DIST = 50f;

    Vector2 TouchPosition() { return (Vector2)Input.mousePosition; }
    bool TouchBegan() { return Input.GetMouseButtonDown(0); }
    bool TouchEnded() { return Input.GetMouseButtonUp(0); }
    bool GetTouch() { return Input.GetMouseButton(0); }

    private void Start()
    {
        Singleton.Instance.LevelGenerationManagerInstance.StartLevelEvent += StartSwipeManager;
        enabled = false;
    }

    void StartSwipeManager()
    {
        enabled = true;
    }

    void Update() // «менить на вызов корутины
    {
        if (TouchBegan())
        {
            _startTouchPosition = TouchPosition();
            _touchMoved = true;
        }
        else if (TouchEnded() && _touchMoved == true)
        {
            SendSwipe();
            _touchMoved = false;
        }

        _swipeMovementDelta = Vector2.zero;
        if(_touchMoved && GetTouch())
        {
            _swipeMovementDelta = TouchPosition() - _startTouchPosition;
        }

        if(_swipeMovementDelta.sqrMagnitude > MIN_SWIPE_DIST * MIN_SWIPE_DIST)
        {
            if(Mathf.Abs(_swipeMovementDelta.x) > Mathf.Abs(_swipeMovementDelta.y))
            {
                _swipeData[(int)Direction.Left] = _swipeMovementDelta.x < 0;
                _swipeData[(int)Direction.Right] = _swipeMovementDelta.x > 0;
            } else
            {
                _swipeData[(int)Direction.Down] = _swipeMovementDelta.y < 0;
                _swipeData[(int)Direction.Up] = _swipeMovementDelta.y > 0;
            }

            SendSwipe();
        }
    }

    void SendSwipe()
    {
        if (_swipeData[0] || _swipeData[1] || _swipeData[2] || _swipeData[3])
        {
            Debug.Log($"{_swipeData[0]} || {_swipeData[1]} || {_swipeData[2]} || {_swipeData[3]}");
            Movement?.Invoke(_swipeData);
        }
        ResetArray();
    }

    void ResetArray()
    {
        _startTouchPosition = _swipeMovementDelta = Vector2.zero;
        _touchMoved = false;
        for (int i = 0; i< 4; i++) { _swipeData[i] = false; }
    }
}
