using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public delegate void MovementDelegate(bool[] swipes);
    public event MovementDelegate Movement;

    public enum Direction { Left, Right, Up, Down};
    bool[] swipeData = new bool[4];

    Vector2 startTouchPosition;
    Vector2 swipeMovementDelta;

    bool touchMoved;

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
            startTouchPosition = TouchPosition();
            touchMoved = true;
        }
        else if (TouchEnded() && touchMoved == true)
        {
            SendSwipe();
            touchMoved = false;
        }

        swipeMovementDelta = Vector2.zero;
        if(touchMoved && GetTouch())
        {
            swipeMovementDelta = TouchPosition() - startTouchPosition;
        }

        if(swipeMovementDelta.sqrMagnitude > MIN_SWIPE_DIST * MIN_SWIPE_DIST)
        {
            if(Mathf.Abs(swipeMovementDelta.x) > Mathf.Abs(swipeMovementDelta.y))
            {
                swipeData[(int)Direction.Left] = swipeMovementDelta.x < 0;
                swipeData[(int)Direction.Right] = swipeMovementDelta.x > 0;
            } else
            {
                swipeData[(int)Direction.Down] = swipeMovementDelta.y < 0;
                swipeData[(int)Direction.Up] = swipeMovementDelta.y > 0;
            }

            SendSwipe();
        }
    }

    void SendSwipe()
    {
        if (swipeData[0] || swipeData[1] || swipeData[2] || swipeData[3])
        {
            Debug.Log($"{swipeData[0]} || {swipeData[1]} || {swipeData[2]} || {swipeData[3]}");
            Movement?.Invoke(swipeData);
        }
        ResetArray();
    }

    void ResetArray()
    {
        startTouchPosition = swipeMovementDelta = Vector2.zero;
        touchMoved = false;
        for (int i = 0; i< 4; i++) { swipeData[i] = false; }
    }
}
