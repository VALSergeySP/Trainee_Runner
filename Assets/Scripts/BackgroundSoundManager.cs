using UnityEngine;

public class BackgroundSoundManager : MonoBehaviour
{
    AudioSource _auidoSource;
    [SerializeField] AudioClip _slowMusic;
    [SerializeField] AudioClip _fastMusic;

    private static bool _wasCreated = false;


    public void StartGameBackgroundMusic() // Музыка для меню
    {
        _auidoSource.Stop();
        _auidoSource.clip = _fastMusic;
        _auidoSource.Play();
    }

    public void StopGameBackgroundMusic() // Музыка для игры (во время бега)
    {
        _auidoSource.Stop();
        _auidoSource.clip = _slowMusic;
        _auidoSource.Play();
    }

    void Start()
    {
        if (_wasCreated == false)
        {
            _wasCreated = true;
            _auidoSource = GetComponent<AudioSource>();

            if (!_auidoSource.isPlaying)
            {
                _auidoSource.clip = _slowMusic;
                _auidoSource.Play();
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
