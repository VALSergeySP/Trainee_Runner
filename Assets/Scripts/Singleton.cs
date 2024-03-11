using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    private Singleton() { }
    public static Singleton Instance = null;

    [SerializeField] private LevelGenerationManager _levelGenerationManager;
    public LevelGenerationManager LevelGeneration => _levelGenerationManager;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
