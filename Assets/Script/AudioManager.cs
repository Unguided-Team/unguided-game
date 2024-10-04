using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("SFX")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip dash;
    public AudioClip land;
    public AudioClip attack;
    public AudioClip takedamage;
    public AudioClip bonfireidle;
    public AudioClip specialAttack;
    public AudioClip playerWalkStep;

    [Header("BGM")]
    public AudioClip defaultPiano;
    public AudioClip bossFightIntro;
    public AudioClip bossFightMainLoop;
    public AudioClip bossFightOutro;

    [Header("Music Fade Settings")]
    [SerializeField] private float fadeInSpeed = 0.05f;
    [SerializeField] private float fadeOutSpeed = 0.05f;
    
    [Header("Volume Setting")]
    [Range(0f, 100f)] public float musicVolume = 0.5f; // Volume for music
    [Range(0f, 100f)] public float sfxVolume = 0.5f;
    
    private void Start()
    {
            musicSource.clip = background;
            musicSource.loop = true; // Enable looping
            musicSource.volume = musicVolume; // Set the background music volume
            musicSource.Play();
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("Attempted to play a BGM, but the clip is null!");
            return; // Exit the method if the clip is null
        }

        musicSource.volume = musicVolume;
        musicSource.GetComponent<AudioSource>().Play(0);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("Attempted to play a sound effect, but the clip is null!");
            return; // Exit the method if the clip is null
        }
        SFXSource.volume = sfxVolume;
        SFXSource.PlayOneShot(clip);
    }

    public void StartFadeInBGM(AudioClip clip)
    {
        StartCoroutine(FadeInBGM(clip));
    }

    public void StartFadeOutBGM(AudioClip clip = null)
    {
        StartCoroutine(FadeOutBGM(clip));
    }

    public IEnumerator FadeInBGM(AudioClip clip)
    {
        float maxVol =  musicSource.volume;
        musicSource.volume = 0;
        musicSource.GetComponent<AudioSource>().clip = clip;
        musicSource.GetComponent<AudioSource>().Play(0);

        while (musicSource.volume < maxVol)
        {
            musicSource.volume += (musicSource.volume + fadeInSpeed <= maxVol ? fadeInSpeed : maxVol - musicSource.volume);
            yield return new WaitForSeconds(0.1f);
        }
        musicSource.volume = maxVol;
    }

    public IEnumerator FadeOutBGM(AudioClip clip = null)
    {
        while (musicSource.volume > 0)
        {
            musicSource.volume -= (musicSource.volume + fadeOutSpeed > 0 ? fadeOutSpeed : musicSource.volume);
            yield return new WaitForSeconds(0.1f);
        }
        musicSource.volume = 0;
    }

    // finish boss music
    public void startBossMusic()
    {
        // StartFadeOutBGM();
    }
}

