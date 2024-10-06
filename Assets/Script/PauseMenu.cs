using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject PauseMenuPanel;
    private AudioManager audioManager;

    void Start()
    {
        PauseMenuPanel.SetActive(false);
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1;
        audioManager.musicSource.volume = audioManager.musicVolume; 
        audioManager.SFXSource.pitch = Time.timeScale;
        GameIsPaused = false;
    }

    private void Pause()
    {
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        audioManager.musicSource.volume = audioManager.musicVolume * 0.25f;
        audioManager.SFXSource.pitch = Time.timeScale;
        GameIsPaused = true;
    }

    // create scene change for loadmenu here

    public void QuitGame()
    {
        Application.Quit();
    }
}
