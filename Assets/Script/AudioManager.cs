using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject boss;

    [Header("Audio Source")]
    public AudioSource musicSource;
    public AudioSource SFXSource;

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

    private bool killedByBossMidSong = false;
    private bool startedBossIntro = false;
    private bool startedBossOutro = false;
    
    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true; // Enable looping
        musicSource.volume = musicVolume; // Set the background music volume
        musicSource.Play();

        boss.GetComponent<BossMain>().bossFightStarted.AddListener(startBossMusicIntroCoroutine);
        boss.GetComponent<BossMain>().bossDied.AddListener(startBossOutro);
        // boss.GetComponent<BossMain>().bossKilledPlayer.AddListener(fadeToDefaultBGM);
        boss.GetComponent<BossMain>().bossKilledPlayer.AddListener(() => {killedByBossMidSong = true;});

        boss.GetComponent<BossMain>().bossFightStarted.AddListener(() => {Debug.Log("boss fight started");});
        boss.GetComponent<BossMain>().bossDied.AddListener(() => {Debug.Log("boss died");});
        boss.GetComponent<BossMain>().bossKilledPlayer.AddListener(() => {Debug.Log("boss killed player");});
    }

    public void Update()
    {
        if (startedBossIntro && !musicSource.isPlaying && !killedByBossMidSong && !startedBossOutro)
        {
            startBossMusicLoop();
            startedBossIntro = false;
        }

        if (startedBossOutro && !musicSource.isPlaying && !killedByBossMidSong)
        {
            changeToDefaultBGM();
            startedBossOutro = false;
            startedBossIntro = false;
        }

        if (killedByBossMidSong)
        {
            fadeToDefaultBGM();
            killedByBossMidSong = false;
            startedBossOutro = false;
            startedBossIntro = false;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("Attempted to play a BGM, but the clip is null!");
            return; // Exit the method if the clip is null
        }

        musicSource.volume = musicVolume;
        musicSource.clip = clip;
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

    public void StartFadeInBGM(AudioClip clip)
    {
        StartCoroutine(FadeInBGM(clip));
    }

    public void StartFadeOutBGM(AudioClip clip = null)
    {
        StartCoroutine(FadeOutBGM(clip));
    }

    private void StartFadeTransitionTo(AudioClip clip)
    {
        StartCoroutine(FadeTransitionTo(clip));
    }

    private void startBossMusicIntroCoroutine()
    {
        StartCoroutine(startBossMusic());
    }

    private IEnumerator FadeInBGM(AudioClip clip)
    {
        float maxVol =  musicVolume;
        musicSource.volume = 0;
        // musicSource.GetComponent<AudioSource>().clip = clip;
        // musicSource.GetComponent<AudioSource>().Play(0);
        musicSource.clip = clip;
        musicSource.Play();

        while (musicSource.volume < maxVol)
        {
            musicSource.volume += (musicSource.volume + fadeInSpeed <= maxVol ? fadeInSpeed : maxVol - musicSource.volume);
            yield return new WaitForSeconds(0.1f);
        }
        musicSource.volume = maxVol;
    }

    private IEnumerator FadeOutBGM(AudioClip clip = null)
    {
        while (musicSource.volume > 0)
        {
            musicSource.volume -= fadeOutSpeed;
            yield return new WaitForSeconds(0.1f);
        }
        musicSource.volume = 0;
    }

    private IEnumerator FadeTransitionTo(AudioClip clip)
    {
        while (musicSource.volume > 0)
        {
            musicSource.volume -= fadeOutSpeed;
            yield return new WaitForSeconds(0.1f);
        }
        musicSource.volume = 0;

        float maxVol =  musicVolume;
        musicSource.volume = 0;
        // musicSource.GetComponent<AudioSource>().clip = clip;
        // musicSource.GetComponent<AudioSource>().Play(0);
        musicSource.clip = clip;
        musicSource.Play();

        while (musicSource.volume < maxVol)
        {
            musicSource.volume += (musicSource.volume + fadeInSpeed <= maxVol ? fadeInSpeed : maxVol - musicSource.volume);
            yield return new WaitForSeconds(0.1f);
        }
        musicSource.volume = maxVol;
    }

    // start boss music
    public IEnumerator startBossMusic()
    {
        killedByBossMidSong = false;

        // fade out boss music
        while (musicSource.volume > 0)
        {
            musicSource.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        
        
        musicSource.volume = musicVolume;
        if (!startedBossOutro)
            PlayBGM(bossFightIntro);
        musicSource.loop = false;

        startedBossIntro = true;
        // StartCoroutine(changeFromIntroToMainLoop(bossFightIntro));
    }

    // fix with new method [UNUSED]
    private IEnumerator changeFromIntroToMainLoop(AudioClip intro)
    {
        yield return new WaitForSeconds(ClipDuration(intro));
        if (!killedByBossMidSong)
            startBossMusicLoop();
    }

    public void startBossMusicLoop()
    {
        PlayBGM(bossFightMainLoop);
        musicSource.loop = true;
    }

    public void startBossOutro()
    {
        PlayBGM(bossFightOutro);
        startedBossOutro = true;
        musicSource.loop = false;

        // StartCoroutine(changeFromOutroToDefault(bossFightIntro));
    }

    private IEnumerator changeFromOutroToDefault(AudioClip outro)
    {
        yield return new WaitForSeconds(outro.length); 
        if (!killedByBossMidSong)
            changeToDefaultBGM();
    }

    public void changeToDefaultBGM()
    {
        if (musicSource.clip == defaultPiano)
            return;

        musicSource.Stop();
        StartFadeInBGM(defaultPiano);
        musicSource.loop = true;
    }

    public void fadeToDefaultBGM()
    {
        if (musicSource.clip == defaultPiano)
            return;
        StartFadeTransitionTo(defaultPiano);
        print("yes");
        musicSource.loop = true;
    }

    private float ClipDuration(AudioClip clip)
    {
        return ((float) clip.samples / clip.frequency);
    }

}

