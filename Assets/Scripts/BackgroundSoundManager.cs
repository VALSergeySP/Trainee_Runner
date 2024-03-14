using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSoundManager : MonoBehaviour
{
    AudioSource _auidoSource;
    [SerializeField] AudioClip _slowMusic;
    [SerializeField] AudioClip _fastMusic;

    public void StartGameBackgroundMusic()
    {
        _auidoSource.Stop();
        _auidoSource.clip = _fastMusic;
        _auidoSource.Play();
    }

    public void StopGameBackgroundMusic()
    {
        _auidoSource.Stop();
        _auidoSource.clip = _slowMusic;
        _auidoSource.Play();
    }

    void Start()
    {
        _auidoSource = GetComponent<AudioSource>();
        _auidoSource.Stop();
        _auidoSource.clip = _slowMusic;
        _auidoSource.Play();
        DontDestroyOnLoad(gameObject);
    }
}
