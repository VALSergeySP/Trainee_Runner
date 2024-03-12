using System.Collections;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    float _laneOffset;
    [SerializeField] float _laneChangeSpeed = 15f;
    Rigidbody _rb;

    float _pointStart;
    float _pointFinish;

    bool _isMoving;
    Coroutine movementRoutine;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Singleton.Instance.SwipeManagerInstance.Movement += MovePlayer;
        _laneOffset = Singleton.Instance.ObstaclesGenerationManagerInstance.LaneOffset;
    }

    void MovePlayer(bool[] swipeDirections) 
    {
        if (swipeDirections[0] && _pointFinish > -_laneOffset)
        {

            MoveHorizontal(-_laneChangeSpeed);
        }
        if (swipeDirections[1] && _pointFinish < _laneOffset)
        {
            MoveHorizontal(_laneChangeSpeed);
        }
    }

    void MoveHorizontal(float speed)
    {
        _pointStart = _pointFinish;
        _pointFinish += Mathf.Sign(speed) * _laneOffset;

        if(_isMoving)
        {
            StopCoroutine(movementRoutine);
            _isMoving = false;
        }
        movementRoutine = StartCoroutine(MoveRoutine(speed));
    }

    IEnumerator MoveRoutine(float speed)
    {
        _isMoving = true;
        while(Mathf.Abs(_pointStart - transform.position.x) < _laneOffset)
        {
            yield return new WaitForFixedUpdate();

            _rb.velocity = new Vector3(speed, _rb.velocity.y, 0);

            float x = Mathf.Clamp(transform.position.x, Mathf.Min(_pointStart, _pointFinish), Mathf.Max(_pointStart, _pointFinish));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        _rb.velocity = Vector3.zero;
        transform.position = new Vector3(_pointFinish, transform.position.y, transform.position.z);
        _isMoving = false;
    }
}
