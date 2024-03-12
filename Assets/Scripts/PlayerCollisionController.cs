using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    public delegate void OnPlayerDeath();
    public event OnPlayerDeath OnPlayerDeathEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
            Singleton.Instance.SwipeManagerInstance.enabled = false; // Нужно будет заменить позже
            OnPlayerDeathEvent();
        }
    }
}
