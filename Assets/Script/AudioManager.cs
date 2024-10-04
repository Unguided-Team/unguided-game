using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]

    public AudioClip background;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip dash;
    public AudioClip land;
    public AudioClip attack;
    public AudioClip takedamage;
    public AudioClip bonfireidle;
    
    [Header("Volume Settings")]
    [Range(0f, 100f)] public float musicVolume = 0.5f; // Volume for music
    [Range(0f, 100f)] public float sfxVolume = 0.5f;
    
    private void Start()
    {
            musicSource.clip = background;
            musicSource.loop = true; // Enable looping
            musicSource.volume = musicVolume; // Set the background music volume
            musicSource.Play();
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
}

