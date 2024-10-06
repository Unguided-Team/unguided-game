using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerRe : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject boss;

    [Header("Audio Source")]
    public AudioSource[] musicSource;
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
    // [SerializeField] private float fadeInSpeed = 0.05f;
    // [SerializeField] private float fadeOutSpeed = 0.05f;
    
    [Header("Volume Setting")]
    [Range(0f, 100f)] public float musicVolume = 1f; // Volume for music
    [Range(0f, 100f)] public float sfxVolume = 0.4f;

    // private bool killedByBossMidSong = false;

    private int selectedMusicSource = 0;
    private double goalTime;

    private bool looping = false;

    private void Start()
    {
        goalTime = AudioSettings.dspTime;
        LoopSong(defaultPiano);

        boss.GetComponent<BossMain>().bossFightStarted.AddListener(PlayBossIntro);
        // boss.GetComponent<BossMain>().bossDied.AddListener(startBossOutro);
        // boss.GetComponent<BossMain>().bossKilledPlayer.AddListener(fadeToDefaultBGM);
        // boss.GetComponent<BossMain>().bossKilledPlayer.AddListener(() => {killedByBossMidSong = true;});

        boss.GetComponent<BossMain>().bossFightStarted.AddListener(() => {Debug.Log("boss fight started");});
        boss.GetComponent<BossMain>().bossDied.AddListener(() => {Debug.Log("boss died");});
        boss.GetComponent<BossMain>().bossKilledPlayer.AddListener(() => {Debug.Log("boss killed player");});
    }

    private void Update()
    {
        if (AudioSettings.dspTime == goalTime)
            looping = true;
        if (looping)
        {
            LoopSong(musicSource[selectedMusicSource].clip);
            looping = false;
        }
    }

    private void LoopSong(AudioClip clip) 
    {
        PlayScheduledClip(clip, ClipDuration(clip), true);
    }

    private void PlayScheduledClip(AudioClip clip, double musicDuration, bool shouldLoop = false)
    {
        selectedMusicSource = 1 - selectedMusicSource;
        musicSource[selectedMusicSource].clip = clip;
        musicSource[selectedMusicSource].PlayScheduled(goalTime);
        musicSource[selectedMusicSource].loop = false;
        goalTime += musicDuration;
        looping = shouldLoop;
    }

    private double ClipDuration(AudioClip clip)
    {
        return ((double) clip.samples / clip.frequency);
    }

    private void PlayBossIntro()
    {
        musicSource[selectedMusicSource].Stop();
        looping = false;
        musicSource[selectedMusicSource].clip = bossFightIntro;
        musicSource[selectedMusicSource].Play();
    }

}
