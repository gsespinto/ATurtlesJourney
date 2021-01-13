using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
public class GameManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private AudioClip loadAudio;
    private AudioSource audioSource;
    public string sceneToLoad;
    public bool setInfo = false;

    //_____PLAYER_STATS_____//
    private string filePath;
    public string playerName;
    public float runnerHighscore;
    public float evadeHighscore;
    public int currentShells;

    public float masterVolume;
    public float musicVolume;
    public float ambientVolume;
    public float sfxVolume;

    [SerializeField] private AudioMixer audioMixer;

    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
            Destroy(gameObject);

        filePath = Application.persistentDataPath + "/savefile.txt";
        Debug.Log(filePath);
        GetInfo();
    }

    public void LoadScene(string _sceneToLoad)
    {
        sceneToLoad = _sceneToLoad;
        SceneManager.LoadScene("SceneLoader");
        audioSource.PlayOneShot(loadAudio);
    }

    private void GetInfo()
    {
        if (!File.Exists(filePath))
        {
            SetInfo();
        }
        else
        {
            setInfo = false;
            string fileContent = File.ReadAllText(filePath);
            fileContent = SimpleXOREncryption.EncryptorDecryptor.EncryptDecrypt(fileContent);
            runnerHighscore = int.Parse(RegexCaptureToString(fileContent, @"(?isx)<runnerHighscore>(?<capture>.*?)<runnerHighscore>"));
            evadeHighscore = int.Parse(RegexCaptureToString(fileContent, @"(?isx)<evadeHighscore>(?<capture>.*?)<evadeHighscore>"));
            // currentShells = int.Parse(RegexCaptureToString(fileContent, @"(?isx)<currentShells>(?<capture>.*?)<currentShells>"));
            masterVolume = float.Parse(RegexCaptureToString(fileContent, @"(?isx)<masterVolume>(?<capture>.*?)<masterVolume>"));
            musicVolume = float.Parse(RegexCaptureToString(fileContent, @"(?isx)<musicVolume>(?<capture>.*?)<musicVolume>"));
            ambientVolume = float.Parse(RegexCaptureToString(fileContent, @"(?isx)<ambientVolume>(?<capture>.*?)<ambientVolume>"));
            sfxVolume = float.Parse(RegexCaptureToString(fileContent, @"(?isx)<sfxVolume>(?<capture>.*?)<sfxVolume>"));
        }
    }

    private string RegexCaptureToString(string input, string pattern) //Property of Ivo yes
    {
        string capture = "";
        Regex rg = new Regex(pattern);
        Match m = rg.Match(input);
        capture = m.Groups["capture"].Value;
        return capture;
    }

    public void SetInfo()
    {
        string _runnerHighscore = "<runnerHighscore>" + 0 + "<runnerHighscore>";
        string _surviveHighscore = "<evadeHighscore>" + 0 + "<evadeHighscore>";
        string _currentShells = "<currentShells>" + 0 + "<currentShells>";
        string _masterVolume = "<masterVolume>" + 10 + "<masterVolume>";
        string _musicVolume = "<musicVolume>" + -27 + "<musicVolume>";
        string _ambientVolume = "<ambientVolume>" + -20 + "<ambientVolume>";
        string _sfxVolume = "<sfxVolume>" + -40 + "<sfxVolume>";
        string[] lines = { _runnerHighscore, _surviveHighscore, /*_currentShells,*/ _masterVolume, _musicVolume, _ambientVolume, _sfxVolume};

        System.IO.File.WriteAllLines(filePath, lines);
        string fileContent = File.ReadAllText(filePath);
        fileContent = SimpleXOREncryption.EncryptorDecryptor.EncryptDecrypt(fileContent);
        System.IO.File.WriteAllText(filePath, fileContent);
        GetInfo();
    }

    public void UpdateInfo()
    {
        string _runnerHighscore = "<runnerHighscore>" + runnerHighscore + "<runnerHighscore>";
        string _surviveHighscore = "<evadeHighscore>" + evadeHighscore + "<evadeHighscore>";
        string _currentShells = "<currentShells>" + currentShells + "<currentShells>";
        string _masterVolume = "<masterVolume>" + masterVolume + "<masterVolume>";
        string _musicVolume = "<musicVolume>" + musicVolume + "<musicVolume>";
        string _ambientVolume = "<ambientVolume>" + ambientVolume + "<ambientVolume>";
        string _sfxVolume = "<sfxVolume>" + sfxVolume + "<sfxVolume>";
        string[] lines = { _runnerHighscore, _surviveHighscore, /*_currentShells,*/ _masterVolume, _musicVolume, _ambientVolume, _sfxVolume };

        System.IO.File.WriteAllLines(filePath, lines);
        string fileContent = File.ReadAllText(filePath);
        fileContent = SimpleXOREncryption.EncryptorDecryptor.EncryptDecrypt(fileContent);
        System.IO.File.WriteAllText(filePath, fileContent);
    }

    public void ResetGame()
    {
        File.Delete(filePath);
        GetInfo();
        LoadScene("MainMenu");
    }
}

namespace SimpleXOREncryption
{
    public static class EncryptorDecryptor
    {
        public static int key = 129;

        public static string EncryptDecrypt(string textToEncrypt)
        {
            StringBuilder inSb = new StringBuilder(textToEncrypt);
            StringBuilder outSb = new StringBuilder(textToEncrypt.Length);
            char c;
            for (int i = 0; i < textToEncrypt.Length; i++)
            {
                c = inSb[i];
                c = (char)(c ^ key);
                outSb.Append(c);
            }
            return outSb.ToString();
        }
    }
}
