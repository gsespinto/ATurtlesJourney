using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    public float currentScore;
    public int shells = 0;

    [SerializeField] private float highScore;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text shellsText;
    [SerializeField] private Text highText;
    [SerializeField] private Text deathScoreText;
    [SerializeField] private PlayerScript playerScript;
    
    private float initialX;
    private float lastX;
    private GameManager gameManager;

    void Start()
    {
        currentScore = 0;
        initialX = this.transform.position.x;
        lastX = this.transform.position.x;

        gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            if (playerScript.autoRunner)
                highScore = gameManager.runnerHighscore;
            else
                highScore = gameManager.evadeHighscore;
        }
        else
            highScore = 0;

        highText.text = "High Score: " + highScore;
        scoreText.text = "Score: " + currentScore;

        if (!playerScript.autoRunner)
            StartCoroutine(SurviveScore());

        UpdateShellText();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerScript.isDead && !playerScript.isPaused)
            UpdateScore();
    }

    void UpdateScore()
    {
        if (playerScript.autoRunner)
        {
            if (this.transform.position.x - initialX > 0 && lastX < this.transform.position.x)
            {
                lastX = this.transform.position.x;
                currentScore = lastX - initialX;
                currentScore = Mathf.RoundToInt(currentScore);
            }
        }

        UpdateScoreText();
        UpdateGameManager();
    }

    public void CollectShell(int scoreValue)
    {
        shells++;
        currentScore += scoreValue;
        UpdateShellText();
    }

    void UpdateShellText()
    {
        shellsText.text = "x" + shells;
    }

    IEnumerator SurviveScore(bool run = true)
    {
        while (run)
        {
            yield return new WaitForSeconds(1f);
            currentScore += 3;
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + currentScore;
        deathScoreText.text = "Final Score: " + currentScore;
        if (currentScore > highScore)
            highScore = currentScore;
        highText.text = "High Score: " + highScore;
    }

    void UpdateGameManager()
    {
        if (gameManager != null)
        {
            if (playerScript.autoRunner)
                gameManager.runnerHighscore = highScore;
            else
                gameManager.evadeHighscore = highScore;

            gameManager.currentShells = shells;

            gameManager.UpdateInfo();
        }
    }
}
