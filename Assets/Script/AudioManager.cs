using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] PlayerSettings playerSettings;

    public static AudioManager Instance;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource sfxSourceBackground;


    public List<AudioClip> clips; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(int clipIndex, bool loop = true, float? volume = null)
    {
        if (!playerSettings.getAudio())
            return;

        musicSource.clip = clips[clipIndex];
        musicSource.loop = loop;

        if (volume.HasValue)
            musicSource.volume = Mathf.Clamp01(volume.Value);

        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusicSFXBack(int clipIndex, bool loop = true, float? volume = null)
    {
        if (!playerSettings.getAudio())
            return;

        sfxSourceBackground.clip = clips[clipIndex];
        sfxSourceBackground.loop = loop;

        if (volume.HasValue)
            musicSource.volume = Mathf.Clamp01(volume.Value);

        sfxSourceBackground.Play();
    }

    public void StopMusicSFXBack()
    {
        sfxSourceBackground.Stop();
    }


    public void PlaySFX(int clipindex, float volume = 1f)
    {
        if (!playerSettings.getAudio())
            return;

        sfxSource.PlayOneShot(clips[clipindex], Mathf.Clamp01(volume));
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}
