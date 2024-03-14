using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    public delegate void CollisionPlayerDelegate();
    public event CollisionPlayerDelegate OnPlayerDeathEvent;
    public event CollisionPlayerDelegate OnItemCollectedEvent;

    [SerializeField] float _onRespawnInvincTime = 1f;
    bool _canBeDamaged = true;


    private void Start()
    {
        Singleton.Instance.LevelGenerationManagerInstance.ContinueLevelEvent += ContinuePlayerCollisions;
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += ResetInvinc;
    }

    void ContinuePlayerCollisions()
    {
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
            Singleton.Instance.SwipeManagerInstance.enabled = false;
            OnPlayerDeathEvent?.Invoke();
            _canBeDamaged = false;
        } else if (other.CompareTag("Collectable"))
        {
            Singleton.Instance.PoolManagerInstance.Despawn(other.gameObject);
            OnItemCollectedEvent?.Invoke();
            Debug.Log("Object collected!");
        }
    }
}
