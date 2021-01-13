using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    #region Variables
    private GameManager gameManager;
    [SerializeField] private Image loadingIcon;
    [SerializeField] private Sprite[] iconSprites;
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1f;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        StartCoroutine(SceneLoader());
        loadingIcon.sprite = iconSprites[Random.Range(0, iconSprites.Length)];
    }

    IEnumerator SceneLoader()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadSceneAsync(gameManager.sceneToLoad);
            gameManager.sceneToLoad = "";
            yield return null;
        }
    }
}
