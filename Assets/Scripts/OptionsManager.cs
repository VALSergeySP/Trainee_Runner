using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] TMP_Text[] _soundLabels;
    [SerializeField] Slider[] _soundSliders;

    private void Start()
    {
        _soundSliders[0].value = Normalize(GetVolumeLevel("masterVolume"));
        _soundLabels[0].text = Mathf.RoundToInt(Normalize(GetVolumeLevel("masterVolume")) * 100f).ToString() + " %";
        _soundSliders[1].value = Normalize(GetVolumeLevel("soundEffectsVolume"));
        _soundLabels[1].text = Mathf.RoundToInt(Normalize(GetVolumeLevel("soundEffectsVolume")) * 100f).ToString() + " %";
        _soundSliders[2].value = Normalize(GetVolumeLevel("musicVolume"));
        _soundLabels[2].text = Mathf.RoundToInt(Normalize(GetVolumeLevel("musicVolume")) * 100f).ToString() + " %";
    }

    private float GetVolumeLevel(string name)
    {
        float value;
        bool result = _audioMixer.GetFloat(name, out value);
        if (result)
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }

    public void SetMasterVolume(float volume)
    {
        _soundLabels[0].text = Mathf.RoundToInt(volume * 100).ToString() + " %";
        if (Singleton.Instance.VolumeManagerInstance != null)
        {
            Singleton.Instance.VolumeManagerInstance.AdjustMasterVolume(Unnormalize(volume));
        }
        else
        {
            _audioMixer.SetFloat("masterVolume", Unnormalize(volume));
        }
    }

    public void SetSoundEffectsVolume(float volume)
    {
        _soundLabels[1].text = Mathf.RoundToInt(volume * 100).ToString() + " %";
        if (Singleton.Instance.VolumeManagerInstance != null)
        {
            Singleton.Instance.VolumeManagerInstance.AdjustEffectsVolume(Unnormalize(volume));
        }
        else
        {
            _audioMixer.SetFloat("soundEffectsVolume", Unnormalize(volume));
        }
    }
    public void SetMusicVolume(float volume)
    {
        _soundLabels[2].text = Mathf.RoundToInt(volume * 100).ToString() + " %"; 
        if (Singleton.Instance.VolumeManagerInstance != null)
        {
            Singleton.Instance.VolumeManagerInstance.AdjustMusicVolume(Unnormalize(volume));
        } 
        else
        {
            _audioMixer.SetFloat("musicVolume", Unnormalize(volume));
        }
    }


    private float Unnormalize(float volume)
    {
        if (volume != 0f)
        {
            return 20f * Mathf.Log(volume);
        }
        else
        {
            return -80f;
        }
    }

    private float Normalize(float volume)
    {
        if (volume != -80f)
        {
            return Mathf.Exp(volume / 20);
        }
        else
        {
            return 0;
        }
    }
}
