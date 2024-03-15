using System.Collections;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    Vector3 _playerStartPosition;
    Quaternion _playerStartRotation;

    float _laneOffset;
    [SerializeField] float _laneChangeSpeed = 15f;
    [SerializeField] float _jumpForce = 15f;
    [SerializeField] float _jumpStopCoef = 1.5f;
    [SerializeField] float _slidingTime = 1.5f;

    [SerializeField] Collider _fullColider;
    [SerializeField] Collider _lowColider;

    Rigidbody _rb;
    Animator _animator;
    AudioSource _audioSource;

    [SerializeField] AudioClip _slideSound;
    [SerializeField] AudioClip _jumpSound;

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
        _audioSource = GetComponent<AudioSource>();

        _playerStartPosition = transform.position;
        _playerStartRotation = transform.rotation;

        Singleton.Instance.SwipeManagerInstance.Movement += MovePlayer;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += ResetPlayer;
        Singleton.Instance.LevelGenerationManagerInstance.StartLevelEvent += StartPlayer;
        Singleton.Instance.LevelGenerationManagerInstance.ContinueLevelEvent += StartPlayer;
        Singleton.Instance.LevelGenerationManagerInstance.PauseLevelEvent += OnPlayerDeath;
        Singleton.Instance.PlayerCollisionControllerInstance.PlayerOnTheGround += ResetJump;
        _laneOffset = Singleton.Instance.ObstaclesGenerationManagerInstance.LaneOffset;
    }

    void StartPlayer()
    {
        _animator.SetTrigger("StartRunning");
        _animator.SetBool("IsPlayerDead", false);
    }

    void OnPlayerDeath()
    {
        _animator.SetBool("IsPlayerDead", true);
    }


    void ResetPlayer()
    {
        _animator.SetTrigger("StopRunning");
        _animator.SetBool("IsPlayerSliding", false);
        _animator.SetBool("IsPlayerJumping", false);
        _animator.SetBool("IsPlayerDead", false);
        _animator.ResetTrigger("StopSliding");
        _animator.ResetTrigger("StartSliding");

        _rb.velocity = Vector3.zero;
        _rb.interpolation = RigidbodyInterpolation.None;
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
            Jump();
        }
    }

    void StopSlide() // Прерывание подката
    {
        StopCoroutine(SlidingRoutine());
        _isSliding = false;
        _fullColider.enabled = true;
        _lowColider.enabled = false;

        _animator.SetBool("IsPlayerSliding", false);
    }

    void ResetJump(bool isOnTheGround) // Сброс прыжка при касании к земле
    {
        Debug.Log(isOnTheGround);
        if(isOnTheGround && _isJumping)
        {
            //StopCoroutine(StopJumpingRoutine());
            _isJumping = false;
            _animator.SetBool("IsPlayerJumping", false);
        }
    }

    void Slide() // Подкат
    {
        _isSliding = true;
        _fullColider.enabled = false;
        _lowColider.enabled = true;

        _audioSource.PlayOneShot(_slideSound);
        StartCoroutine(SlidingRoutine());
    }

    IEnumerator SlidingRoutine()
    {
        _animator.SetBool("IsPlayerSliding", true);

        yield return new WaitForSeconds(_slidingTime);

        _isSliding = false;

        _animator.SetBool("IsPlayerSliding", false);

        yield return new WaitForSeconds(0.1f);

        _fullColider.enabled = true;
        _lowColider.enabled = false;
    }

    void StopJump() // Остановка прыжка
    {
        _rb.AddForce(_jumpForce * _jumpStopCoef * Vector3.down, ForceMode.Impulse);
    }

    void Jump()
    {
        _isJumping = true;
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

        _animator.SetBool("IsPlayerJumping", true);

        //StartCoroutine(StopJumpingRoutine());
        _audioSource.PlayOneShot(_jumpSound);
    }

    IEnumerator StopJumpingRoutine()
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (_rb.velocity.y != 0);

        _isJumping = false;
        
        _animator.SetBool("IsPlayerJumping", false);
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
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
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

        _rb.interpolation = RigidbodyInterpolation.None;
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        transform.position = new Vector3(_pointFinish, transform.position.y, transform.position.z);
        _isMoving = false;
    }
}
