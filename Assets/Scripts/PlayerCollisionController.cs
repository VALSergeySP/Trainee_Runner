using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    public delegate void CollisionPlayerDelegate();
    public event CollisionPlayerDelegate OnPlayerDeathEvent;
    public event CollisionPlayerDelegate OnItemCollectedEvent;
    public delegate void BoolCollisionPlayerDelegate(bool param);
    public event BoolCollisionPlayerDelegate PlayerOnTheGround;


    [SerializeField] float _onRespawnInvincTime = 1f;
    bool _canBeDamaged = true;
    bool _isPlayerOnTheGround = true;


    AudioSource _audioSource;
    [SerializeField] AudioClip _hitSound;
    [SerializeField] AudioClip _collectSound;

    GameObject lastCollisonObject;


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        Singleton.Instance.LevelGenerationManagerInstance.ContinueLevelEvent += ContinuePlayerCollisions;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += ResetInvinc;
    }

    void ContinuePlayerCollisions()
    {
        Singleton.Instance.PoolManagerInstance.Despawn(lastCollisonObject);
        Invoke(nameof(ResetInvinc), _onRespawnInvincTime);
    }

    void ResetInvinc()
    {
        _canBeDamaged = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle") && _canBeDamaged)
        {
            lastCollisonObject = other.gameObject;
            _audioSource.PlayOneShot(_hitSound);
            Singleton.Instance.SwipeManagerInstance.enabled = false;
            OnPlayerDeathEvent?.Invoke();
            _canBeDamaged = false;
        } else if (other.CompareTag("Collectable"))
        {
            _audioSource.PlayOneShot(_collectSound);
            Singleton.Instance.PoolManagerInstance.Despawn(other.gameObject);
            OnItemCollectedEvent?.Invoke();
            Debug.Log("Object collected!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider != null && !_isPlayerOnTheGround)
        {
            PlayerOnTheGround?.Invoke(true);
            _isPlayerOnTheGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider != null && _isPlayerOnTheGround)
        {
            PlayerOnTheGround?.Invoke(false);
            _isPlayerOnTheGround = false;
        }
    }
}
