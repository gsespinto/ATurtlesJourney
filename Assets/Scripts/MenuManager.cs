using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    #region Variables
    private GameManager gameManager;
    [SerializeField] private bool isMainMenu;
    [SerializeField] private Text runnerHighScoreTXT;
    [SerializeField] private Text evadeHighScoreTXT;
    [SerializeField] private Text controlsTXT;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject controlUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject creditsUI;
    [SerializeField] private AudioClip btSound;
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private ScoreScript scoreScript;

    [SerializeField] private Slider[] volumeSlider;
    [SerializeField] private AudioMixer audioMixer;

    private AudioSource audioSource;
    private bool playedSea;
    private bool playedSorrow;
    private bool playedStar;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        if (GameObject.FindGameObjectWithTag("GameController") != null)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            if (isMainMenu)
            {
                runnerHighScoreTXT.text = "RUNNER MODE HIGHSCORE: " + gameManager.runnerHighscore;
                evadeHighScoreTXT.text = "EVADE MODE HIGHSCORE: " + gameManager.evadeHighscore;
#if UNITY_STANDALONE_WIN || UNITY_WIN || UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX || UNITY_LINUX || UNITY_WEBGL
                controlsTXT.text = "CONTROLS:\nARROW KEYS - Movement\nESCAPE - Pause\n\nGAME ELEMENTS:";
#elif UNITY_EDITOR_ANDROID || UNITY_STANDALONE_ANDROID  || UNITY_ANDROID
                controlsTXT.text = "CONTROLS:\nTOUCH POSITION - Movement\n\nGAME ELEMENTS:";
#endif
                }
                //InitialAudioMixer();
            }
        for (int i = 0; i < volumeSlider.Length; i++)
        {
            switch (i)
            {
                case 0:
                    volumeSlider[i].value = gameManager.masterVolume;
                    break;
                case 1:
                    volumeSlider[i].value = gameManager.musicVolume;
                    break;
                case 2:
                    volumeSlider[i].value = gameManager.ambientVolume;
                    break;
                case 3:
                    volumeSlider[i].value = gameManager.sfxVolume;
                    break;
            }
            ChangeVolume(i);
        }
    }

    private void Update()
    {
        PlayRandomMusic();
    }

    public void LoadLevel(string level)
    {
        gameManager.LoadScene(level);
    }

    public void ShowControls(bool show)
    {
        controlUI.SetActive(show);
        mainUI.SetActive(!show);
    }

    public void ShowStettings(bool show)
    {
        settingsUI.SetActive(show);
        mainUI.SetActive(!show);
    }

    public void ShowCredits(bool show)
    {
        creditsUI.SetActive(show);
        mainUI.SetActive(!show);
    }

    public void PlayBTSound()
    {
        audioSource.PlayOneShot(btSound);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenURL(string URL)
    {
        Application.OpenURL(URL);
    }

    void PlayRandomMusic()
    {
        AudioSource musicSource = Camera.main.GetComponent<AudioSource>();
        if (!musicSource.isPlaying)
        {
            int musicIndex = Random.Range(0, musicClips.Length-3);
            if (musicClips[musicIndex] == musicSource.clip)
                musicIndex = Random.Range(0, musicClips.Length-3);
            musicSource.clip = musicClips[musicIndex];
            musicSource.Play();
        }

        if (!isMainMenu)
        {
            if (scoreScript.currentScore > 1000 && !playedSea)
            {
                musicSource.Pause();
                musicSource.clip = musicClips[musicClips.Length - 3];
                musicSource.Play();
                playedSea = true;
            }
            else if (scoreScript.currentScore > 1500 && !playedSorrow)
            {
                musicSource.Pause();
                musicSource.clip = musicClips[musicClips.Length - 2];
                musicSource.Play();
                playedSorrow = true;
            }
            else if (scoreScript.currentScore > 2000 && !playedStar)
            {
                musicSource.Pause();
                musicSource.clip = musicClips[musicClips.Length - 1];
                musicSource.Play();
                playedStar = true;
            }
        }
    }

    void InitialAudioMixer()
    {
        audioMixer.SetFloat("masterVolume", gameManager.masterVolume);
        audioMixer.SetFloat("musicVolume", gameManager.masterVolume);
        audioMixer.SetFloat("ambientVolume", gameManager.masterVolume);
        audioMixer.SetFloat("sfxVolume", gameManager.masterVolume);
    }

    public void ChangeVolume(int index)
    {
        float value;
        switch (index)
        {
            case 0:
                audioMixer.SetFloat("masterVolume", volumeSlider[index].value);
                if (volumeSlider[index].value == volumeSlider[index].minValue)
                    audioMixer.SetFloat("masterVolume", -80);
                audioMixer.GetFloat("masterVolume", out value);
                volumeSlider[index].value = value;
                gameManager.masterVolume = value;
                gameManager.UpdateInfo();
                break;
            case 1:
                audioMixer.SetFloat("musicVolume", volumeSlider[index].value);
                if (volumeSlider[index].value == volumeSlider[index].minValue)
                    audioMixer.SetFloat("musicVolume", -80);
                audioMixer.GetFloat("musicVolume", out value);
                volumeSlider[index].value = value;
                gameManager.musicVolume = value;
                gameManager.UpdateInfo();
                break;
            case 2:
                audioMixer.SetFloat("ambientVolume", volumeSlider[index].value);
                if (volumeSlider[index].value == volumeSlider[index].minValue)
                    audioMixer.SetFloat("ambientVolume", -80);
                audioMixer.GetFloat("ambientVolume", out value);
                volumeSlider[index].value = value;
                gameManager.ambientVolume = value;
                gameManager.UpdateInfo();
                break;
            case 3:
                audioMixer.SetFloat("sfxVolume", volumeSlider[index].value);
                if (volumeSlider[index].value == volumeSlider[index].minValue)
                    audioMixer.SetFloat("sfxVolume", -80);
                audioMixer.GetFloat("sfxVolume", out value);
                volumeSlider[index].value = value;
                gameManager.sfxVolume = value;
                gameManager.UpdateInfo();
                break;

        }
    }
}
