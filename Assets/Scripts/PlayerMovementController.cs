using System.Collections;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    Vector3 _playerStartPosition;
    Quaternion _playerStartRotation;

    float _laneOffset;
    [SerializeField] float _laneChangeSpeed = 15f;
    [SerializeField] float _jumpForce = 15f;
    [SerializeField] float _slidingTime = 1.5f;

    Rigidbody _rb;
    Animator _animator;

    float _pointStart;
    float _pointFinish;

    bool _isMoving = false;
    bool _isJumping = false;
    bool _isSliding = false;
    Coroutine _movementRoutine;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _playerStartPosition = transform.position;
        _playerStartRotation = transform.rotation;

        Singleton.Instance.SwipeManagerInstance.Movement += MovePlayer;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += ResetPlayer;
        Singleton.Instance.PlayerCollisionControllerInstance.OnPlayerDeathEvent += OnPlayerDeath;
        _laneOffset = Singleton.Instance.ObstaclesGenerationManagerInstance.LaneOffset;
    }

    void OnPlayerDeath()
    {
        if(_isMoving)
        {
            StopCoroutine(_movementRoutine);
        }
    }


    void ResetPlayer()
    {
        _animator.ResetTrigger("StopSliding");
        _animator.ResetTrigger("StartSliding");

        _rb.velocity = Vector3.zero;
        _isMoving = false;
        _isJumping = false;
        _isSliding = false;
        _pointStart = 0f;
        _pointFinish = 0f;

        transform.position = _playerStartPosition;
        transform.rotation = _playerStartRotation;
    }

    void MovePlayer(bool[] swipeDirections) 
    {
        if (swipeDirections[(int)SwipeManager.Direction.Left] && _pointFinish > -_laneOffset)
        {
            MoveHorizontal(-_laneChangeSpeed);
        }
        if (swipeDirections[(int)SwipeManager.Direction.Right] && _pointFinish < _laneOffset)
        {
            MoveHorizontal(_laneChangeSpeed);
        }
        if (swipeDirections[(int)SwipeManager.Direction.Up] && !_isJumping && !_isSliding)
        {
            Jump();
        }
        if (swipeDirections[(int)SwipeManager.Direction.Down] && _isJumping)
        {
            StopJump();
        }
        if (swipeDirections[(int)SwipeManager.Direction.Down] && !_isJumping && !_isSliding)
        {
            Slide();
        }
        if (swipeDirections[(int)SwipeManager.Direction.Up] && _isSliding)
        {
            StopSlide();
        }
    }

    void StopSlide()
    {
        StopCoroutine(SlidingRoutine());
        _isSliding = false;
        _animator.SetTrigger("StopSliding");
    }

    void Slide()
    {
        _isSliding = true;
        _animator.ResetTrigger("StopSliding");

        StartCoroutine(SlidingRoutine());
    }

    IEnumerator SlidingRoutine()
    {
        _animator.SetTrigger("StartSliding");

        yield return new WaitForSeconds(_slidingTime);

        _isSliding = false;
        _animator.SetTrigger("StopSliding");
    }

    void StopJump()
    {
        _rb.AddForce(Vector3.down * _jumpForce, ForceMode.Impulse);
    }

    void Jump()
    {
        _isJumping = true;
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

        StartCoroutine(StopJumpingRoutine());
    }

    IEnumerator StopJumpingRoutine()
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (_rb.velocity.y != 0);

        _isJumping = false;
    }

    void MoveHorizontal(float speed)
    {
        _pointStart = _pointFinish;
        _pointFinish += Mathf.Sign(speed) * _laneOffset;

        if(_isMoving)
        {
            StopCoroutine(_movementRoutine);
            _isMoving = false;
        }
        _movementRoutine = StartCoroutine(MoveRoutine(speed));
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
