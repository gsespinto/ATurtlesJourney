using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeashellScript : MonoBehaviour
{
    private ScoreScript scoreScript;
    [SerializeField] private int scoreValue;

    void Start()
    {
        scoreScript = GameObject.FindObjectOfType<ScoreScript>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!scoreScript)
        {
            return;
        }

        if (other.gameObject.tag == "HeadBox" || other.gameObject.tag == "BodyBox")
        {
            scoreScript.CollectShell(scoreValue);
        }
    }
}
